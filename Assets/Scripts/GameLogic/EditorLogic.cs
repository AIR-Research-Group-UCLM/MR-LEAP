// EditorLogic.cs
// Furious Koalas S.L.
// 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelObject;

/// <summary>
/// Clase que contiene toda la lógica del editor de niveles <see cref="EditorLogic" />.
/// </summary>
public class EditorLogic : MonoBehaviour
{
    /// <summary>
    /// Tamaño máximo del mapa en el eje X.
    /// </summary>
    [SerializeField] private int mapSizeX = 8;

    /// <summary>
    /// Tamaño máximo del mapa en el eje Y.
    /// </summary>
    [SerializeField] private int mapSizeY = 4;

    /// <summary>
    /// Tamaño máximo del mapa en el eje Z.
    /// </summary>
    [SerializeField] private int mapSizeZ = 8;

    /// <summary>
    /// El objeto que contiene al escenario.
    /// </summary>
    [SerializeField] private GameObject placeableMap;

    /// <summary>
    /// Segundos de duración de las pantallas de aviso.
    /// </summary>
    [SerializeField] private float secondsWarningScreen = 3;

    /// <summary>
    /// Referencias a los objetos del nivel.
    /// </summary>
    private EditorObject[] editorObjects;

    /// <summary>
    /// La herramienta seleccionada.
    /// </summary>
    private SelectedTool selectedTool;

    /// <summary>
    /// Estructura de datos para guardar el numero de instrucciones disponibles.
    /// </summary>
    private AvailableInstructions availableInstructions;

    /// <summary>
    /// ¿Tiene bandera?
    /// </summary>
    private bool hasFlag = false;

    /// <summary>
    /// ¿Tiene jugador?.
    /// </summary>
    private bool hasPlayer = false;

    /// <summary>
    /// Enum con la clase de herramientas que podemos usar.
    /// </summary>
    public enum EditorToolType
    {
        Item,
        Block,
        Player,
        Eraser
    }

    private string currentLevelName;

    /// <summary>
    /// Instancia estática de la clase.
    /// </summary>
    private static EditorLogic editorLogic;

    /// <summary>
    /// Devuelve la instancia de la clase.
    /// </summary>
    public static EditorLogic Instance
    {
        get
        {
            if (!editorLogic)
            {
                editorLogic = FindObjectOfType(typeof(EditorLogic)) as EditorLogic;

                if (!editorLogic)
                {
                    Debug.LogError("There needs to be one active EditorLogic script on a GameObject in your scene.");
                }
            }

            return editorLogic;
        }
    }

    /// <summary>
    /// Awake.
    /// </summary>
    private void Awake()
    {
        EventAggregator.Instance.Subscribe<MsgEditorMapSize>(ServeMapSize);
        EventAggregator.Instance.Subscribe<MsgEditorSurfaceTapped>(EditorSurfaceTapped);
        EventAggregator.Instance.Subscribe<MsgEditorToolSelected>(ChangeSelectedTool);
        EventAggregator.Instance.Subscribe<MsgEditorAvailableInstructionsChanged>(ChangeAvailableInstructions);
        EventAggregator.Instance.Subscribe<MsgEditorSaveMap>(SaveMap);
        EventAggregator.Instance.Subscribe<MsgEditorMenu>(ReturnToMainMenu);
    }

    /// <summary>
    /// Método que recibe el mensaje para volver al menú principal.
    /// </summary>
    /// <param name="msg">El mensaje<see cref="MsgEditorMenu"/>.</param>
    private void ReturnToMainMenu(MsgEditorMenu msg)
    {
        EventAggregator.Instance.Publish<MsgHideAllScreens>(new MsgHideAllScreens());
        ResetEditor();
        MainMenuLogic.Instance.ShowMainMenu();
    }

    /// <summary>
    /// Método para mostrar el editor.
    /// </summary>
    public void ShowEditor()
    {
        placeableMap.GetComponent<MapController>().EnableEditorControls();
        ResetEditor();
    }

