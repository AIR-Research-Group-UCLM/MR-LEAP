// CameraMsgs.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// Pequeña clase que responde a las peticiones de mensajes que se hacen a la cámara <see cref="CameraMsgs" />.
/// </summary>
public class CameraMsgs : MonoBehaviour
{
    /// <summary>
    /// Awake.
    /// </summary>
    private void Awake()
    {
        EventAggregator.Instance.Subscribe<MsgGetMainCameraTransform>(ServeCameraTransform);
    }

    /// <summary>
    /// Responde a las peticiones de enviar el objeto Transform de la cámara.
    /// </summary>
    /// <param name="msg">El mensaje recibido<see cref="MsgGetMainCameraTransform"/>.</param>
    private void ServeCameraTransform(MsgGetMainCameraTransform msg)
    {
        EventAggregator.Instance.Publish<ResponseWrapper<MsgGetMainCameraTransform, Transform>>(new ResponseWrapper<MsgGetMainCameraTransform, Transform>(msg, transform));
    }
}