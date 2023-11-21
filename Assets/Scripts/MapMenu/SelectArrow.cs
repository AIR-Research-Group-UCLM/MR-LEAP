// SelectArrow.cs 
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// Clase de las flechas de selección de nivel.
/// </summary>
public class SelectArrow : MonoBehaviour
{
    /// <summary>
    /// Definición del delegado de métodos a los que llamar cuando se hace click.
    /// </summary>
    public delegate void CallbackDelegate();

    /// <summary>
    /// Delegado de métodos a los que llamar cuando se hace click.
    /// </summary>
    private CallbackDelegate callbackDelegate;

    /// <summary>
    /// OnSelect.
    /// </summary>
    public void OnSelect()
    {
        callbackDelegate?.Invoke();
    }

    /// <summary>
    /// Permite a otros objetos suscribirse a este para ejecutar el callback cuando se pulse.
    /// </summary>
    /// <param name="action">El método al que llamar cuando se pulse en esta flecha.</param>
    public void InformMeOfClickedArrow(CallbackDelegate action)
    {
        callbackDelegate += action;
    }
}