    /// <summary>
    /// Recibe los mensajes para actualizar el número de instrucciones disponibles.
    /// </summary>
    /// <param name="msg">El mensaje<see cref="MsgEditorAvailableInstructionsChanged"/>.</param>
    private void ChangeAvailableInstructions(MsgEditorAvailableInstructionsChanged msg)
    {
        if (availableInstructions != null)
        {
            switch (msg.Button)
            {
                case LevelButtons.Buttons.Action:
                    availableInstructions.action = msg.NumberOfInstructions;
                    break;

                case LevelButtons.Buttons.Condition:
                    availableInstructions.condition = msg.NumberOfInstructions;
                    break;

                case LevelButtons.Buttons.Jump:
                    availableInstructions.jump = msg.NumberOfInstructions;
                    break;

                case LevelButtons.Buttons.Loop:
                    availableInstructions.loop = msg.NumberOfInstructions;

                    break;

                case LevelButtons.Buttons.Move:
                    availableInstructions.move = msg.NumberOfInstructions;
                    break;

                case LevelButtons.Buttons.TurnLeft:
                    availableInstructions.turnLeft = msg.NumberOfInstructions;
                    break;

                case LevelButtons.Buttons.TurnRight:
                    availableInstructions.turnRight = msg.NumberOfInstructions;
                    break;
            }
        }
    }

    /// <summary>
    /// Recibe el mensaje para cambiar de herramienta seleccionada.
    /// </summary>
    /// <param name="msg">El mensaje<see cref="MsgEditorToolSelected"/>.</param>
    private void ChangeSelectedTool(MsgEditorToolSelected msg)
    {
        if (selectedTool == null)
        {
            selectedTool = new SelectedTool(msg.ToolType, msg.ToolIdentifier);
        }
        else
        {
            selectedTool.ToolType = msg.ToolType;
            selectedTool.ToolIdentifier = msg.ToolIdentifier;
        }
    }

    /// <summary>
    /// Start.
    /// </summary>
    private void Start()
    {
        ResetEditor();
    }

    /// <summary>
    /// Recibe el mensaje para guardar el mapa.
    /// </summary>
    /// <param name="msg">El mensaje<see cref="MsgEditorSaveMap"/>.</param>
    private void SaveMap(MsgEditorSaveMap msg)
    {
        if (!hasPlayer)
        {
            EventAggregator.Instance.Publish<MsgShowScreen>(new MsgShowScreen("error_no_robot", secondsWarningScreen));
            return;
        }

        if (!hasFlag)
        {
            EventAggregator.Instance.Publish<MsgShowScreen>(new MsgShowScreen("error_no_flag", secondsWarningScreen));
            return;
        }
        LevelData newLevel = GenerateLevel();
        if (newLevel != null)
        {
            MapMenuLogic.Instance.AddNewLevel(newLevel);
        }
    }

