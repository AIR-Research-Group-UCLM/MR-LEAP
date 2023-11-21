// ConditionCardFrame.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// La clase <see cref="ConditionCardFrame" /> incorpora la parte física de la carta
/// e informa de si se ha tocado.
/// </summary>
public class ConditionCardFrame : MonoBehaviour
{
    /// <summary>
    /// Delegado para informar de que se ha tocado la carta.
    /// </summary>
    public delegate void TappedFrame();

    /// <summary>
    /// Instancia del delegado para informar de que se ha tocado la carta.
    /// </summary>
    public TappedFrame tappedFrameDelegate;

    /// <summary>
    /// Se ejecuta si el usuario hace tap sobre la carta.
    /// </summary>
    public void OnSelect()
    {
        tappedFrameDelegate();
    }
}