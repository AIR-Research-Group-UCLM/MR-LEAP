// ConnectorVertical.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;
using static PathContainer;

/// <summary>
/// Conector vertical.
/// </summary>
public class ConnectorVertical : Road
{
    /// <summary>
    /// Entrada a la carretera.
    /// </summary>
    [SerializeField] private RoadInput rInput;

    /// <summary>
    /// Ejecuta una acción en base a una lista de argumentos.
    /// </summary>
    /// <param name="args">Los argumentos.</param>
    public override void ExecuteAction(in string[] args)
    {
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
        if (input == rInput)
        {
            GetPathByName("Path", out path);
            output = path.ioEnd;
            return true;
        }

        path = new Path();
        output = null;
        return false;
    }
}