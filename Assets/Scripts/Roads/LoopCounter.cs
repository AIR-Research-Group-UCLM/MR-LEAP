// LoopCounter.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// Contador del número de iteraciones de un loop.
/// </summary>
public class LoopCounter : MonoBehaviour
{
    /// <summary>
    /// Número maximo.
    /// </summary>
    [SerializeField] private int maxNumber = 9;

    /// <summary>
    /// Número por defecto.
    /// </summary>
    [SerializeField] private int defaultNumber = 0;

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
    /// ¿Está bloqueado?
    /// </summary>
    private bool locked = false;

    /// <summary>
    /// Retorna si está bloqueado.
    /// </summary>
    public bool Locked
    {
        get { return locked; }
    }

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
    /// Pone el número que se le indica si es posible, aunque si es menor que cero se pone a cero
    /// y si es mayor que el número máximo se pone a ese.
    /// </summary>
    /// <param name="number">El número a poner.</param>
    /// <returns>El número que se ha puesto.</returns>
    public int SetNumber(int number)
    {
        numbers[actualNumber].SetActive(false);
        int numberAux = number;
        if (number < 0)
        {
            numberAux = 0;
        }

        if (number > maxNumber)
        {
            numberAux = 0;
        }

        numbers[numberAux].SetActive(true);
        actualNumber = numberAux;
        return numberAux;
    }

    /// <summary>
    /// OnSelect.
    /// </summary>
    public void OnSelect()
    {
        if (!locked)
        {
            SetNumber(actualNumber + 1);
        }
    }

    /// <summary>
    /// Retorna el número actual.
    /// </summary>
    /// <returns>El número actual.</returns>
    public int ActualNumber()
    {
        return actualNumber;
    }

    /// <summary>
    /// Bloquea el contador.
    /// </summary>
    public void Lock()
    {
        locked = true;
    }

    /// <summary>
    /// Desbloquea el contador.
    /// </summary>
    public void Unlock()
    {
        locked = false;
    }
}