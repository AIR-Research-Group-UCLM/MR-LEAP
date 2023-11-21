// EditorObject.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;
using static EditorLogic;

/// <summary>
/// Las instancias de esta forman los objetos que guarda el editor de niveles.
/// </summary>
public class EditorObject
{
    /// <summary>
    /// GameObject que tiene asociado (por ejemplo, un bloque).
    /// </summary>
    private GameObject associatedGameobject;

    /// <summary>
    /// Tipo de objeto (Item, Block, Player).
    /// </summary>
    private EditorToolType objectType;

    /// <summary>
    /// Parámetro extra, por ejemplo para distinguir un bloque de hielo de otro de lava.
    /// </summary>
    private int objectIdentifier;

    /// <summary>
    /// Constructor de la clase.
    /// </summary>
    /// <param name="associatedGameobject">GameObject que tiene asociado.</param>
    /// <param name="objectType">Tipo de objeto (Item, Block, Player).</param>
    /// <param name="objectIdentifier">Parámetro extra.</param>
    public EditorObject(GameObject associatedGameobject, EditorToolType objectType, int objectIdentifier)
    {
        this.associatedGameobject = associatedGameobject;
        this.objectType = objectType;
        this.objectIdentifier = objectIdentifier;
    }

    /// <summary>
    /// Retorna o cambia el GameObject asociado.
    /// </summary>
    public GameObject AssociatedGameobject { get => associatedGameobject; set => associatedGameobject = value; }

    /// <summary>
    /// Retorna o cambia el tipo de objeto.
    /// </summary>
    public EditorToolType ObjectType { get => objectType; set => objectType = value; }

    /// <summary>
    /// Retorna o cambia el identificador del objeto.
    /// </summary>
    public int ObjectIdentifier { get => objectIdentifier; set => objectIdentifier = value; }
}