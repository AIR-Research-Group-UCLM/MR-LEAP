// LevelButtons.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// Manager de los botones de colocar carreteras.
/// </summary>
public class LevelButtons : MonoBehaviour
{
    /// <summary>
    /// Tipos de botones aceptados.
    /// </summary>
    public enum Buttons
    {
        Action = 0,
        Condition = 1,
        Jump = 2,
        Loop = 3,
        Move = 4,
        Play = 5,
        Restart = 6,
        TurnLeft = 7,
        TurnRight = 8,
        Undo = 9,
        MapMenu = 10,
        Undefined = 999
    };

    /// <summary>
    /// Contador del botón.
    /// </summary>
    public ButtonCounterScript Action;

    /// <summary>
    /// Contador del botón.
    /// </summary>
    public ButtonCounterScript Condition;

    /// <summary>
    /// Contador del botón.
    /// </summary>
    public ButtonCounterScript Jump;

    /// <summary>
    /// Contador del botón.
    /// </summary>
    public ButtonCounterScript Loop;

    /// <summary>
    /// Contador del botón.
    /// </summary>
    public ButtonCounterScript Move;

    /// <summary>
    /// Contador del botón.
    /// </summary>
    public ButtonCounterScript TurnLeft;

    /// <summary>
    /// Contador del botón.
    /// </summary>
    public ButtonCounterScript TurnRight;

    /// <summary>
    /// Todos los botones.
    /// </summary>
    private RoadButton[] allRoadButtons;

    /// <summary>
    /// Desactiva todos los botones.
    /// </summary>
    /// <param name="msg">El mensaje <see cref="MsgDisableAllButtons"/>.</param>
    private void DisableAllButtons(MsgDisableAllButtons msg)
    {
        foreach (RoadButton r in allRoadButtons)
        {
            r.Disable();
        }
    }

    /// <summary>
    /// Activa un botón concreto.
    /// </summary>
    /// <param name="msg">El mensaje <see cref="MsgEnableButton"/>.</param>
    private void EnableButton(MsgEnableButton msg)
    {
        foreach (RoadButton r in allRoadButtons)
        {
            if (r.ButtonType == msg.button)
            {
                r.Enable();
            }
        }
    }

    /// <summary>
    /// Awake.
    /// </summary>
    private void Awake()
    {
        EventAggregator.Instance.Subscribe<MsgSetAvInstructions>(SetNumberOfAvailableInstructions);
        EventAggregator.Instance.Subscribe<MsgEnableAllButtons>(EnableAllButtons);
        EventAggregator.Instance.Subscribe<MsgDisableAllButtons>(DisableAllButtons);
        EventAggregator.Instance.Subscribe<MsgEnableButton>(EnableButton);
    }

    /// <summary>
    /// Activa todos los botones.
    /// </summary>
    /// <param name="msg">El mensaje <see cref="MsgEnableAllButtons"/>.</param>
    private void EnableAllButtons(MsgEnableAllButtons msg)
    {
        foreach (RoadButton r in allRoadButtons)
        {
            r.Enable();
        }
    }

    /// <summary>
    /// Start.
    /// </summary>
    private void Start()
    {
        allRoadButtons = FindObjectsOfType<RoadButton>();
    }

    /// <summary>
    /// Cambia el contador de un botón al número apropiado.
    /// </summary>
    /// <param name="button">El botón.</param>
    /// <param name="number">El número.</param>
    /// <returns>Número que se ha puesto.</returns>
    private int SetNumberOfAvailableInstructions(in Buttons button, int number)
    {
        switch (button)
        {
            case Buttons.Action:
                return Action.SetNumber(number);

            case Buttons.Condition:
                return Condition.SetNumber(number);

            case Buttons.Jump:
                return Jump.SetNumber(number);

            case Buttons.Loop:
                return Loop.SetNumber(number);

            case Buttons.Move:
                return Move.SetNumber(number);

            case Buttons.TurnLeft:
                return TurnLeft.SetNumber(number);

            case Buttons.TurnRight:
                return TurnRight.SetNumber(number);

            default:
                return 0;
        }
    }

    /// <summary>
    /// Cambia en número de instrucciones disponibles.
    /// </summary>
    /// <param name="msg">El mensaje <see cref="MsgSetAvInstructions"/>.</param>
    private void SetNumberOfAvailableInstructions(MsgSetAvInstructions msg)
    {
        AvailableInstructions availableInstructions = msg.avInst;
        SetNumberOfAvailableInstructions(Buttons.Action, availableInstructions.action);
        SetNumberOfAvailableInstructions(Buttons.Condition, availableInstructions.condition);
        SetNumberOfAvailableInstructions(Buttons.Jump, availableInstructions.jump);
        SetNumberOfAvailableInstructions(Buttons.Loop, availableInstructions.loop);
        SetNumberOfAvailableInstructions(Buttons.Move, availableInstructions.move);
        SetNumberOfAvailableInstructions(Buttons.TurnLeft, availableInstructions.turnLeft);
        SetNumberOfAvailableInstructions(Buttons.TurnRight, availableInstructions.turnRight);
    }
}