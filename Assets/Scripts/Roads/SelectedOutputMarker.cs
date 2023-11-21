// SelectedOutputMarker.cs
// Furious Koalas S.L.
// 2023

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase de la flecha para seleccionar carretera.
/// </summary>
public class SelectedOutputMarker : MonoBehaviour
{
    /// <summary>
    /// Objeto que se muestra marcando la carretera seleccionada.
    /// </summary>
    [SerializeField] private GameObject sphere;

    /// <summary>
    /// Lógica de posicionamiento de carreteras.
    /// </summary>
    [SerializeField] private RoadPlacementLogic RoadPlacementLogic;

    /// <summary>
    /// ¿Se está usando la flecha?
    /// </summary>
    private bool placing = false;

    public void OnManipulationStarted()
    {
        sphere.transform.position = SearchClosestsIO(RoadPlacementLogic.FirstInput).transform.position;
        sphere.SetActive(true);
        placing = true;
    }

    /// <summary>
    /// Update.
    /// </summary>
    private void Update()
    {
        if (placing)
        {
            sphere.transform.position = SearchClosestsIO(RoadPlacementLogic.FirstInput).transform.position;
        }
    }

    public void OnManipulationEnded()
    {
        placing = false;
        FindAndSelectClosestIO();
    }

    /// <summary>
    /// Busca y selecciona la IO más cercana.
    /// </summary>
    public void FindAndSelectClosestIO()
    {
        RoadIO pivotIO = RoadPlacementLogic.FirstInput;

        if (pivotIO != null)
        {
            RoadIO closests = SearchClosestsIO(pivotIO);
            if (closests != null)
            {
                RoadPlacementLogic.SelectedIO = closests;
                gameObject.transform.position = closests.transform.position;
                sphere.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Busca la IO más cercana.
    /// </summary>
    /// <param name="pivotIO">IO por la que empezar a buscar.</param>
    /// <returns>La IO más cercana.</returns>
    private RoadIO SearchClosestsIO(RoadIO pivotIO)
    {
        RoadIO closestSelectableToPivotIO = null ;

        if (pivotIO != null)
        {
            RoadIO closestIO = pivotIO;

            List<RoadIO> processedIO = new List<RoadIO>();

            Stack<RoadIO> ioToProc = new Stack<RoadIO>();

            RoadIO[] tmpe;

            tmpe = pivotIO.GetParentRoad().GetAllIO();

            foreach (var io in tmpe)
            {
                if (io != null && io.CanBeSelected)
                {
                    ioToProc.Push(io);
                    closestSelectableToPivotIO = io;
                }
            }

            while (ioToProc.Count > 0)
            {
                RoadIO toProc = ioToProc.Pop();
                RoadIO connectedTo = toProc.ConnectedTo;

                tmpe = toProc.GetParentRoad().GetAllIO();

                foreach (RoadIO rio in tmpe)
                {
                    if (!processedIO.Contains(rio))
                    {
                        ioToProc.Push(rio);
                    }
                }

                if (connectedTo != null)
                {
                    tmpe = connectedTo.GetParentRoad().GetAllIO();

                    foreach (RoadIO rio in tmpe)
                    {
                        if (!processedIO.Contains(rio))
                        {
                            ioToProc.Push(rio);
                        }
                    }
                }

                if (toProc.CanBeSelected && Vector3.Distance(closestIO.transform.position, transform.position) > Vector3.Distance(toProc.transform.position, transform.position))
                {
                    closestIO = toProc;
                }

                if (!processedIO.Contains(toProc))
                {
                    processedIO.Add(toProc);
                }
            }

            if (closestIO.ConnectedTo != null && closestIO.ConnectedTo is RoadOutput && closestIO.ConnectedTo.CanBeSelected)
            {
                closestIO = closestIO.ConnectedTo;
            }

            if (closestIO.CanBeSelected)
            {
                return closestIO;
            }
 
        }

        
        return closestSelectableToPivotIO;
    }
}