// GameLogic.cs
// Furious Koalas S.L.
// 2023

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Block;
using static LevelButtons;
using static LevelObject;
using static MessageScreenManager;

/// <summary>
/// La clase <see cref="GameLogic" /> contiene la lógica del robot que se mueve por el mapa y determina
/// cuando se gana o se pierde la partida, etc.
/// </summary>
public class GameLogic : MonoBehaviour
{
    /// <summary>
    /// El escenario del juego.
    /// </summary>
    [SerializeField] private GameObject placeableMap;

    /// <summary>
    /// Determina si se debe mostrar la pantalla de excepción cuando el
    /// jugador pierde el nivel. 
    /// </summary>
    [SerializeField] public bool showExceptionScreen = false;

    /// <summary>
    /// El padre de los objetos que componen el mapa.
    /// </summary>
    private GameObject mapParent;

    /// <summary>
    /// Estructura de datos que contiene el nivel actual.
    /// </summary>
    private LevelData currentLevelData;

    /// <summary>
    /// Contiene las referencias a los bloques del nivel.
    /// </summary>
    private LevelObject[] objectReferences;

    /// <summary>
    /// Contiene los items del nivel.
    /// </summary>
    private Item[] items;

    /// <summary>
    /// Datos originales del mapa sin modificar para poder hacer el reseteo.
    /// </summary>
    private LevelData clonedLevelData;

    /// <summary>
    /// El inventario del jugador.
    /// </summary>
    private Stack<Item> inventory = new Stack<Item>();

    /// <summary>
    /// Diccionario de acciones.
    /// </summary>
    private Dictionary<Buttons, Action> buttonActionsDictionary;

    /// <summary>
    /// Flag que determina si se ha de parar la ejecución de instrucciones.
    /// </summary>
    private bool haltExecution = false;

    /// <summary>
    /// Determina si el nivel está cargando.
    /// </summary>
    private bool loading = true;

    /// <summary>
    /// Instancia de MessageWarehouse.
    /// </summary>
    private MessageWarehouse msgWar;

    /// <summary>
    /// El robot que se mueve por el mapa.
    /// </summary>
    private GameObject bigCharater;

    /// <summary>
    /// Determina si el robot de las carreteras ha acabado de moverse.
    /// </summary>
    private bool finishedMinibotMovement = false;

    /// <summary>
    /// Determina si el robot de las carreteras ha acabado de moverse.
    /// </summary>
    public bool FinishedMinibotMovement { get { return finishedMinibotMovement; } set { finishedMinibotMovement = value; CheckState(); } }

    /// <summary>
    /// Estructura de datos que contiene el nivel actual.
    /// </summary>
    public LevelData CurrentLevelData { get => currentLevelData; set => currentLevelData = value; }

    /// <summary>
    /// Awake.
    /// </summary>
    internal void Awake()
    {
        msgWar = new MessageWarehouse(EventAggregator.Instance);
    }

    /// <summary>
    /// Start.
    /// </summary>
    internal void Start()
    {
        //Llenar el diccionario de funciones
        buttonActionsDictionary = new Dictionary<Buttons, Action>();
        buttonActionsDictionary.Add(Buttons.Action, DoAction);
        buttonActionsDictionary.Add(Buttons.Condition, DoCondition);
        buttonActionsDictionary.Add(Buttons.Jump, DoJump);
        buttonActionsDictionary.Add(Buttons.Loop, DoLoop);
        buttonActionsDictionary.Add(Buttons.Move, DoMove);
        buttonActionsDictionary.Add(Buttons.Play, DoPlay);
        buttonActionsDictionary.Add(Buttons.Restart, DoRestart);
        buttonActionsDictionary.Add(Buttons.TurnLeft, DoTurnLeft);
        buttonActionsDictionary.Add(Buttons.TurnRight, DoTurnRight);
        buttonActionsDictionary.Add(Buttons.MapMenu, GoToMainMenu);
    }

    /// <summary>
    /// Instancia de la clase a la que otros objetos pueden acceder.
    /// </summary>
    private static GameLogic gameLogic;

    /// <summary>
    /// Retorna la instancia de la clase.
    /// </summary>
    public static GameLogic Instance
    {
        get
        {
            if (!gameLogic)
            {
                gameLogic = FindObjectOfType(typeof(GameLogic)) as GameLogic;

                if (!gameLogic)
                {
                    Debug.LogError("There needs to be one active GameLogic script on a GameObject in your scene.");
                }
            }

            return gameLogic;
        }
    }

