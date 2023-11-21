// MsgRenderMapAndItems.cs
// Furious Koalas S.L.
// 2023

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mensaje para renderizar un nivel.
/// </summary>
public class MsgRenderMapAndItems
{
    /// <summary>
    /// Inicializa el mensaje.
    /// </summary>
    /// <param name="mapAndItems">Lista de bloques e items.</param>
    /// <param name="levelSize">Tamaño del nivel.</param>
    /// <param name="goal">Meta del nivel.</param>
    /// <param name="mapParent">Padre del nivel.</param>
    public MsgRenderMapAndItems(List<int> mapAndItems, List<int> levelSize, List<int> goal, GameObject mapParent)
    {
        this.MapAndItems = mapAndItems;
        this.LevelSize = levelSize;
        this.Goal = goal;
        this.MapParent = mapParent;
    }

    /// <summary>
    /// Retorna la lista de items y bloques.
    /// </summary>
    public List<int> MapAndItems { get; }

    /// <summary>
    /// Retorna el tamaño del nivel.
    /// </summary>
    public List<int> LevelSize { get; }

    /// <summary>
    /// Retorna la meta.
    /// </summary>
    public List<int> Goal { get; }

    /// <summary>
    /// Retorna el padre del mapa.
    /// </summary>
    public GameObject MapParent { get; }
}