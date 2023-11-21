// NodeIfIn.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;
using static PathContainer;

/// <summary>
/// Clase de entrada al if.
/// </summary>
public class NodeIfIn : Road
{
    /// <summary>
    /// Entrada al if.
    /// </summary>
    [SerializeField] private RoadInput inputIf;

    /// <summary>
    /// Objeto para elegir cartas de condición.
    /// </summary>
    [SerializeField] private ConditionCardPicker cPicker;

    /// <summary>
    /// Ejecuta una acción en base a una lista de argumentos.
    /// </summary>
    /// <param name="args">Los argumentos.</param>
    public override void ExecuteAction(in string[] args)
    {
        if (args.Length > 0)
        {
            switch (args[0])
            {
                case "lock":
                    cPicker.Lock();
                    Debug.Log("Picker locked.");
                    break;

                case "unlock":
                    cPicker.Unlock();
                    Debug.Log("Picker unlocked.");
                    break;

                default:
                    Debug.LogWarning("Undefined action: " + args[0]);
                    break;
            }
        }
    }

    /// <summary>
    /// Dado un input retorna el camino que inicia en él y el output en el que termina.
    /// </summary>
    /// <param name="input">El input <see cref="RoadInput"/>.</param>
    /// <param name="path">El camino <see cref="Path"/>.</param>
    /// <param name="output">El output <see cref="RoadOutput"/>.</param>
    /// <returns>True si hay camino, false si no.</returns>
    public override bool GetPathAndOutput(in RoadInput input, out Path path, out RoadOutput output)
    {
        if (input == inputIf)
        {
            if (IsConditionMet())
            {
                GetPathByName("Yes", out path);
            }
            else
            {
                GetPathByName("No", out path);
            }

            output = path.ioEnd;
            return true;
        }

        path = new Path();
        output = null;
        return false;
    }

    /// <summary>
    /// Comprueba si se cumple la condición de la carta.
    /// </summary>
    /// <returns>True si se cumple, false si no.</returns>
    private bool IsConditionMet()
    {
        return GameLogic.Instance.CheckNextBlockDownProperty(cPicker.GetCardProperty());
    }
}