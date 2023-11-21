// FaceCamera.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// Hace que el objeto que lo tiene mire siempre a la cámara principal.
/// </summary>
public class FaceCamera : MonoBehaviour
{
    /// <summary>
    /// Update.
    /// </summary>
    private void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}