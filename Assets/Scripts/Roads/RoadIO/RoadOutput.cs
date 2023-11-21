// RoadOutput.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// Marca un objeto como output de una carretera.
/// </summary>
public class RoadOutput : RoadIO
{
    /// <summary>
    /// Retorna el input asociado, si tiene.
    /// </summary>
    public RoadInput RoadInput { get; set; }

    /// <summary>
    /// Color del gizmo.
    /// </summary>
    [SerializeField] private Color color = UnityEngine.Color.red;

    /// <summary>
    /// Retorna el color del gizmo.
    /// </summary>
    /// <returns>El color.</returns>
    public override Color Color()
    {
        return color;
    }
}