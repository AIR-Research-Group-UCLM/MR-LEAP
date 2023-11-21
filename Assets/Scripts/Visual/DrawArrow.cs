using UnityEngine;

/// <summary>
/// Dibuja un gizmo con forma de flecha. Adaptado de https://forum.unity.com/threads/debug-drawarrow.85980
/// </summary>
public static class DrawArrow
{
    /// <summary>
    /// Dibuja un gizmo con forma de flecha.
    /// </summary>
    /// <param name="pos">Posición inicial de la flecha.</param>
    /// <param name="direction">Dirección de la flecha.</param>
    /// <param name="arrowHeadLength">Longitud de la cabeza de la flecha.</param>
    /// <param name="arrowHeadAngle">Ángulo de la cabeza de la flecha.</param>
    public static void ForGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.02f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.DrawRay(pos, direction);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
        Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
    }
}