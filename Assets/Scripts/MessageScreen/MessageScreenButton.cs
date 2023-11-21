// MessageScreenButton.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;
using static MessageScreenManager;

/// <summary>
/// Clase para los botones de <see cref="MessageScreen".
/// </summary>
[RequireComponent(typeof(Collider))]
public class MessageScreenButton : MonoBehaviour
{
    /// <summary>
    /// Delegado para informar cuando el usuario hace click.
    /// </summary>
    private OnMessageScreenButtonPressed informOnPressed;

    /// <summary>
    /// Tipo del botón.
    /// </summary>
    [SerializeField] private string buttonType;

    /// <summary>
    /// Para suscribirse al delegado.
    /// </summary>
    public OnMessageScreenButtonPressed InformOnPressed { get => informOnPressed; set => informOnPressed += value; }

    /// <summary>
    /// Retorna o cambia el tipo de botón.
    /// </summary>
    public string ButtonType { get => buttonType; set => buttonType = value; }

    public AudioClip ClickSfx { get => clickSfx; set => clickSfx = value; }

    /// <summary>
    /// Audio de hacer click.
    /// </summary>
    [SerializeField] private AudioClip clickSfx;

    /// <summary>
    /// OnSelect.
    /// </summary>
    public void OnSelect()
    {
        informOnPressed?.Invoke();

        if (clickSfx != null)
        {
            EventAggregator.Instance.Publish<MsgPlaySfxAtPoint>(new MsgPlaySfxAtPoint(clickSfx, 1.0f, transform.position));
        }
    }

    /// <summary>
    /// Pone el delegado a null.
    /// </summary>
    public void ResetDelegates()
    {
        informOnPressed = null;
    }
}