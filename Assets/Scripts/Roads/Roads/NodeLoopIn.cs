// NodeLoopIn.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;
using static PathContainer;

/// <summary>
/// Clase de la carretera de entrada al bucle.
/// </summary>
public class NodeLoopIn : Road
{
    /// <summary>
    /// Contador de iteraciones.
    /// </summary>
    [SerializeField] private LoopCounter lCounter;

    /// <summary>
    /// Entrada por arriba.
    /// </summary>
    [SerializeField] private RoadInput inputTop;

    /// <summary>
    /// Entrada por abajo.
    /// </summary>
    [SerializeField] private RoadInput inputBottom;

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
                    lCounter.Lock();
                    Debug.Log("Counter locked.");
                    break;

                case "unlock":
                    lCounter.Unlock();
                    Debug.Log("Counter unlocked.");
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
        if (input == inputTop || input == inputBottom)
        {
            if (lCounter.ActualNumber() == 0)
            {
                //Mandarlos a outputno
                if (input == inputTop)
                {
                    GetPathByName("TopToNo", out path);
                    output = path.ioEnd;
                    return true;
                }
                if (input == inputBottom)
                {
                    GetPathByName("BottomToNo", out path);
                    output = path.ioEnd;
                    return true;
                }
            }
            else
            {
                //mandarlos a outputyes
                if (input == inputTop)
                {
                    GetPathByName("TopToYes", out path);
                    lCounter.SetNumber(lCounter.ActualNumber() - 1);
                    output = path.ioEnd;
                    return true;
                }

                if (input == inputBottom)
                {
                    GetPathByName("BottomToYes", out path);
                    lCounter.SetNumber(lCounter.ActualNumber() - 1);
                    output = path.ioEnd;
                    return true;
                }
            }
        }

        path = new Path();
        output = null;
        return false;
    }
}