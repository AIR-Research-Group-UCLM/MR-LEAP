// MapContainer.cs
// Furious Koalas S.L.
// 2023

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contenedor para los niveles del juego con algunas utilidades.
/// </summary>
public class MapContainer : MonoBehaviour
{
    /// <summary>
    /// Centro del mapa.
    /// </summary>
    private GameObject mapCenter;

    /// <summary>
    /// Retorna o cambia el centro del mapa.
    /// </summary>
    public Vector3 MapCenter { get => mapCenter.transform.position - transform.position; }

    /// <summary>
    /// Start.
    /// </summary>
    private void Start()
    {
        mapCenter = new GameObject();
        mapCenter.transform.parent = transform;
    }

    /// <summary>
    /// OnDrawGizmos.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + MapCenter, 0.01f);
    }

    /// <summary>
    /// Pone el mapa en el punto requerido y haciendo que quede sobre la coordenada y
    /// especificada.
    /// </summary>
    /// <param name="mapCenterPos">Donde hay que poner el centro del mapa.</param>
    /// <param name="surfaceY">Superficie sobre la que se asienta el mapa.</param>
    /// <param name="blockLength">Longitud de los bloques del mapa.</param>
    public void MoveMapTo(in Vector3 mapCenterPos, float surfaceY, float blockLength)
    {
        Vector3 newPos;
        newPos.x = mapCenterPos.x - MapCenter.x;
        newPos.y = surfaceY + blockLength / 2;
        newPos.z = mapCenterPos.z - MapCenter.z;
        transform.position = newPos;
    }

    /// <summary>
    /// Actualiza el punto central del mapa.
    /// </summary>
    /// <param name="mapSize">Tamaño del mapa.</param>
    /// <param name="blockLength">Longitud de los bloques del mapa.<see cref="float"/>.</param>
    public void UpdateMapCenter(List<int> mapSize, float blockLength)
    {
        Vector3 mapCenter;
        mapCenter.x = ((mapSize[0] - 1) * blockLength) / 2;
        mapCenter.y = 0;
        mapCenter.z = ((mapSize[2] - 1) * blockLength) / 2;
        mapCenter += transform.position;

        this.mapCenter.transform.position = mapCenter;
    }
}