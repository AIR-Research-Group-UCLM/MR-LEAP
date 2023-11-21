// LevelObjectFactory.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;
using static LevelObject;

/// <summary>
/// Define la clase <see cref="LevelObjectFactory" /> que instancia robots, bloques y objetos.
/// </summary>
public class LevelObjectFactory : MonoBehaviour
{
    /// <summary>
    /// Los bloques que se pueden instanciar.
    /// </summary>
    [SerializeField] private Block[] blocks = new Block[0];

    /// <summary>
    /// Los items que se pueden instanciar.
    /// </summary>
    [SerializeField] private Item[] items = new Item[0];

    /// <summary>
    /// El robot que se mueve por los bloques.
    /// </summary>
    [SerializeField] private GameObject MainCharacter;

    /// <summary>
    /// El robot que se mueve por las carreteras.
    /// </summary>
    [SerializeField] private GameObject MiniCharacter;

    /// <summary>
    /// Awake.
    /// </summary>
    private void Awake()
    {
        //blocks = Resources.LoadAll<Block>("Prefabs/Blocks");
        //items = Resources.LoadAll<Item>("Prefabs/Items");

        EventAggregator.Instance.Subscribe<MsgRenderMainCharacter>(GetMainCharacterInstance);
    }

    /// <summary>
    /// Genera la instancia de un objeto dado su ID.
    /// </summary>
    /// <param name="id">El id <see cref="int"/>.</param>
    /// <returns>El <see cref="LevelObject"/> generado.</returns>
    public LevelObject GetGameObjectInstance(in int id)
    {
        LevelObject reference = FindObjectReferenceByID(id);
        return InstantiateObject(reference);
    }

    /// <summary>
    /// Busca la referencia a un objeto a partir de su ID.
    /// </summary>
    /// <param name="id">El id <see cref="int"/>.</param>
    /// <returns>El <see cref="LevelObject"/> encontrado.</returns>
    private LevelObject FindObjectReferenceByID(in int id)
    {
        foreach (Block block in blocks)
        {
            if (block.BlockType == (Blocks)id)
            {
                return block;
            }
        }

        foreach (Item item in items)
        {
            if (item.ItemType == (Items)id)
            {
                return item;
            }
        }

        return null;
    }

    /// <summary>
    /// Instancia un objeto a partir de su referencia.
    /// </summary>
    /// <param name="reference">La referencia <see cref="LevelObject"/>.</param>
    /// <returns>El <see cref="LevelObject"/> generado.</returns>
    private LevelObject InstantiateObject(LevelObject reference)
    {
        return Instantiate(reference, reference.transform.position, reference.transform.rotation);
    }

    /// <summary>
    /// Retorna la instancia del robot principal.
    /// </summary>
    /// <param name="msg">El mensaje <see cref="MsgRenderMainCharacter"/>.</param>
    public void GetMainCharacterInstance(MsgRenderMainCharacter msg)
    {
        GameObject mainCharacter = Instantiate(MainCharacter);
        EventAggregator.Instance.Publish(new ResponseWrapper<MsgRenderMainCharacter, GameObject>(msg, mainCharacter));
    }

    /// <summary>
    /// Retorna la instancia del robot principal.
    /// </summary>
    /// <returns>El <see cref="GameObject"/> generado.</returns>
    public GameObject InstantiateMainCharacter()
    {
        return Instantiate(MainCharacter);
    }

    /// <summary>
    /// Retorna la instancia del robot pequeño.
    /// </summary>
    /// <returns>El <see cref="GameObject"/> generado.</returns>
    public GameObject GetMiniCharacterInstance()
    {
        return Instantiate(MiniCharacter, MiniCharacter.transform.position, MiniCharacter.transform.rotation);
    }
}