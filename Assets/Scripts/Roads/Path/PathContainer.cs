// PathContainer.cs
// Furious Koalas S.L.
// 2023

using System;
using UnityEngine;

/// <summary>
/// Contiene caminos por los que se moverá el robot que son listas de puntos ordenados.
/// </summary>
public class PathContainer : MonoBehaviour
{
    /// <summary>
    /// Lista de caminos.
    /// </summary>
    [SerializeField] private Path[] paths = new Path[0];

    /// <summary>
    /// OnDrawGizmos.
    /// </summary>
    private void OnDrawGizmos()
    {
        foreach (Path p in paths)
        {
            if (p.drawPreview)
            {
                iTween.DrawPath(p.points, p.color);
                if (p.pathName != null)
                {
                    int nPoints = 0;
                    Vector3 total = new Vector3(0, 0, 0);
                    foreach (Transform point in p.points)
                    {
                        nPoints++;
                        total = total + point.position;
                    }

                    if (nPoints > 0)
                    {
                        total = total / nPoints;
                        GUIStyle style = new GUIStyle();
                        style.normal.textColor = p.color;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Retorna un camino por su nombre.
    /// </summary>
    /// <param name="name">El nombre del camino.</param>
    /// <param name="path">El camino.</param>
    /// <returns>True si se ha encontrado, false si no.</returns>
    public bool GetPathByName(in string name, out Path path)
    {
        foreach (Path p in paths)
        {
            if (p.pathName.Equals(name))
            {
                path = p;
                return true;
            }
        }
        path = new Path();
        return false;
    }

    /// <summary>
    /// Define un camino.
    /// </summary>
    [Serializable]
    public struct Path
    {
        /// <summary>
        /// Nombre del camino.
        /// </summary>
        public string pathName;

        /// <summary>
        /// Puntos que lo forman.
        /// </summary>
        public Transform[] points;

        /// <summary>
        /// Color del camino.
        /// </summary>
        public Color color;

        /// <summary>
        /// ¿Se dibujará el gizmo del camino?
        /// </summary>
        public bool drawPreview;

        /// <summary>
        /// IO inicial.
        /// </summary>
        public RoadInput ioBegin;

        /// <summary>
        /// IO final.
        /// </summary>
        public RoadOutput ioEnd;
    }
}