    /// <summary>
    /// Da comienzo a un nivel del juego.
    /// </summary>
    /// <param name="levelData">Los datos del nivel<see cref="LevelData"/>.</param>
    /// <param name="mapParent">El padre de los objetos del nivel<see cref="GameObject"/>.</param>
    public void StartLevel(LevelData levelData, GameObject mapParent)
    {
        this.loading = true;
        this.haltExecution = true;
        finishedMinibotMovement = false;
        placeableMap.GetComponent<MapController>().EnableGameControls();

        inventory = new Stack<Item>();
        haltExecution = false;

        clonedLevelData = levelData.Clone();
        currentLevelData = levelData.Clone();
        items = null;

        this.mapParent = mapParent;
        StartCoroutine(RenderALevel());
    }

    /// <summary>
    /// Determina el punto apropiado de la superficie de un bloque para que el robot se mueva a este.
    /// </summary>
    /// <param name="x">Coordenada x<see cref="int"/>.</param>
    /// <param name="y">Coordenada y<see cref="int"/>.</param>
    /// <param name="z">Coordenada z<see cref="int"/>.</param>
    /// <param name="surfacePoint">Parametro de salida con el punto apropiado <see cref="Vector3"/>.</param>
    /// <returns>True si se ha encontrado, false si no <see cref="bool"/>.</returns>
    private bool GetBlockSurfacePoint(in int x, in int y, in int z, out Vector3 surfacePoint)
    {
        LevelObject rawObj = GetBlock(CurrentLevelData, objectReferences, x, y, z);

        if (rawObj != null)
        {
            if (rawObj is Block)
            {
                Block b = (Block)rawObj;
                surfacePoint = b.SurfacePoint;
                return true;
            }
        }

        surfacePoint = new Vector3();
        return false;
    }

    /// <summary>
    /// Comprueba si se ha ganado o perdido la partida.
    /// </summary>
    private void CheckState()
    {
        if (!haltExecution)
        {
            if (CheckWinState())
            {
                haltExecution = true;
                StartCoroutine(YouWin());
            }
            else if (CheckLoseState())
            {
                haltExecution = true;
                StartCoroutine(YouLose());
            }
        }
    }

