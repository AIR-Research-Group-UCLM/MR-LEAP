// MsgBigRobotAction.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// Mensaje para añadir una acción al robot principal.
/// </summary>
public class MsgBigRobotAction
{
    /// <summary>
    /// Acciones disponibles.
    /// </summary>
    public enum BigRobotActions
    {
        Jump,
        Move,
        TurnLeft,
        TurnRight,
        Win,
        Lose
    }

    /// <summary>
    /// Acción a ejecutar.
    /// </summary>
    private BigRobotActions action;

    /// <summary>
    /// Objetivo del robot.
    /// </summary>
    private Vector3 target;

    /// <summary>
    /// Retorna la acción.
    /// </summary>
    public BigRobotActions Action { get => action; }

    /// <summary>
    /// Retorna el objetivo.
    /// </summary>
    public Vector3 Target { get => target; }

    /// <summary>
    /// Constructor del mensaje.
    /// </summary>
    /// <param name="action">La acción.</param>
    /// <param name="target">Posición objetivo.</param>
    public MsgBigRobotAction(BigRobotActions action, Vector3 target)
    {
        this.action = action;
        this.target = target;
    }
}