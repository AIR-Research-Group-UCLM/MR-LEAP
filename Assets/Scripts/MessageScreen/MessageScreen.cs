// MessageScreen.cs
// Furious Koalas S.L.
// 2023

using System.Collections.Generic;
using UnityEngine;
using static MessageScreenManager;

/// <summary>
/// La clase <see cref="MessageScreen" /> representa una pantalla para mostrar mensajes al usuario.
/// </summary>
public class MessageScreen : MonoBehaviour
{
    /// <summary>
    /// Nombre por el que se identifica la pantalla.
    /// </summary>
    [SerializeField] private string screenName = "defaultName";

    /// <summary>
    /// Botones que contiene esta pantalla.
    /// </summary>
    private Dictionary<string, MessageScreenButton> buttons = new Dictionary<string, MessageScreenButton>();

    /// <summary>
    /// Array con todos los botones de esta pantalla.
    /// </summary>
    private MessageScreenButton[] allButtons;

    /// <summary>
    /// Retorna el nombre de la pantalla.
    /// </summary>
    public string ScreenName { get => screenName; }

    /// <summary>
    /// Start.
    /// </summary>
    private void Start()
    {
        allButtons = GetComponentsInChildren<MessageScreenButton>();
        foreach (MessageScreenButton b in allButtons)
        {
            Debug.Log(screenName + b.ButtonType);
            if (!buttons.ContainsKey(b.ButtonType))
            {
                buttons.Add(b.ButtonType, b);
            }
        }
    }

    /// <summary>
    /// OnEnable.
    /// </summary>
    private void OnEnable()
    {
        allButtons = GetComponentsInChildren<MessageScreenButton>();
        foreach (MessageScreenButton b in allButtons)
        {
            Debug.Log(screenName + b.ButtonType);
            if (!buttons.ContainsKey(b.ButtonType))
            {
                buttons.Add(b.ButtonType, b);
            }
        }
    }

    /// <summary>
    /// Hace que el botón con nombre bType llame al delegado bDelegate cuando se hace click en él.
    /// </summary>
    /// <param name="bType">Nombre del botón.</param>
    /// <param name="bDelegate">Método al que llamar cuando se pulse el botón.</param>
    public void AddDelegateToButton(string bType, OnMessageScreenButtonPressed bDelegate)
    {
        if (buttons.ContainsKey(bType))
        {
            buttons[bType].InformOnPressed = bDelegate;
        }
    }

    /// <summary>
    /// Resetea los delegados de los botones.
    /// </summary>
    public void ResetAllButtons()
    {
        foreach (MessageScreenButton b in allButtons)
        {
            b.ResetDelegates();
        }
    }
}