    /// <summary>
    /// Comprueba si se ha ganado la partida.
    /// </summary>
    /// <returns>True si si, false si no <see cref="bool"/>.</returns>
    private bool CheckWinState()
    {
        if (currentLevelData.goal[0] == currentLevelData.playerPos[0] &&
            currentLevelData.goal[1] == currentLevelData.playerPos[1] &&
            currentLevelData.goal[2] == currentLevelData.playerPos[2])
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Comprueba si se ha perdido la partida.
    /// </summary>
    /// <returns>True si si, false si no <see cref="bool"/>.</returns>
    private bool CheckLoseState()
    {
        if (CheckBlockProperty(currentLevelData.playerPos[0], currentLevelData.playerPos[1] - 1, currentLevelData.playerPos[2], currentLevelData, BlockProperties.Dangerous)
            || !IsPositionInsideMap(currentLevelData.playerPos, currentLevelData) || (finishedMinibotMovement && !CheckWinState()))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Muestra la pantalla de win cuando el robot termine sus acciones.
    /// </summary>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator YouWin()
    {
        haltExecution = true;
        Debug.Log("A winner is you");

        MsgBigCharacterAllActionsFinished msg = new MsgBigCharacterAllActionsFinished();
        msgWar.PublishMsgAndWaitForResponse<MsgBigCharacterAllActionsFinished, bool>(msg);
        bool outTmp;
        yield return new WaitUntil(() => msgWar.IsResponseReceived<MsgBigCharacterAllActionsFinished, bool>(msg, out outTmp));

        EventAggregator.Instance.Publish(new MsgBigRobotAction(MsgBigRobotAction.BigRobotActions.Win, new Vector3()));
        EventAggregator.Instance.Publish<MsgShowScreen>(new MsgShowScreen("win", new Tuple<string, OnMessageScreenButtonPressed>[] {
            Tuple.Create<string, OnMessageScreenButtonPressed>("Yes", YesButton),
            Tuple.Create<string, OnMessageScreenButtonPressed>("No", NoButton)}));
    }

    /// <summary>
    /// Muestra la pantalla de lose cuando el robot termine sus acciones.
    /// </summary>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator YouLose()
    {
        //Se ha determinado que el jugador ha perdido la partida
        haltExecution = true;
        Debug.Log("A loser is you");

        //Sin embargo la lógica y los gráficos no están sincronizados por lo que seguramente
        //el robot no haya terminado de realizar sus acciones.
        //Con el objeto msgWar instancia de la clase MessageWarehouse.cs podemos publicar un mensaje
        //y esperar hasta que la respuesta esté lista.
        MsgBigCharacterAllActionsFinished msg = new MsgBigCharacterAllActionsFinished();
        msgWar.PublishMsgAndWaitForResponse<MsgBigCharacterAllActionsFinished, bool>(msg);
        bool outTmp;
        yield return new WaitUntil(() => msgWar.IsResponseReceived<MsgBigCharacterAllActionsFinished, bool>(msg, out outTmp));

        //Evitando así que la pantalla que pregunta al jugador si quiere reintentar el nivel se muestre antes de tiempo
        EventAggregator.Instance.Publish(new MsgBigRobotAction(MsgBigRobotAction.BigRobotActions.Lose, new Vector3()));
        if (showExceptionScreen)
        {
            EventAggregator.Instance.Publish<MsgShowScreen>(new MsgShowScreen("lose", new Tuple<string, OnMessageScreenButtonPressed>[] {
            Tuple.Create<string, OnMessageScreenButtonPressed>("Yes", YesButton),
            Tuple.Create<string, OnMessageScreenButtonPressed>("No", NoButton)}));
        }
        else
        {
            //Hacemos restart sin preguntar
            yield return new WaitForSeconds(2f);
            YesButton();
        }
    }

    /// <summary>
    /// Lógica del botón yes.
    /// </summary>
    private void YesButton()
    {
        RoadPlacementLogic.Instance.ResetRoad();
        DoRestart();
    }

    /// <summary>
    /// Lógica del botón no.
    /// </summary>
    private void NoButton()
    {
        RoadPlacementLogic.Instance.ResetRoad();
        GoToMainMenu();
    }

    /// <summary>
    /// Publica un mensaje para que los contadores de las instrucciones restantes actualicen su número.
    /// </summary>
    public void UpdateAvailableInstructions()
    {
        EventAggregator.Instance.Publish<MsgSetAvInstructions>(new MsgSetAvInstructions(currentLevelData.availableInstructions));
    }

    /// <summary>
    /// Genera los objetos del nivel actual.
    /// </summary>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator RenderALevel()
    {
        this.loading = true;
        this.haltExecution = true;

        //Reseteamos el inventario
        inventory = new Stack<Item>();

        //Reseteamos el leveldata
        currentLevelData = clonedLevelData.Clone();

        //Pedimos que se renderice el nivel
        MsgRenderMapAndItems msg = new MsgRenderMapAndItems(currentLevelData.mapAndItems, currentLevelData.levelSize, currentLevelData.goal, mapParent);
        LevelObject[] loadedLevel = null;
        msgWar.PublishMsgAndWaitForResponse<MsgRenderMapAndItems, LevelObject[]>(msg);
        yield return new WaitUntil(() => msgWar.IsResponseReceived<MsgRenderMapAndItems, LevelObject[]>(msg, out loadedLevel));

        if (loadedLevel != null)
        {
            //Ponemos el numero de instrucciones en los botones
            EventAggregator.Instance.Publish<MsgSetAvInstructions>(new MsgSetAvInstructions(currentLevelData.availableInstructions));

            objectReferences = loadedLevel;

            if (bigCharater == null)
            {
                MsgRenderMainCharacter msgMain = new MsgRenderMainCharacter();
                msgWar.PublishMsgAndWaitForResponse<MsgRenderMainCharacter, GameObject>(msgMain);
                yield return new WaitUntil(() => msgWar.IsResponseReceived<MsgRenderMainCharacter, GameObject>(msgMain, out bigCharater));
                bigCharater.SetActive(true);
            }

            Vector3 playerPos;

            //Podría dar fallo si el personaje esta mal colocado
            GetBlockSurfacePoint(currentLevelData.playerPos[0], currentLevelData.playerPos[1] - 1, currentLevelData.playerPos[2], out playerPos);

            MsgPlaceCharacter msgLld = new MsgPlaceCharacter(playerPos, new Vector3(0, 90f * currentLevelData.playerOrientation, 0), mapParent.transform);
            msgWar.PublishMsgAndWaitForResponse<MsgPlaceCharacter, GameObject>(msgLld);
            yield return new WaitUntil(() => msgWar.IsResponseReceived<MsgPlaceCharacter, GameObject>(msgLld, out bigCharater));

            bigCharater.SetActive(false);

            bool doReturn = true;
            for (int y = 0; y < currentLevelData.levelSize[1]; y++)
            {
                for (int x = 0; x < currentLevelData.levelSize[0]; x++)
                {
                    for (int z = 0; z < currentLevelData.levelSize[2]; z++)
                    {
                        doReturn = true;

                        if (currentLevelData.playerPos[0] == x && currentLevelData.playerPos[1] - 1 == y && currentLevelData.playerPos[2] == z)
                        {
                            bigCharater.SetActive(true);
                        }

                        try
                        {
                            LevelObject lOb = GetBlock(currentLevelData, loadedLevel, x, y, z);
                            lOb.gameObject.SetActive(true);

                            if (lOb is Block)
                            {
                                Block lOb2 = (Block)lOb;
                                if (lOb2.BlockType == Blocks.NoBlock)
                                {
                                    doReturn = false;
                                }
                            }
                        }
                        catch
                        {
                        }
                        if (doReturn)
                        {
                            yield return null;
                        }
                    }
                }
            }

            //Separar los items y en su lugar spawnear noblocks
            Vector3 itemPosition;
            Quaternion itemRotation;
            Transform itemParent;
            items = new Item[loadedLevel.Length];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = null;
            }
            for (int y = 0; y < currentLevelData.levelSize[1]; y++)
            {
                for (int x = 0; x < currentLevelData.levelSize[0]; x++)
                {
                    for (int z = 0; z < currentLevelData.levelSize[2]; z++)
                    {
                        if ((int)GetBlockType(currentLevelData, x, y, z) >= 25)
                        {
                            SetBlockType(currentLevelData, (int)Blocks.NoBlock, x, y, z);
                            Item thisItem = GetBlock(currentLevelData, loadedLevel, x, y, z).GetComponent<Item>();

                            if (thisItem != null && thisItem.IsItem())
                            {
                                items[x + z * currentLevelData.levelSize[0] + y * (currentLevelData.levelSize[0] * currentLevelData.levelSize[2])] = thisItem;
                                itemPosition = thisItem.transform.position;
                                itemRotation = thisItem.transform.rotation;
                                itemParent = thisItem.transform.parent;
                                LevelObject spawnedObject = MapRenderer.Instance.SpawnBlock((int)Blocks.NoBlock);
                                spawnedObject.gameObject.transform.parent = itemParent;
                                spawnedObject.gameObject.transform.position = itemPosition;
                                spawnedObject.gameObject.transform.rotation = itemRotation;
                                SetBlock(currentLevelData, loadedLevel, x, y, z, spawnedObject);
                            }
                        }
                    }
                }
            }

            objectReferences = loadedLevel;
            //Por si acaso hay un item en la casilla inicial o está el jugador en mala posición
            CheckState();
            TakeItem();
        }

        haltExecution = false;
        this.loading = false;
    }

    /// <summary>
    /// Ejecuta las acciones apropiadas en respuesta a la pulsación de un botón.
    /// </summary>
    /// <param name="button">El botón pulsado <see cref="Buttons"/>.</param>
    public void AddInputFromButton(Buttons button)
    {
        if (button == Buttons.Restart)
        {
            haltExecution = false;
        }

        if (!haltExecution)
        {
            try
            {
                buttonActionsDictionary[button]();
            }
            catch
            {
                Debug.LogError("Unknown input: " + button.ToString());
            }
        }
    }

    /// <summary>
    /// Retorna al menú principal.
    /// </summary>
    private void GoToMainMenu()
    {
        EventAggregator.Instance.Publish<MsgHideAllScreens>(new MsgHideAllScreens());
        finishedMinibotMovement = false;
        if (bigCharater != null)
        {
            Destroy(bigCharater);
            bigCharater = null;
        }
        currentLevelData = null;
        clonedLevelData = null;
        if (objectReferences != null)
        {
            StartCoroutine(DestroyLevelObjectsOnBackground((LevelObject[])objectReferences.Clone(), (Item[])items.Clone()));
            objectReferences = null;
            items = null;
        }

        MainMenuLogic.Instance.ShowMainMenu();
    }

    /// <summary>
    /// Destruye los objetos de forma que no afecta al rendimiento.
    /// </summary>
    /// <param name="levelObjects">Los bloques del nivel <see cref="LevelObject[]"/>.</param>
    /// <param name="items">Los items<see cref="Item[]"/>.</param>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator DestroyLevelObjectsOnBackground(LevelObject[] levelObjects, Item[] items)
    {
        foreach (Item i in items)
        {
            if (i != null)
            {
                i.gameObject.SetActive(false);
            }
        }
        foreach (LevelObject l in levelObjects)
        {
            l.gameObject.SetActive(false);
        }

        yield return null;
        foreach (LevelObject l in levelObjects)
        {
            Destroy(l.gameObject);
            yield return null;
        }
        foreach (Item i in items)
        {
            if (i != null)
            {
                Destroy(i.gameObject);
                yield return null;
            }
        }
    }

    /// <summary>
    /// Intenta coger un item si es posible.
    /// </summary>
    private void TakeItem()
    {
        Item item = items[currentLevelData.playerPos[0] + currentLevelData.playerPos[2] * currentLevelData.levelSize[0] + currentLevelData.playerPos[1] * (currentLevelData.levelSize[0] * currentLevelData.levelSize[2])];
        if (item != null)
        {
            if (!inventory.Contains(item))
            {
                Debug.Log("Item taken");
                inventory.Push(item);
                EventAggregator.Instance.Publish<MsgTakeItem>(new MsgTakeItem(item, inventory.Count));
            }
        }
    }

    /// <summary>
    /// Ejecuta un item sobre un bloque.
    /// </summary>
    /// <param name="blockLevelObject">El bloque sobre el que se va a ejecutar el item<see cref="LevelObject"/>.</param>
    /// <param name="item">El item<see cref="Item"/>.</param>
    /// <param name="x">Coordenada x del bloque<see cref="int"/>.</param>
    /// <param name="y">Coordenada y del bloque<see cref="int"/>.</param>
    /// <param name="z">Coordenada z del bloque<see cref="int"/>.</param>
    /// <param name="itemPos">La posición del item<see cref="Vector3"/>.</param>
    /// <returns>True si se ha podido ejecutar, false si no <see cref="bool"/>.</returns>
    private bool ExecuteActionEffect(LevelObject blockLevelObject, Item item, int x, int y, int z, Vector3 itemPos)
    {
        if (blockLevelObject != null && blockLevelObject.IsBlock())
        {
            Block frontBlock = (Block)blockLevelObject;

            EffectReaction[] effectReactions = frontBlock.EffectReactions;
            Effects itemEffect = item.Effect;

            if (itemEffect != Effects.None)
            {
                EffectReaction reaction = Array.Find<EffectReaction>(effectReactions, value => value.effect == itemEffect);

                if (reaction != null)
                {
                    //Miramos si el item es compatible con este objeto
                    bool compatible = false;
                    if (reaction.compatibleItems.Length <= 0)
                    {
                        compatible = true;
                    }
                    else
                    {
                        foreach (Items itemSearch in reaction.compatibleItems)
                        {
                            if (itemSearch == item.ItemType)
                            {
                                compatible = true;
                                break;
                            }
                        }
                    }

                    //Si es compatible ejecutamos las acciones necesarias
                    if (compatible)
                    {
                        if (reaction.newProperties.Length != 0)
                        {
                            frontBlock._BlockProperties = reaction.newProperties;
                        }
                        LevelObject spawnedObject = null;

                        if (reaction.replaceBlock)
                        {
                            spawnedObject = MapRenderer.Instance.SpawnBlock((int)reaction.block);
                            spawnedObject.gameObject.transform.parent = frontBlock.transform.parent;
                            spawnedObject.gameObject.transform.position = frontBlock.transform.position;
                            spawnedObject.gameObject.transform.rotation = frontBlock.transform.rotation;

                            spawnedObject.gameObject.SetActive(false);

                            objectReferences[x + z * currentLevelData.levelSize[0] + y * (currentLevelData.levelSize[0] * currentLevelData.levelSize[2])] = spawnedObject;
                        }
                        inventory.Pop();
                        EventAggregator.Instance.Publish(new MsgUseItem(frontBlock, reaction, spawnedObject, itemPos, item, inventory));

                        return true;
                    }
                }
            }
        }

        //No se ha podido usar el item
        return false;
    }

    /// <summary>
    /// Lógica del botón de acción.
    /// </summary>
    private void DoAction()
    {
        if (inventory.Count > 0)
        {
            Item item = inventory.Peek();
            if (item != null)
            {
                List<int> intendedBlock = BlockToAdvanceTo(currentLevelData.playerOrientation, currentLevelData.playerPos[0], currentLevelData.playerPos[1], currentLevelData.playerPos[2]);
                LevelObject frontBlock = GetBlock(currentLevelData, objectReferences, intendedBlock[0], intendedBlock[1], intendedBlock[2]);
                LevelObject frontBelowBlock = GetBlock(currentLevelData, objectReferences, intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2]);
                bool usedItem = false;
                if (frontBlock != null && item.UseOnFrontBlock && frontBlock.IsBlock())
                {
                    Vector3 posNew;
                    GetBlockSurfacePoint(intendedBlock[0], intendedBlock[1], intendedBlock[2], out posNew);
                    if (ExecuteActionEffect(frontBlock, item, intendedBlock[0], intendedBlock[1], intendedBlock[2], posNew))
                    {
                        usedItem = true;
                    }
                }
                if (frontBlock == null || CheckBlockProperty(frontBlock, BlockProperties.Immaterial))
                {
                    if (frontBelowBlock != null && item.UseOnFrontBelowBlock && !usedItem && frontBelowBlock.IsBlock())
                    {
                        Vector3 posNew;
                        GetBlockSurfacePoint(intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2], out posNew);
                        ExecuteActionEffect(frontBelowBlock, item, intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2], posNew);
                    }
                }
            }
        }
        CheckState();
    }

    /// <summary>
    /// Lógica del botón de condición.
    /// </summary>
    private void DoCondition()
    {
        CheckState();
    }

    /// <summary>
    /// Lógica del botón de salto.
    /// </summary>
    private void DoJump()
    {
        for (int y = currentLevelData.playerPos[1]; y >= 0; y--)
        {
            List<int> intendedBlock = BlockToAdvanceTo(currentLevelData.playerOrientation, currentLevelData.playerPos[0], y, currentLevelData.playerPos[2]);
            if (IsPositionInsideMap(intendedBlock, currentLevelData))
            {
                if (!CheckBlockProperty(intendedBlock[0], intendedBlock[1], intendedBlock[2], currentLevelData, BlockProperties.Immaterial))
                {
                    Vector3 target;
                    if (GetBlockSurfacePoint(intendedBlock[0], intendedBlock[1], intendedBlock[2], out target))
                    {
                        EventAggregator.Instance.Publish(new MsgBigRobotAction(MsgBigRobotAction.BigRobotActions.Jump, target));

                        currentLevelData.playerPos[0] = intendedBlock[0];
                        //Al saltar queda por encima del bloque
                        currentLevelData.playerPos[1] = intendedBlock[1] + 1;
                        currentLevelData.playerPos[2] = intendedBlock[2];

                        break;
                    }
                }
            }
        }

        TakeItem();
        CheckState();
    }

    /// <summary>
    /// Lógica del botón de bucle.
    /// </summary>
    private void DoLoop()
    {
        CheckState();
    }

    /// <summary>
    /// Lógica del botón avanzar.
    /// </summary>
    private void DoMove()
    {
        List<int> intendedBlock = BlockToAdvanceTo(currentLevelData.playerOrientation, currentLevelData.playerPos[0], currentLevelData.playerPos[1], currentLevelData.playerPos[2]);

        if (IsPositionInsideMap(intendedBlock, currentLevelData))
        {
            if (CheckBlockProperty(intendedBlock[0], intendedBlock[1], intendedBlock[2], currentLevelData, BlockProperties.Immaterial))
            {
                //Miramos el bloque de debajo
                if (!CheckBlockProperty(intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2], currentLevelData, BlockProperties.Immaterial))
                {
                    Vector3 target;
                    if (GetBlockSurfacePoint(intendedBlock[0], intendedBlock[1] - 1, intendedBlock[2], out target))
                    {
                        Debug.Log(target);
                        EventAggregator.Instance.Publish(new MsgBigRobotAction(MsgBigRobotAction.BigRobotActions.Move, target));

                        currentLevelData.playerPos = intendedBlock;
                    }
                }
                else
                {
                    DoJump();
                }
            }
            else
            {
                Debug.Log("You are colliding against a block");
            }
        }
        else
        {
            DoJump();
        }
        TakeItem();
        CheckState();
    }

    /// <summary>
    /// Comprueba si el bloque de enfrente y abajo cumple una propiedad.
    /// </summary>
    /// <param name="property">La propiedad<see cref="BlockProperties"/>.</param>
    /// <returns>True si la cumple, false si no <see cref="bool"/>.</returns>
    public bool CheckNextBlockDownProperty(BlockProperties property)
    {
        //-1 a la y para mirar el que va a pisar el robot
        List<int> nextBlock = BlockToAdvanceTo(currentLevelData.playerOrientation, currentLevelData.playerPos[0], currentLevelData.playerPos[1], currentLevelData.playerPos[2]);

        return CheckBlockProperty(nextBlock[0], nextBlock[1] - 1, nextBlock[2], currentLevelData, property);
    }

    /// <summary>
    /// Dado un bloque comprueba si cumple una propiedad.
    /// </summary>
    /// <param name="blockLevelObject">El bloque <see cref="LevelObject"/>.</param>
    /// <param name="property">La propiedad<see cref="BlockProperties"/>.</param>
    /// <returns>True si la cumple, false si no <see cref="bool"/>.</returns>
    private bool CheckBlockProperty(LevelObject blockLevelObject, BlockProperties property)
    {
        if (blockLevelObject != null && blockLevelObject.IsBlock())
        {
            Block block = (Block)blockLevelObject;
            return block.CheckProperty(property);
        }
        return false;
    }

    /// <summary>
    /// Dado un bloque comprueba si cumple una propiedad.
    /// </summary>
    /// <param name="x">Coordenada x<see cref="int"/>.</param>
    /// <param name="y">Coordenada y<see cref="int"/>.</param>
    /// <param name="z">Coordenada z<see cref="int"/>.</param>
    /// <param name="data">La estructura del nivel <see cref="LevelData"/>.</param>
    /// <param name="property">La propiedad <see cref="BlockProperties"/>.</param>
    /// <returns>True si la cumple, false si no <see cref="bool"/>.</returns>
    private bool CheckBlockProperty(int x, int y, int z, LevelData data, BlockProperties property)
    {
        LevelObject blockLevelObject = GetBlock(data, objectReferences, x, y, z);
        if (blockLevelObject != null && blockLevelObject.IsBlock())
        {
            Block block = (Block)blockLevelObject;
            return block.CheckProperty(property);
        }
        return false;
    }

    /// <summary>
    /// Retorna el tipo de un bloque.
    /// </summary>
    /// <param name="data">La estructura del nivel <see cref="LevelData"/>.</param>
    /// <param name="x">Coordenada x<see cref="int"/>.</param>
    /// <param name="y">Coordenada y<see cref="int"/>.</param>
    /// <param name="z">Coordenada z<see cref="int"/>.</param>
    /// <returns>El tipo del bloque <see cref="int"/>.</returns>
    private int GetBlockType(LevelData data, int x, int y, int z)
    {
        if (x < 0 || x >= data.levelSize[0]) return (int)Blocks.NoBlock;
        if (y < 0 || y >= data.levelSize[1]) return (int)Blocks.NoBlock;
        if (z < 0 || z >= data.levelSize[2]) return (int)Blocks.NoBlock;
        return data.mapAndItems[x + z * data.levelSize[0] + y * (data.levelSize[0] * data.levelSize[2])];
    }

    /// <summary>
    /// Retorna un bloque.
    /// </summary>
    /// <param name="data">La estructura del nivel <see cref="LevelData"/>.</param>
    /// <param name="objects">Array con los bloques <see cref="LevelObject[]"/>.</param>
    /// <param name="x">Coordenada x<see cref="int"/>.</param>
    /// <param name="y">Coordenada y<see cref="int"/>.</param>
    /// <param name="z">Coordenada z<see cref="int"/>.</param>
    /// <returns>El bloque encontrado o null si no <see cref="LevelObject"/>.</returns>
    private LevelObject GetBlock(LevelData data, LevelObject[] objects, int x, int y, int z)
    {
        if (x < 0 || x >= data.levelSize[0]) return null;
        if (y < 0 || y >= data.levelSize[1]) return null;
        if (z < 0 || z >= data.levelSize[2]) return null;
        return objects[x + z * data.levelSize[0] + y * (data.levelSize[0] * data.levelSize[2])];
    }

    /// <summary>
    /// Pone un bloque en la estructura de objetos.
    /// </summary>
    /// <param name="data">La estructura del nivel <see cref="LevelData"/>.</param>
    /// <param name="objects">Array con los bloques <see cref="LevelObject[]"/>.</param>
    /// <param name="x">Coordenada x<see cref="int"/>.</param>
    /// <param name="y">Coordenada y<see cref="int"/>.</param>
    /// <param name="z">Coordenada z<see cref="int"/>.</param>
    /// <param name="levelObject">El objeto a colocar <see cref="LevelObject"/>.</param>
    private void SetBlock(LevelData data, LevelObject[] objects, int x, int y, int z, LevelObject levelObject)
    {
        if (x < 0 || x >= data.levelSize[0]) return;
        if (y < 0 || y >= data.levelSize[1]) return;
        if (z < 0 || z >= data.levelSize[2]) return;
        objects[x + z * data.levelSize[0] + y * (data.levelSize[0] * data.levelSize[2])] = levelObject;
    }

    /// <summary>
    /// Modifica el tipo de un bloque.
    /// </summary>
    /// <param name="data">La estructura del nivel <see cref="LevelData"/>.</param>
    /// <param name="value">El nuevo tipo del bloque <see cref="int"/>.</param>
    /// <param name="x">Coordenada x<see cref="int"/>.</param>
    /// <param name="y">Coordenada y<see cref="int"/>.</param>
    /// <param name="z">Coordenada z<see cref="int"/>.</param>
    private void SetBlockType(LevelData data, int value, int x, int y, int z)
    {
        if (x < 0 || x >= data.levelSize[0]) return;
        if (y < 0 || y >= data.levelSize[1]) return;
        if (z < 0 || z >= data.levelSize[2]) return;
        data.mapAndItems[x + z * data.levelSize[0] + y * (data.levelSize[0] * data.levelSize[2])] = (int)value;
    }

    /// <summary>
    /// Determina si una posición está dentro del mapa.
    /// </summary>
    /// <param name="posToCheck">La posición a comprobar tal que [x,y,x] <see cref="List{int}"/>.</param>
    /// <param name="data">La estructura del nivel <see cref="LevelData"/>.</param>
    /// <returns>True si está dentro, false si no <see cref="bool"/>.</returns>
    private bool IsPositionInsideMap(List<int> posToCheck, LevelData data)
    {
        if (posToCheck[0] < 0)
        {
            return false;
        }

        if (posToCheck[1] < 0)
        {
            return false;
        }

        if (posToCheck[2] < 0)
        {
            return false;
        }

        if (posToCheck[0] >= data.levelSize[0])
        {
            return false;
        }

        if (posToCheck[1] >= data.levelSize[1])
        {
            return false;
        }

        if (posToCheck[2] >= data.levelSize[2])
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Retorna las coordenadas del bloque al que avanza el jugador.
    /// </summary>
    /// <param name="playerOrientation">La orientación del jugador (0, 1, 2, 3)<see cref="int"/>.</param>
    /// <param name="x">Coordenada x<see cref="int"/>.</param>
    /// <param name="y">Coordenada y<see cref="int"/>.</param>
    /// <param name="z">Coordenada z<see cref="int"/>.</param>
    /// <returns>[x,y,z] <see cref="List{int}"/>.</returns>
    private List<int> BlockToAdvanceTo(int playerOrientation, int x, int y, int z)
    {
        List<int> output = new List<int>();
        output.Add(x);
        output.Add(y);
        output.Add(z);

        //0 - z+
        //1 - x+
        //2 - z-
        //3 - x-
        //90f * data.playerOrientation
        switch (playerOrientation)
        {
            case 0:
                output[2]++;

                break;

            case 1:
                output[0]++;

                break;

            case 2:
                output[2]--;

                break;

            case 3:
                output[0]--;

                break;

            default:
                Debug.LogError("Unknown orientation");
                break;
        }

        return output;
    }

    /// <summary>
    /// Lógica del botón play.
    /// </summary>
    private void DoPlay()
    {
        //Nothing
    }

    /// <summary>
    /// Lógica del botón restart.
    /// </summary>
    private void DoRestart()
    {
        EventAggregator.Instance.Publish<MsgHideAllScreens>(new MsgHideAllScreens());
        currentLevelData = clonedLevelData.Clone();
        finishedMinibotMovement = false;
        bigCharater.SetActive(false);
        if (objectReferences != null)
        {
            StartCoroutine(DestroyLevelObjectsOnBackground((LevelObject[])objectReferences.Clone(), (Item[])items.Clone()));
            objectReferences = null;
            items = null;
        }

        StartCoroutine(RenderALevel());
    }

    /// <summary>
    /// Lógica del botón de girar a la izquierda.
    /// </summary>
    private void DoTurnLeft()
    {
        currentLevelData.playerOrientation--;
        if (currentLevelData.playerOrientation < 0)
        {
            currentLevelData.playerOrientation = 3;
        }
        EventAggregator.Instance.Publish(new MsgBigRobotAction(MsgBigRobotAction.BigRobotActions.TurnLeft, new Vector3(0, 0, 0)));
    }

    /// <summary>
    /// Lógica del botón de girar a la derecha.
    /// </summary>
    private void DoTurnRight()
    {
        currentLevelData.playerOrientation++;
        if (currentLevelData.playerOrientation > 3)
        {
            currentLevelData.playerOrientation = 0;
        }
        EventAggregator.Instance.Publish(new MsgBigRobotAction(MsgBigRobotAction.BigRobotActions.TurnRight, new Vector3(0, 0, 0)));
    }
}