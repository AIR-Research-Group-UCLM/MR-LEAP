// GenericButton.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// Representa un botón genérico.
/// </summary>
public class GenericButton : MonoBehaviour
{
    /// <summary>
    /// Declaración del delegado que se llamará cuando se haga tap.
    /// </summary>
    public delegate void Clicked();

    /// <summary>
    /// Delegado al que se llamará cuando se haga tap.
    /// </summary>
    private Clicked clickCalbacks;

    /// <summary>
    /// ¿Está el botón activo?
    /// </summary>
    private bool enable = true;

    /// <summary>
    /// Añade un callback.
    /// </summary>
    public Clicked ClickCalbacks { get => clickCalbacks; set => clickCalbacks += value; }

    /// <summary>
    /// OnSelect.
    /// </summary>
    public void OnSelect()
    {
        if (enable)
        {
            clickCalbacks();
        }
    }

    /// <summary>
    /// Desactiva el botón.
    /// </summary>
    public void Disable()
    {
        enable = false;
    }

    /// <summary>
    /// Activa el botón.
    /// </summary>
    public void Enable()
    {
        enable = true;
    }
}