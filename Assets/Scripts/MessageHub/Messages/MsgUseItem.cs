// MsgUseItem.cs
// Furious Koalas S.L.
// 2023

using System.Collections.Generic;
using UnityEngine;
using static Block;

/// <summary>
/// Mensaje que indica al robot que debe usar un item.
/// </summary>
public class MsgUseItem
{
    /// <summary>
    /// Bloque sobre el que usar el item.
    /// </summary>
    public Block frontBlock;

    /// <summary>
    /// Reacción del item.
    /// </summary>
    public EffectReaction reaction;

    /// <summary>
    /// ¿Hay que cambiar el bloque por otro?
    /// </summary>
    public LevelObject replaceBlock;

    /// <summary>
    /// Posición del item.
    /// </summary>
    public Vector3 itemPos;

    /// <summary>
    /// El item.
    /// </summary>
    public Item item;

    /// <summary>
    /// Inventario del robot.
    /// </summary>
    public Stack<Item> inventory;

    /// <summary>
    /// Inicializa el mensaje.
    /// </summary>
    /// <param name="frontBlock">Bloque sobre el que usar el item.</param>
    /// <param name="reaction">Reacción del item.</param>
    /// <param name="replaceBlock">¿Hay que cambiar el bloque por otro?.</param>
    /// <param name="itemPos">Posición del item.</param>
    /// <param name="item">El item.</param>
    /// <param name="inventory">Inventario del robot.</param>
    public MsgUseItem(Block frontBlock, EffectReaction reaction, LevelObject replaceBlock, Vector3 itemPos, Item item, Stack<Item> inventory)
    {
        this.frontBlock = frontBlock;
        this.reaction = reaction;
        this.replaceBlock = replaceBlock;
        this.item = item;
        this.itemPos = itemPos;
        this.inventory = new Stack<Item>();
        //Ya que las llamadas no son sincronas es importante llevar una copia del inventario y no una referencia
        Stack<Item> auxStack = new Stack<Item>();
        foreach (Item i in inventory)
        {
            auxStack.Push(i);
        }

        foreach (Item i in auxStack)
        {
            this.inventory.Push(i);
        }
        auxStack = null;
    }
}