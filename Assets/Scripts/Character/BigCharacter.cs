// BigCharacter.cs
// Furious Koalas S.L.
// 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Block;
using static LevelObject;

/// <summary>
/// La clase <see cref="BigCharacter" /> contiene .
/// </summary>
public class BigCharacter : Character
{
    /// <summary>
    /// Posición del primer item del inventario del robot.
    /// </summary>
    [SerializeField] private GameObject inventoryMarker;

    /// <summary>
    /// Velocidad a la que van las acciones.
    /// </summary>
    public float actionSpeed = 0.5f;

    /// <summary>
    /// Porcentaje de la altura del salto hacia arriba respecto a la del bloque.
    /// </summary>
    [Range(0.0f, 10.0f)]
    public float jumpPct = 1f;

    /// <summary>
    /// Altura del salto.
    /// </summary>
    private float jumpHeight;

    /// <summary>
    /// Porcentaje de la altura del salto hacia abajo respecto a la altura total del salto.
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float descendJumpPct = 0.3f;

    /// <summary>
    /// Porcentaje del tiempo de salto que se pasa ascendiendo.
    /// </summary>
    [Range(0, 1)]
    public float takeOff = 0.8f;

    /// <summary>
    /// Audio de perder.
    /// </summary>
    [SerializeField] private AudioClip loseSfx;

    /// <summary>
    /// Tiempo que dura la rotación.
    /// </summary>
    public float rotationTime = 1f;

    /// <summary>
    /// Capacidad inicial de la lista de acciones pendientes.
    /// </summary>
    public int initialActionCapacity = 20;

    /// <summary>
    /// Lista de acciones.
    /// </summary>
    private List<IEnumerator> actionList;

    /// <summary>
    /// False si se esta ejecutando una acción,
    /// true si no.
    /// </summary>
    private bool lastActionFinished;

    /// <summary>
    /// Posición local por defecto.
    /// </summary>
    private Vector3 defaultLocalPosition = new Vector3();

    /// <summary>
    /// Rotación local por defecto.
    /// </summary>
    private Quaternion defaultLocalRotation = new Quaternion();

    /// <summary>
    /// MessageWarehouse a usar por el robot.
    /// </summary>
    private MessageWarehouse msgWar;

    /// <summary>
    /// ¿Se ha cargado todo lo necesario?.
    /// </summary>
    private bool loaded = false;

    /// <summary>
    /// Longitud de los bloques.
    /// </summary>
    private float blockLength;

