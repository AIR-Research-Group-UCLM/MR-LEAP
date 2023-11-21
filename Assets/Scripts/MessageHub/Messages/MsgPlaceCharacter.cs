// MsgPlaceCharacter.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// Mensaje para colocar al robot principal.
/// </summary>
public class MsgPlaceCharacter
{
    /// <summary>
    /// Posición del robot.
    /// </summary>
    private Vector3 position;

    /// <summary>
    /// Rotación del robot.
    /// </summary>
    private Vector3 rotation;

    /// <summary>
    /// Nuevo padre del robot.
    /// </summary>
    private Transform newParent;

    /// <summary>
    /// Inicializa la clase.
    /// </summary>
    /// <param name="position">Posición.</param>
    /// <param name="rotation">Rotación.</param>
    /// <param name="newParent">El nuevo padre.</param>
    public MsgPlaceCharacter(Vector3 position, Vector3 rotation, Transform newParent)
    {
        this.position = position;
        this.rotation = rotation;
        this.newParent = newParent;
    }

    /// <summary>
    /// Retorna la posición.
    /// </summary>
    public Vector3 Position { get => position; }

    /// <summary>
    /// Retorna la rotación.
    /// </summary>
    public Vector3 Rotation { get => rotation; }

    /// <summary>
    /// Retorna el padre.
    /// </summary>
    public Transform NewParent { get => newParent; }
}