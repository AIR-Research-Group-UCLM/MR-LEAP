// MessageScreenManager.cs
// Furious Koalas S.L.
// 2023

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager para objetos del tipo <see cref="MessageScreen" />.
/// </summary>
public class MessageScreenManager : MonoBehaviour
{
    /// <summary>
    /// Diccionario que relaciona <see cref="MessageScreen" /> con su nombre.
    /// </summary>
    private Dictionary<string, MessageScreen> messageScreensDic = new Dictionary<string, MessageScreen>();

    /// <summary>
    /// Delegado para cuando se pulsa un botón.
    /// </summary>
    public delegate void OnMessageScreenButtonPressed();

    /// <summary>
    /// Start.
    /// </summary>
    private void Start()
    {
        MessageScreen[] messageScreens = GetComponentsInChildren<MessageScreen>();
        foreach (MessageScreen messageScreen in messageScreens)
        {
            if (messageScreensDic.ContainsKey(messageScreen.ScreenName))
            {
                Debug.LogError("Duplicate screen: " + messageScreen.ScreenName);
            }
            else
            {
                messageScreensDic.Add(messageScreen.ScreenName, messageScreen);
            }
            messageScreen.gameObject.SetActive(false);
        }
        EventAggregator.Instance.Subscribe<MsgShowScreen>(ShowScreen);
        EventAggregator.Instance.Subscribe<MsgHideAllScreens>(HideAllScreens);
    }

    /// <summary>
    /// Esconde todas las pantallas de mensaje.
    /// </summary>
    /// <param name="msg">El mensaje <see cref="MsgHideAllScreens"/>.</param>
    private void HideAllScreens(MsgHideAllScreens msg)
    {
        foreach (KeyValuePair<string, MessageScreen> entry in messageScreensDic)
        {
            entry.Value.ResetAllButtons();
            entry.Value.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Muestra una pantalla concreta.
    /// </summary>
    /// <param name="msg">El mensaje <see cref="MsgShowScreen"/>.</param>
    private void ShowScreen(MsgShowScreen msg)
    {
        if (messageScreensDic.ContainsKey(msg.screenName))
        {
            MessageScreen msgScreen = messageScreensDic[msg.screenName];
            msgScreen.gameObject.SetActive(true);
            foreach (Tuple<string, OnMessageScreenButtonPressed> t in msg.listOfActions)
            {
                msgScreen.AddDelegateToButton(t.Item1, t.Item2);
            }

            if (msg.seconds > 0)
            {
                StartCoroutine(DisableScreenOnSeconds(msgScreen, msg.seconds));
            }
        }
    }

    /// <summary>
    /// Esconde una pantalla pasado un tiempo.
    /// </summary>
    /// <param name="messageScreen">La pantalla en cuestión <see cref="MessageScreen"/>.</param>
    /// <param name="seconds">Segundos que tiene que estar activa.</param>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator DisableScreenOnSeconds(MessageScreen messageScreen, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        messageScreen.gameObject.SetActive(false);
    }
}