    /// <summary>
    /// Awake.
    /// </summary>
    private void Awake()
    {
        EventAggregator.Instance.Subscribe<MsgBigRobotAction>(ReceiveAction);
        EventAggregator.Instance.Subscribe<MsgPlaceCharacter>(PlaceCharacter);
        EventAggregator.Instance.Subscribe<MsgTakeItem>(TakeItem);
        EventAggregator.Instance.Subscribe<MsgBigRobotIdle>(IsRobotIdle);
        EventAggregator.Instance.Subscribe<MsgUseItem>(UseItem);
        EventAggregator.Instance.Subscribe<MsgBigCharacterAllActionsFinished>(ServeAllActionsFinished);

        msgWar = new MessageWarehouse(EventAggregator.Instance);
    }

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        StartCoroutine(StartCrt());
    }

    /// <summary>
    /// Inicialización de variables.
    /// </summary>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator StartCrt()
    {
        loaded = false;
        actionList = new List<IEnumerator>(20);
        lastActionFinished = true;

        MsgBlockLength msg = new MsgBlockLength();
        msgWar.PublishMsgAndWaitForResponse<MsgBlockLength, float>(msg);
        yield return new WaitUntil(() => msgWar.IsResponseReceived<MsgBlockLength, float>(msg, out blockLength));
        jumpHeight = blockLength * jumpPct;
        loaded = true;
    }

    /// <summary>
    /// Manda un mensaje cuando todas las acciones han acabado.
    /// </summary>
    /// <param name="msg">El mensaje <see cref="MsgBigCharacterAllActionsFinished"/>.</param>
    private void ServeAllActionsFinished(MsgBigCharacterAllActionsFinished msg)
    {
        StartCoroutine(ServeAllActionsFinishedCrt(msg));
    }

    /// <summary>
    /// Corrutina para mandar un mensaje cuando todas las acciones han acabado.
    /// </summary>
    /// <param name="msg">El mensaje <see cref="MsgBigCharacterAllActionsFinished"/>.</param>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator ServeAllActionsFinishedCrt(MsgBigCharacterAllActionsFinished msg)
    {
        yield return new WaitUntil(() => AreAllActionsFinished());
        EventAggregator.Instance.Publish(new ResponseWrapper<MsgBigCharacterAllActionsFinished, bool>(msg, true));
    }

    /// <summary>
    /// Coloca al robot en su posición.
    /// </summary>
    /// <param name="msg">El mensaje <see cref="MsgPlaceCharacter"/>.</param>
    private void PlaceCharacter(MsgPlaceCharacter msg)
    {
        if (msg.NewParent != null)
        {
            gameObject.transform.parent = msg.NewParent;
        }
        gameObject.transform.position = new Vector3();
        gameObject.transform.position = msg.Position;

        gameObject.transform.rotation = new Quaternion();
        gameObject.transform.Rotate(msg.Rotation);

        defaultLocalPosition = gameObject.transform.localPosition;
        defaultLocalRotation = gameObject.transform.localRotation;
        EventAggregator.Instance.Publish(new ResponseWrapper<MsgPlaceCharacter, GameObject>(msg, this.gameObject));
    }

    /// <summary>
    /// Update.
    /// </summary>
    private void Update()
    {
        StartNextAction();
    }

    /// <summary>
    /// Comprueba si han terminado todas las acciones del robot.
    /// </summary>
    /// <returns>True si han terminado todas las acciones.</returns>
    public bool AreAllActionsFinished()
    {
        if (actionList.Count == 0 && lastActionFinished)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// ¿Está el robot parado?
    /// </summary>
    /// <param name="msg">El mensaje <see cref="MsgBigRobotIdle"/>.</param>
    private void IsRobotIdle(MsgBigRobotIdle msg)
    {
        if (AreAllActionsFinished())
        {
            EventAggregator.Instance.Publish(new ResponseWrapper<MsgBigRobotIdle, bool>(msg, true));
        }
        else
        {
            EventAggregator.Instance.Publish(new ResponseWrapper<MsgBigRobotIdle, bool>(msg, false));
        }
    }

    /// <summary>
    /// Recibe mensajes para llevar a cabo acciones.
    /// </summary>
    /// <param name="msg">El mensaje <see cref="MsgBigRobotAction"/>.</param>
    private void ReceiveAction(MsgBigRobotAction msg)
    {
        if (loaded)
        {
            switch (msg.Action)
            {
                case MsgBigRobotAction.BigRobotActions.Jump:
                    DoJump(msg.Target);
                    break;

                case MsgBigRobotAction.BigRobotActions.Move:
                    DoMove(msg.Target);
                    break;

                case MsgBigRobotAction.BigRobotActions.TurnLeft:
                    DoTurnLeft();
                    break;

                case MsgBigRobotAction.BigRobotActions.TurnRight:
                    DoTurnRight();
                    break;

                case MsgBigRobotAction.BigRobotActions.Win:
                    DoWin();
                    break;

                case MsgBigRobotAction.BigRobotActions.Lose:
                    DoLose();
                    break;
            }
        }
    }

    /// <summary>
    /// Acción de ganar.
    /// </summary>
    private void DoWin()
    {
        AddAction(WinCoroutine());
    }

    /// <summary>
    /// Corrutina de la acción de ganar.
    /// </summary>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator WinCoroutine()
    {
        NotifyStartOfAction();
        SetAnimationTrigger("Greet");
        yield return null;
        NotifyEndOfAction();
    }

    /// <summary>
    /// Corrutina de la acción de perder.
    /// </summary>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator LoseCoroutine()
    {
        NotifyStartOfAction();
        SetAnimationTrigger("Lose");
        if (loseSfx != null)
        {
            EventAggregator.Instance.Publish<MsgPlaySfxAtPoint>(new MsgPlaySfxAtPoint(loseSfx, 1.0f, transform.position));
        }

        yield return new WaitForSeconds(0.5f);
        gameObject.transform.localRotation = defaultLocalRotation;
        gameObject.transform.localPosition = defaultLocalPosition;
        NotifyEndOfAction();
    }

    /// <summary>
    /// Acción de perder.
    /// </summary>
    private void DoLose()
    {
        AddAction(LoseCoroutine());
    }

    /// <summary>
    /// Inicia la primera acción de la lista.
    /// </summary>
    private void StartNextAction()
    {
        if (actionList.Count > 0 && lastActionFinished)
        {
            IEnumerator coroutine = actionList[0];
            actionList.RemoveAt(0);
            if (coroutine != null)
            {
                StartCoroutine(coroutine);
            }
        }
    }

    /// <summary>
    /// Añade una acción a la lista.
    /// </summary>
    /// <param name="action">La acción a añadir.</param>
    private void AddAction(IEnumerator action)
    {
        actionList.Add(action);
    }

    /// <summary>
    /// Notifica que se ha iniciado una acción.
    /// </summary>
    private void NotifyStartOfAction()
    {
        lastActionFinished = false;
    }

    /// <summary>
    /// Notifica que se ha acabado una acción.
    /// </summary>
    private void NotifyEndOfAction()
    {
        lastActionFinished = true;
    }

    /// <summary>
    /// Acción de tomar un item.
    /// </summary>
    /// <param name="msg">El mensaje <see cref="MsgTakeItem"/>.</param>
    private void TakeItem(MsgTakeItem msg)
    {
        AddAction(TakeItemCoroutine(msg.item, msg.numberOfItems));
    }

    /// <summary>
    /// Actualiza la posición de los items en el inventario.
    /// </summary>
    /// <param name="inventory">El inventario como un stack de items.</param>
    private void UpdateItemsPos(Stack<Item> inventory)
    {
        int numberOfItems = 1;
        foreach (Item item in inventory)
        {
            item.FollowOffset = new Vector3(0, numberOfItems * blockLength, 0);
            numberOfItems++;
        }
    }

    /// <summary>
    /// Acción de usar item
    /// </summary>
    /// <param name="msg">El mensaje <see cref="MsgUseItem"/>.</param>
    private void UseItem(MsgUseItem msg)
    {
        AddAction(UseItemCoroutine(msg.frontBlock, msg.reaction, msg.replaceBlock, msg.itemPos, msg.item, msg.inventory));
    }

    /// <summary>
    /// Corrutina de usar item.
    /// </summary>
    /// <param name="frontBlock">El bloque sobre el que se usará <see cref="Block"/>.</param>
    /// <param name="reaction">La reacción a ejecutar <see cref="EffectReaction"/>.</param>
    /// <param name="newlySpawnedObject">El bloque por el que se reemplaza <see cref="LevelObject"/>.</param>
    /// <param name="posNew">La nueva posición del item <see cref="Vector3"/>.</param>
    /// <param name="item">El item en si <see cref="Item"/>.</param>
    /// <param name="inventory">El inventario <see cref="Stack{Item}"/>.</param>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator UseItemCoroutine(Block frontBlock, EffectReaction reaction, LevelObject newlySpawnedObject, Vector3 posNew, Item item, Stack<Item> inventory)
    {
        NotifyStartOfAction();
        UpdateItemsPos(inventory);
        foreach (BlockActions blockAction in reaction.actionsToExecute)
        {
            frontBlock.ExecuteAction(blockAction);
        }
        if (item.ParentToBlockParent)
        {
            Transform blockParent = frontBlock.transform.parent;
            if (blockParent != null)
            {
                item.transform.parent = blockParent;
            }
        }

        item.transform.position = inventoryMarker.transform.position;
        if (!item.UseOnPlayersHand)
        {
            Vector3 itempos;
            itempos.x = frontBlock.SurfacePoint.x;
            itempos.y = frontBlock.SurfacePoint.y + blockLength / 2;
            itempos.z = frontBlock.SurfacePoint.z;

            item.transform.position = itempos;
        }
        SetAnimationTrigger("Use");
        item.Use();

        foreach (string trigger in reaction.animationTriggers)
        {
            frontBlock.SetAnimationTrigger(trigger);
        }
        if (reaction.replaceBlock && newlySpawnedObject != null)
        {
            newlySpawnedObject.gameObject.SetActive(true);
            if (item.Effect == Effects.Destroy)
            {
                frontBlock.Destroy();
            }
            else
            {
                frontBlock.gameObject.SetActive(false);
                Destroy(frontBlock.gameObject);
            }

            if (newlySpawnedObject != null && newlySpawnedObject.IsBlock())
            {
                Block newlySpawnedBlock = (Block)newlySpawnedObject;
                newlySpawnedBlock.ExecuteAction(BlockActions.Place);
            }
        }
        yield return new WaitForSeconds(1);
        NotifyEndOfAction();
    }

    /// <summary>
    /// Corrutina de tomar item.
    /// </summary>
    /// <param name="item">El <see cref="Item"/> a coger.</param>
    /// <param name="numberOfItems">El número de items.</param>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator TakeItemCoroutine(Item item, int numberOfItems)
    {
        NotifyStartOfAction();
        item.Pick(inventoryMarker.transform, new Vector3(0, numberOfItems * blockLength, 0));
        yield return null;
        NotifyEndOfAction();
    }

    /// <summary>
    /// Acción de saltar.
    /// </summary>
    /// <param name="target">El objetivo.</param>
    private void DoJump(Vector3 target)
    {
        float finalHeight = transform.position.y > target.y ? jumpHeight * descendJumpPct : jumpHeight;
        AddAction(JumpCoroutine(target, actionSpeed, finalHeight, takeOff));
    }

    /// <summary>
    /// Acción de girar a la derecha.
    /// </summary>
    private void DoTurnRight()
    {
        AddAction(TurnCoroutine(rotationTime, 90f));
    }

    /// <summary>
    /// Acción de girar a la izquierda.
    /// </summary>
    private void DoTurnLeft()
    {
        AddAction(TurnCoroutine(rotationTime, -90f));
    }

    /// <summary>
    /// Acción de moverse.
    /// </summary>
    /// <param name="target">El objetivo.</param>
    private void DoMove(in Vector3 target)
    {
        AddAction(MoveCoroutine(actionSpeed, target));
    }

    /// <summary>
    /// Corrutina de salto.
    /// </summary>
    /// <param name="target">El objetivo.</param>
    /// <param name="speed">La velocidad.</param>
    /// <param name="jumpHeight">Altura del salto.</param>
    /// <param name="takeOff">Porcentaje del tiempo del salto que pasa ascendiendo.</param>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator JumpCoroutine(Vector3 target, float speed, float jumpHeight, float takeOff)
    {
        NotifyStartOfAction();

        //Porcentaje del movimiento en el que nos encontramos
        float percent = 0;

        //Posicion de partida
        Vector3 originalPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        //Mientras el porcentaje sea menor o igual que 1
        while (percent <= 1)
        {
            //Para la x y la z simplemente hacemos lerp entre el punto original y el final
            Vector3 newPos = Vector3.Lerp(originalPos, target, percent);

            //Para la y tomamos en cuenta si estamos subiendo o bajando
            if (percent < takeOff)
            {
                newPos.y = Mathf.Lerp(originalPos.y, originalPos.y + jumpHeight, percent / takeOff);
            }
            else
            {
                newPos.y = Mathf.Lerp(originalPos.y + jumpHeight, target.y, (percent - takeOff) / (1 - takeOff));
            }

            //Actualizamos la posicion del jugador
            transform.position = newPos;

            //Actualizamos el porcentaje recorrido
            percent += speed * Time.deltaTime * 50;

            yield return null;
        }

        //Transportamos al personaje a su posicion final para evitar errores de exactitud
        transform.position = target;
        NotifyEndOfAction();
    }

    /// <summary>
    /// Corrutina de girar.
    /// </summary>
    /// <param name="time">Tiempo que toma la acción.</param>
    /// <param name="degrees">Grados que girar.</param>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator TurnCoroutine(float time, float degrees)
    {
        NotifyStartOfAction();

        //La rotación es la rotación actual + los grados en el eje 'y' (por eso multiplico
        //por Vector3.up (0,1,0)

        Vector3 finalRotation = transform.eulerAngles + Vector3.up * degrees;

        LTDescr lTDescr = LeanTween.rotate(this.gameObject, finalRotation, time).setEaseInOutSine();

        while (LeanTween.isTweening(lTDescr.id))
        {
            yield return null;
        }

        NotifyEndOfAction();
    }

    /// <summary>
    /// Corrutina de moverse.
    /// </summary>
    /// <param name="speed">Velocidad de la acción.</param>
    /// <param name="target">El objetivo.</param>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator MoveCoroutine(float speed, Vector3 target)
    {
        NotifyStartOfAction();
        Vector3 startPos = transform.position;
        float distance = Vector3.Distance(startPos, target);

        for (float i = 0; i <= 1;)
        {
            i += ((speed * Time.deltaTime) / distance);
            transform.position = Vector3.Lerp(startPos, target, i);
            yield return null;
        }
        transform.position = target;

        NotifyEndOfAction();
    }
}