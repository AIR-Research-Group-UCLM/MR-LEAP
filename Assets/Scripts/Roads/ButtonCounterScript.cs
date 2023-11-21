// ButtonCounterScript.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// Actúa como intermediario con el contador de un botón.
/// </summary>
public class ButtonCounterScript : MonoBehaviour
{
    /// <summary>
    /// Objeto que contiene el contador.
    /// </summary>
    private GameObject counterObject;

    /// <summary>
    /// Script del contador.
    /// </summary>
    private Counter counterScript;

    /// <summary>
    /// Awake.
    /// </summary>
    private void Awake()
    {
        counterObject = transform.Find("Counter").gameObject;
        counterScript = counterObject.GetComponent<Counter>();
    }

    /// <summary>
    /// Pone un número en ese contador.
    /// </summary>
    /// <param name="number">El número a poner.</param>
    /// <returns>El número que se ha puesto.</returns>
    public int SetNumber(int number)
    {
        return counterScript.SetNumber(number);
    }
}