    /// <summary>
    /// Genera un nivel en formato LevelData si es posible.
    /// </summary>
    /// <returns>El nivel generado si ha tenido éxito <see cref="LevelData"/>.</returns>
    private LevelData GenerateLevel()
    {
        if (hasPlayer && hasFlag && availableInstructions != null)
        {
            LevelData newLevel = new LevelData();

            newLevel.levelName = currentLevelName;
            int[] firstBlockPosition = GetMinXYZ();
            int[] lastBlockPosition = GetMaxXYZ();

            //Si en la ultima capa hay un bloque y hay alguna instrucción de salto
            //ponemos una capa mas por si el jugador se pone encima

            if (availableInstructions.jump > 0)
            {
                bool CheckLastLayerForBlocks(int y)
                {
                    EditorObject editorObject;
                    for (int z = firstBlockPosition[2]; z <= lastBlockPosition[2]; z++)
                    {
                        for (int x = firstBlockPosition[0]; x <= lastBlockPosition[0]; x++)
                        {
                            editorObject = GetEditorObject(x, y, z);
                            if (editorObject != null)
                            {
                                if (editorObject.ObjectType == EditorToolType.Block)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    return false;
                }

                if (CheckLastLayerForBlocks(lastBlockPosition[1]))
                {
                    lastBlockPosition[1]++;
                }
            }

            newLevel.levelSize = new List<int>();
            newLevel.levelSize.Add((lastBlockPosition[0] - firstBlockPosition[0]) + 1);
            newLevel.levelSize.Add((lastBlockPosition[1] - firstBlockPosition[1]) + 1);
            newLevel.levelSize.Add((lastBlockPosition[2] - firstBlockPosition[2]) + 1);
            newLevel.availableInstructions = availableInstructions;
            newLevel.mapAndItems = new List<int>();

            EditorObject obj;
            for (int y = firstBlockPosition[1]; y <= lastBlockPosition[1]; y++)
            {
                for (int z = firstBlockPosition[2]; z <= lastBlockPosition[2]; z++)
                {
                    for (int x = firstBlockPosition[0]; x <= lastBlockPosition[0]; x++)
                    {
                        obj = GetEditorObject(x, y, z);
                        if (obj != null)
                        {
                            if (obj.ObjectType == EditorToolType.Block)
                            {
                                newLevel.mapAndItems.Add(obj.ObjectIdentifier);
                            }
                            else if (obj.ObjectType == EditorToolType.Item)
                            {
                                if (obj.ObjectIdentifier == (int)Items.FlagItem)
                                {
                                    newLevel.goal = new List<int>();

                                    newLevel.goal.Add(x - firstBlockPosition[0]);
                                    newLevel.goal.Add(y - firstBlockPosition[1]);
                                    newLevel.goal.Add(z - firstBlockPosition[2]);
                                    newLevel.mapAndItems.Add((int)Blocks.NoBlock);
                                }
                                else
                                {
                                    newLevel.mapAndItems.Add(obj.ObjectIdentifier);
                                }
                            }
                            else if (obj.ObjectType == EditorToolType.Player)
                            {
                                newLevel.playerOrientation = obj.ObjectIdentifier;
                                newLevel.playerPos = new List<int>();

                                newLevel.playerPos.Add(x - firstBlockPosition[0]);
                                newLevel.playerPos.Add(y - firstBlockPosition[1]);
                                newLevel.playerPos.Add(z - firstBlockPosition[2]);
                                newLevel.mapAndItems.Add((int)Blocks.NoBlock);
                            }
                        }
                        else
                        {
                            newLevel.mapAndItems.Add((int)Blocks.NoBlock);
                        }
                    }
                }
            }

            return newLevel;
        }
        return null;
    }

    /// <summary>
    /// Retorna la coordenada X,Y,Z mínimas del nivel que se está generando.
    /// </summary>
    /// <returns>[X,Y,Z] <see cref="int[]"/>.</returns>
    private int[] GetMinXYZ()
    {
        int minX = mapSizeX - 1;
        int minY = mapSizeY - 1;
        int minZ = mapSizeZ - 1;

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                for (int z = 0; z < mapSizeZ; z++)
                {
                    if (GetEditorObject(x, y, z) != null)
                    {
                        if (x < minX)
                        {
                            minX = x;
                        }
                        if (y < minY)
                        {
                            minY = y;
                        }
                        if (z < minZ)
                        {
                            minZ = z;
                        }
                    }
                }
            }
        }
        return new int[] { minX, minY, minZ };
    }

    /// <summary>
    /// Retorna la coordenada X,Y,Z máximas del nivel que se está generando.
    /// </summary>
    /// <returns>[X,Y,Z] <see cref="int[]"/>.</returns>
    private int[] GetMaxXYZ()
    {
        int maxX = 0;
        int maxY = 0;
        int maxZ = 0;

        for (int x = mapSizeX - 1; x >= 0; x--)
        {
            for (int y = mapSizeY - 1; y >= 0; y--)
            {
                for (int z = mapSizeZ - 1; z >= 0; z--)
                {
                    if (GetEditorObject(x, y, z) != null)
                    {
                        if (x > maxX)
                        {
                            maxX = x;
                        }
                        if (y > maxY)
                        {
                            maxY = y;
                        }
                        if (z > maxZ)
                        {
                            maxZ = z;
                        }
                    }
                }
            }
        }
        return new int[] { maxX, maxY, maxZ };
    }

    /// <summary>
    /// Devuelve el editor a su estado original.
    /// </summary>
    private void ResetEditor()
    {
        currentLevelName = System.Guid.NewGuid().ToString();
        EventAggregator.Instance.Publish<MsgEditorResetAllCounters>(new MsgEditorResetAllCounters());
        EventAggregator.Instance.Publish<MsgResetEditorSurface>(new MsgResetEditorSurface());
        selectedTool = null;
        hasFlag = false;
        hasPlayer = false;
        if (editorObjects != null)
        {
            StartCoroutine(DestroyOldObjectsInBackgroundCrt((EditorObject[])editorObjects.Clone()));
        }
        editorObjects = new EditorObject[mapSizeX * mapSizeY * mapSizeZ];
        availableInstructions = new AvailableInstructions();
    }

    /// <summary>
    /// Responde a los mensajes requiriendo el tamaño del nivel.
    /// </summary>
    /// <param name="msg">El mensaje<see cref="MsgEditorMapSize"/>.</param>
    private void ServeMapSize(MsgEditorMapSize msg)
    {
        List<int> mapSize = new List<int>();
        mapSize.Add(mapSizeX);
        mapSize.Add(mapSizeY);
        mapSize.Add(mapSizeZ);
        EventAggregator.Instance.Publish(new ResponseWrapper<MsgEditorMapSize, List<int>>(msg, mapSize));
    }

    /// <summary>
    /// Recibe el mensaje de que se ha pulsado en la superficie del editor.
    /// </summary>
    /// <param name="msg">El mensaje<see cref="MsgEditorSurfaceTapped"/>.</param>
    private void EditorSurfaceTapped(MsgEditorSurfaceTapped msg)
    {
        if (selectedTool != null)
        {
            int x = msg.TappedPoint.SurfacePointPositionX;
            int z = msg.TappedPoint.SurfacePointPositionZ;

            switch (selectedTool.ToolType)
            {
                case EditorToolType.Item:
                    if (PlaceBlockOrItem(false, x, z, msg.TappedPoint))
                    {
                        msg.TappedPoint.Up();
                    }
                    break;

                case EditorToolType.Block:
                    if (PlaceBlockOrItem(true, x, z, msg.TappedPoint))
                    {
                        msg.TappedPoint.Up();
                    }
                    break;

                case EditorToolType.Player:
                    if (PlaceCharacter(x, z, msg.TappedPoint))
                    {
                        msg.TappedPoint.Up();
                    }
                    break;

                case EditorToolType.Eraser:
                    if (Eraser(x, z))
                    {
                        msg.TappedPoint.Down();
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Coloca al robot en el sitio apropiado con la orientación requerida.
    /// </summary>
    /// <param name="x">La coordenada x<see cref="int"/>.</param>
    /// <param name="z">La coordenada z<see cref="int"/>.</param>
    /// <param name="TappedPoint">El punto donde se ha hecho tap<see cref="EditorSurfacePoint"/>.</param>
    /// <returns>True si se ha podido colocar, false si no <see cref="bool"/>.</returns>
    private bool PlaceCharacter(int x, int z, EditorSurfacePoint TappedPoint)
    {
        if (!hasPlayer)
        {
            int y = 0;
            bool insideMap = false;
            for (; y < mapSizeY; y++)
            {
                if (GetEditorObject(x, y, z) == null)
                {
                    insideMap = true;
                    break;
                }
            }

            if (insideMap)
            {
                bool validPos = false;
                EditorObject obj = GetEditorObject(x, y - 1, z);
                //Solo se puede poner un jugador encima de un bloque
                if (obj.ObjectType == EditorToolType.Block)
                {
                    validPos = true;
                }

                if (validPos)
                {
                    GameObject spawned = MapRenderer.Instance.RenderMainCharacter();
                    EditorObject newEditorObject = new EditorObject(spawned, selectedTool.ToolType, selectedTool.ToolIdentifier);
                    SetEditorObject(x, y, z, newEditorObject);
                    spawned.SetActive(true);
                    spawned.transform.parent = TappedPoint.EditorSurface;
                    spawned.transform.rotation = TappedPoint.transform.rotation * spawned.transform.rotation;
                    spawned.transform.Rotate(new Vector3(0, 90f * selectedTool.ToolIdentifier, 0));
                    spawned.transform.position = new Vector3(TappedPoint.transform.position.x,
                        (TappedPoint.EditorSurface.position.y + TappedPoint.BlockLength * y) - TappedPoint.BlockLength / 2,
                        TappedPoint.transform.position.z);

                    hasPlayer = true;
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Coloca un bloque o un item donde se requiera.
    /// </summary>
    /// <param name="isBlock">¿Es un bloque?<see cref="bool"/>.</param>
    /// <param name="x">Coordenada x<see cref="int"/>.</param>
    /// <param name="z">Coordenada z<see cref="int"/>.</param>
    /// <param name="TappedPoint">El punto donde se ha hecho tap<see cref="EditorSurfacePoint"/>.</param>
    /// <returns>True si se ha podido colocar, false si no <see cref="bool"/>.</returns>
    private bool PlaceBlockOrItem(bool isBlock, int x, int z, EditorSurfacePoint TappedPoint)
    {
        int y = 0;
        bool insideMap = false;
        for (; y < mapSizeY; y++)
        {
            if (GetEditorObject(x, y, z) == null)
            {
                insideMap = true;
                break;
            }
        }

        if (insideMap)
        {
            bool validPos = false;
            EditorObject obj = GetEditorObject(x, y - 1, z);
            //No se puede poner un bloque encima de un jugador o un item
            if (isBlock)
            {
                if (obj == null || obj.ObjectType == EditorToolType.Block)
                {
                    validPos = true;
                }
            }
            else
            {
                //Solo se puede poner items sobre bloques
                if (obj != null && obj.ObjectType == EditorToolType.Block)
                {
                    validPos = true;
                }
            }

            if (validPos)
            {
                //Comprobamos que no se pueda poner mas de una bandera
                if (hasFlag && selectedTool.ToolType == EditorToolType.Item)
                {
                    if (selectedTool.ToolIdentifier == (int)Items.FlagItem)
                    {
                        return false;
                    }
                }
                GameObject spawned = MapRenderer.Instance.SpawnBlock(selectedTool.ToolIdentifier).gameObject;
                spawned.SetActive(true);
                EditorObject newEditorObject = new EditorObject(spawned, selectedTool.ToolType, selectedTool.ToolIdentifier);
                SetEditorObject(x, y, z, newEditorObject);

                spawned.transform.parent = TappedPoint.EditorSurface;
                spawned.transform.rotation = TappedPoint.transform.rotation * spawned.transform.rotation;
                spawned.transform.position = new Vector3(TappedPoint.transform.position.x,
                    TappedPoint.EditorSurface.position.y + TappedPoint.BlockLength * y,
                    TappedPoint.transform.position.z);

                if (newEditorObject.ObjectType == EditorToolType.Item)
                {
                    if (newEditorObject.ObjectIdentifier == (int)Items.FlagItem)
                    {
                        hasFlag = true;
                    }
                }
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Borra el objeto más alto de la coordenada x, z.
    /// </summary>
    /// <param name="x">Coordenada x<see cref="int"/>.</param>
    /// <param name="z">Coordenada z<see cref="int"/>.</param>
    /// <returns>True si lo ha podido borrar, false si no <see cref="bool"/>.</returns>
    private bool Eraser(int x, int z)
    {
        int y = 0;

        for (; y < mapSizeY; y++)
        {
            if (GetEditorObject(x, y, z) == null)
            {
                break;
            }
        }

        y--;

        EditorObject objectToErase = GetEditorObject(x, y, z);
        if (objectToErase != null)
        {
            if (objectToErase.AssociatedGameobject != null)
            {
                Destroy(objectToErase.AssociatedGameobject);
            }
            if (objectToErase.ObjectType == EditorToolType.Player)
            {
                hasPlayer = false;
            }
            if (objectToErase.ObjectType == EditorToolType.Item)
            {
                if (objectToErase.ObjectIdentifier == (int)Items.FlagItem)
                {
                    hasFlag = false;
                }
            }
            SetEditorObject(x, y, z, null);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Retorna un EditorObject dadas las coordenadas X,Y,Z.
    /// </summary>
    /// <param name="x">Coordenada x<see cref="int"/>.</param>
    /// <param name="y">Coordenada y<see cref="int"/>.</param>
    /// <param name="z">Coordenada z<see cref="int"/>.</param>
    /// <returns>El objeto de esa posición o null si no hay <see cref="EditorObject"/>.</returns>
    private EditorObject GetEditorObject(in int x, in int y, in int z)
    {
        if (x < 0 || x >= mapSizeX) return null;
        if (y < 0 || y >= mapSizeY) return null;
        if (z < 0 || z >= mapSizeZ) return null;
        return editorObjects[x + z * mapSizeX + y * (mapSizeX * mapSizeZ)];
    }

    /// <summary>
    /// Pone un EditorObject en la coordenada X,Y,Z apropiada.
    /// </summary>
    /// <param name="x">Coordenada x<see cref="int"/>.</param>
    /// <param name="y">Coordenada y<see cref="int"/>.</param>
    /// <param name="z">Coordenada z<see cref="int"/>.</param>
    /// <param name="editorObject">El objeto a colocar<see cref="EditorObject"/>.</param>
    /// <returns>True si se ha podido colocar, false si no <see cref="bool"/>.</returns>
    private bool SetEditorObject(in int x, in int y, in int z, in EditorObject editorObject)
    {
        if (x < 0 || x >= mapSizeX) return false;
        if (y < 0 || y >= mapSizeY) return false;
        if (z < 0 || z >= mapSizeZ) return false;
        editorObjects[x + z * mapSizeX + y * (mapSizeX * mapSizeZ)] = editorObject;
        return true;
    }

    /// <summary>
    /// Elimina los objetos de forma que no produce bajadas de rendimiento.
    /// </summary>
    /// <param name="editorObjects">Los objetos a borrar<see cref="EditorObject[]"/>.</param>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator DestroyOldObjectsInBackgroundCrt(EditorObject[] editorObjects)
    {
        if (editorObjects != null)
        {
            for (int i = 0; i < editorObjects.Length; i++)
            {
                if (editorObjects[i] != null)
                {
                    if (editorObjects[i].AssociatedGameobject != null)
                    {
                        editorObjects[i].AssociatedGameobject.SetActive(false);
                    }
                }
            }

            yield return null;

            for (int i = 0; i < editorObjects.Length; i++)
            {
                if (editorObjects[i] != null)
                {
                    if (editorObjects[i].AssociatedGameobject != null)
                    {
                        Destroy(editorObjects[i].AssociatedGameobject);
                        yield return null;
                    }
                    editorObjects[i] = null;
                }
            }

            editorObjects = null;
        }
    }

    /// <summary>
    /// Clase privada para mantener la herramienta seleccionada <see cref="SelectedTool" />.
    /// </summary>
    private class SelectedTool
    {
        /// <summary>
        /// Tipo de herramienta.
        /// </summary>
        private EditorToolType toolType;

        /// <summary>
        /// Parámetro opcional de la herramienta.
        /// </summary>
        private int toolIdentifier;

        /// <summary>
        /// Retorna o cambia el tipo de la herramienta.
        /// </summary>
        public EditorToolType ToolType { get => toolType; set => toolType = value; }

        /// <summary>
        /// Retorna o cambia el parámetro opcional.
        /// </summary>
        public int ToolIdentifier { get => toolIdentifier; set => toolIdentifier = value; }

        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="SelectedTool"/>.
        /// </summary>
        /// <param name="toolType">El toolType<see cref="EditorToolType"/>.</param>
        /// <param name="toolIdentifier">El toolIdentifier<see cref="int"/>.</param>
        public SelectedTool(EditorToolType toolType, int toolIdentifier)
        {
            this.toolType = toolType;
            this.toolIdentifier = toolIdentifier;
        }
    }
}