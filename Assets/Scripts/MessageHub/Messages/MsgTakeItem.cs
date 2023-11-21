// MsgTakeItem.cs
// Furious Koalas S.L.
// 2023

/// <summary>
/// Mensaje para recoger un item.
/// </summary>
public class MsgTakeItem
{
    /// <summary>
    /// El item.
    /// </summary>
    public Item item;

    /// <summary>
    /// Número de items en el inventario.
    /// </summary>
    public int numberOfItems;

    /// <summary>
    /// Constructor del mensaje.
    /// </summary>
    /// <param name="item">El item.</param>
    /// <param name="numberOfItems">Número de items en el inventario.</param>
    public MsgTakeItem(Item item, int numberOfItems)
    {
        this.item = item;
        this.numberOfItems = numberOfItems;
    }
}