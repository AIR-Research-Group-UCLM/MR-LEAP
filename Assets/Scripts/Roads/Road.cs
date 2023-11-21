// Road.cs
// Furious Koalas S.L.
// 2023

using System.Collections.Generic;
using UnityEngine;
using static PathContainer;
using static RoadIO;

/// <summary>
/// Clase padre de las carreteras.
/// </summary>
public abstract class Road : MonoBehaviour
{
    /// <summary>
    /// ¿Es un conector?
    /// </summary>
    [SerializeField] private bool connector = false;

    /// <summary>
    /// Los objetos RoadIO como un diccionario de listas con su dirección como clave.
    /// </summary>
    private Dictionary<IODirection, List<RoadIO>> ioByDirection = new Dictionary<IODirection, List<RoadIO>>();

    /// <summary>
    /// Los objetos RoadIO como un diccionario de listas con su ID como clave.
    /// </summary>
    private Dictionary<string, RoadIO> ioByID = new Dictionary<string, RoadIO>();

    /// <summary>
    /// Los caminos por su nombre.
    /// </summary>
    private Dictionary<string, Path> pathsByName = new Dictionary<string, Path>();

    /// <summary>
    /// Toda la IO.
    /// </summary>
    private RoadIO[] allIO;

    /// <summary>
    /// El container de los caminos.
    /// </summary>
    private PathContainer pathContainer;

    /// <summary>
    /// ¿Se ha ejecutado el método awake?
    /// Me ha hecho falta controlarlo porque a veces daba problemas.
    /// </summary>
    private bool awaked = false;

    /// <summary>
    /// En este caso el identificador es el nombre del objeto.
    /// </summary>
    public string RoadIdentifier
    {
        get
        {
            return gameObject.name;
        }
    }

    /// <summary>
    /// Devuelve si es conector o no.
    /// </summary>
    public bool Connector { get => connector; }

    /// <summary>
    /// Retorna la IO por su dirección.
    /// </summary>
    /// <param name="direction">La dirección <see cref="IODirection"/>.</param>
    /// <returns>La lista de IO.</returns>
    public List<RoadIO> GetRoadIOByDirection(IODirection direction)
    {
        if (!ioByDirection.ContainsKey(direction) && !awaked)
        {
            Awake();
        }

        if (!ioByDirection.ContainsKey(direction))
        {
            return null;
        }
        return ioByDirection[direction];
    }

    /// <summary>
    /// Retorna IO por su ID.
    /// </summary>
    /// <param name="ioID">El ID.</param>
    /// <returns>La IO encontrada.</returns>
    public RoadIO GetRoadIOByID(string ioID)
    {
        if (!ioByID.ContainsKey(ioID))
        {
            return null;
        }
        return ioByID[ioID];
    }

    /// <summary>
    /// Retorna toda la IO.
    /// </summary>
    /// <returns>Array de IO.</returns>
    public RoadIO[] GetAllIO()
    {
        if (!awaked)
        {
            Awake();
        }
        return allIO;
    }

    /// <summary>
    /// Awake.
    /// </summary>
    private void Awake()
    {
        if (!awaked)
        {
            pathContainer = GetComponentInChildren<PathContainer>();

            allIO = GetComponentsInChildren<RoadIO>();

            foreach (IODirection direction in System.Enum.GetValues(typeof(IODirection)))
            {
                List<RoadIO> listOfIO = new List<RoadIO>();
                ioByDirection.Add(direction, listOfIO);
            }

            foreach (RoadIO thisIO in allIO)
            {
                List<RoadIO> listOfIO = ioByDirection[thisIO.Direction];
                listOfIO.Add(thisIO);
                if (!ioByID.ContainsKey(thisIO.IOIdentifier))
                {
                    ioByID.Add(thisIO.IOIdentifier, thisIO);
                }
            }
            awaked = true;
        }
    }

    /// <summary>
    /// Ejecuta una acción en base a una lista de argumentos.
    /// </summary>
    /// <param name="args">Los argumentos.</param>
    public abstract void ExecuteAction(in string[] args);

    /// <summary>
    /// Dado un input retorna el camino que inicia en él y el output en el que termina.
    /// </summary>
    /// <param name="input">El input <see cref="RoadInput"/>.</param>
    /// <param name="path">El camino <see cref="Path"/>.</param>
    /// <param name="output">El output <see cref="RoadOutput"/>.</param>
    /// <returns>True si hay camino, false si no.</returns>
    public abstract bool GetPathAndOutput(in RoadInput input, out Path path, out RoadOutput output);

    /// <summary>
    /// Retorna un camino por su nombre.
    /// </summary>
    /// <param name="name">El nombre del camino.</param>
    /// <param name="path">El camino en si.</param>
    /// <returns>True si hay camino, false si no.</returns>
    protected bool GetPathByName(in string name, out Path path)
    {
        if (pathsByName.ContainsKey(name))
        {
            path = pathsByName[name];
            return true;
        }
        else
        {
            if (pathContainer != null)
            {
                if (pathContainer.GetPathByName(name, out path))
                {
                    pathsByName.Add(name, path);

                    return true;
                }
            }
            else
            {
                Debug.LogWarning("No PathContainer found");
            }
        }
        path = new Path();
        return false;
    }

    /// <summary>
    /// ¿Está la carretera preparada?
    /// </summary>
    /// <returns>True si lo está, false si no.</returns>
    public bool RoadReady()
    {
        return true;
    }
}