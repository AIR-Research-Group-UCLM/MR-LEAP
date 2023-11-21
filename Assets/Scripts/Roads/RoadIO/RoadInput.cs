// RoadInput.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// Marca un objeto como input de una carretera.
/// </summary>
public class RoadInput : RoadIO
{
    /// <summary>
    /// Retorna el output asociado, si tiene.
    /// </summary>
    public RoadOutput RoadOutput { get; set; }

    /// <summary>
    /// Color del gizmo.
    /// </summary>
    [SerializeField] private Color color = UnityEngine.Color.green;

    /// <summary>
    /// Retorna el color del gizmo.
    /// </summary>
    /// <returns>El color.</returns>
    public override Color Color()
    {
        return color;
    }
}