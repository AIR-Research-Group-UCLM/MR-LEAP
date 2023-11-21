// Counter.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// Contador para el número de instrucciones restantes.
/// </summary>
public class Counter : MonoBehaviour
{
    /// <summary>
    /// Número máximo.
    /// </summary>
    public int maxNumber = 9;

    /// <summary>
    /// Número por defecto.
    /// </summary>
    public int defaultNumber = 0;

    /// <summary>
    /// Padre de los números.
    /// </summary>
    private GameObject numbersParent;

    /// <summary>
    /// Array con los números.
    /// </summary>
    private GameObject[] numbers;

    /// <summary>
    /// Número actual.
    /// </summary>
    private int actualNumber;

    /// <summary>
    /// Retorna el número actual.
    /// </summary>
    public int ActualNumber { get => actualNumber; }

    /// <summary>
    /// Awake.
    /// </summary>
    private void Awake()
    {
        numbersParent = transform.Find("Numbers").gameObject;
        numbers = new GameObject[maxNumber + 1];
        for (int i = 0; i <= maxNumber; i++)
        {
            numbers[i] = numbersParent.transform.Find("RepeatsX" + i).gameObject;
            numbers[i].SetActive(false);
        }
        actualNumber = SetNumber(defaultNumber);
    }

    /// <summary>
    /// Pone un número en el contador. Si se pasa pone el número máximo y si es menor
    /// que cero pone el cero.
    /// </summary>
    /// <param name="number">El número a poner.</param>
    /// <returns>El número que se ha puesto realmente.</returns>
    public int SetNumber(in int number)
    {
        numbers[actualNumber].SetActive(false);
        int numberAux = number;
        if (number < 0)
        {
            numberAux = maxNumber;
        }

        if (number > maxNumber)
        {
            numberAux = 0;
        }

        numbers[numberAux].SetActive(true);
        actualNumber = numberAux;
        return numberAux;
    }
}