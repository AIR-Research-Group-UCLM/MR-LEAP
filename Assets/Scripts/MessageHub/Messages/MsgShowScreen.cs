// MsgShowScreen.cs
// Furious Koalas S.L.
// 2023

using System;
using static MessageScreenManager;

/// <summary>
/// Mensaje para mostrar una pantalla de información.
/// </summary>
public class MsgShowScreen
{
    /// <summary>
    /// Nombre de la pantalla.
    /// </summary>
    public string screenName;

    /// <summary>
    /// Lista de tuplas (nombre de botón, acción).
    /// </summary>
    public Tuple<string, OnMessageScreenButtonPressed>[] listOfActions;

    /// <summary>
    /// Segundos que debe estar la pantalla activa.
    /// </summary>
    public float seconds = -1;

    /// <summary>
    /// Inicializa el mensaje.
    /// </summary>
    /// <param name="screenName">Nombre de la pantalla.</param>
    /// <param name="listOfActions">Lista de tuplas (nombre de botón, acción).</param>
    public MsgShowScreen(string screenName, Tuple<string, OnMessageScreenButtonPressed>[] listOfActions)
    {
        this.screenName = screenName;
        this.listOfActions = listOfActions;
    }

    /// <summary>
    /// Inicializa el mensaje.
    /// </summary>
    /// <param name="screenName">Nombre de la pantalla.</param>
    /// <param name="seconds">Segundos que debe estar activa.</param>
    public MsgShowScreen(string screenName, float seconds)
    {
        this.screenName = screenName;
        this.listOfActions = new Tuple<string, OnMessageScreenButtonPressed>[0];
        this.seconds = seconds;
    }
}