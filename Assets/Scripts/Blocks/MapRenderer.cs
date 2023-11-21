// MapRenderer.cs
// Furious Koalas S.L.
// 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelObject;

/// <summary>
/// La clase <see cref="MapRenderer" /> genera el mapa a partir de los datos que se le pasen.
/// </summary>
public class MapRenderer : MonoBehaviour
{
    /// <summary>
    /// Longitud del bloque.
    /// </summary>
    [SerializeField] private float blockLength = 1f;

    /// <summary>
    /// La factoría de objetos.
    /// </summary>
    [SerializeField] private LevelObjectFactory levelObjectsFactory;

    /// <summary>
    /// Retorna la longitud del bloque.
    /// </summary>
    public float BlockLength { get => blockLength; }

    /// <summary>
    /// Instancia de la clase.
    /// </summary>
    private static MapRenderer mapRenderer;

    /// <summary>
    /// Retorna la instancia de la clase.
    /// </summary>
    public static MapRenderer Instance
    {
        get
        {
            if (!mapRenderer)
            {
                mapRenderer = FindObjectOfType(typeof(MapRenderer)) as MapRenderer;

                if (!mapRenderer)
                {
                    Debug.LogError("There needs to be one active MapRenderer script on a GameObject in your scene.");
                }
            }

            return mapRenderer;
        }
    }

    /// <summary>
    /// Awake.
    /// </summary>
    private void Awake()
    {
        EventAggregator.Instance.Subscribe<MsgRenderMapAndItems>(RenderMapAndItems);
        EventAggregator.Instance.Subscribe<MsgBlockLength>(ServeBlockLength);
    }

    /// <summary>
    /// Retorna la longitud de los bloques en respuesta al mensaje apropiado.
    /// </summary>
    /// <param name="msg">El mensaje <see cref="MsgBlockLength"/>.</param>
    private void ServeBlockLength(MsgBlockLength msg)
    {
        EventAggregator.Instance.Publish<ResponseWrapper<MsgBlockLength, float>>(new ResponseWrapper<MsgBlockLength, float>(msg, blockLength));
    }

    /// <summary>
    /// Genera el mapa cuando se le pide.
    /// </summary>
    /// <param name="msg">El mensaje <see cref="MsgRenderMapAndItems"/>.</param>
    private void RenderMapAndItems(MsgRenderMapAndItems msg)
    {
        StartCoroutine(RenderMapAndItemsCoroutine(msg));
    }

    /// <summary>
    /// Corrutina que genera el mapa y pública un mensaje para informar de que ha terminado su función.
    /// </summary>
    /// <param name="msg">El mensaje <see cref="MsgRenderMapAndItems"/>.</param>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator RenderMapAndItemsCoroutine(MsgRenderMapAndItems msg)
    {
        List<int> levelSize = msg.LevelSize;
        List<int> mapAndItems = msg.MapAndItems;
        LevelObject[] objectReferences = new LevelObject[levelSize[0] * levelSize[1] * levelSize[2]];
        for (int y = 0; y < levelSize[1]; y++)
        {
            for (int x = 0; x < levelSize[0]; x++)
            {
                for (int z = 0; z < levelSize[2]; z++)
                {
                    int blockToSpawn = Get(mapAndItems, levelSize, x, y, z);

                    LevelObject block = levelObjectsFactory.GetGameObjectInstance(blockToSpawn);

                    objectReferences[x + z * levelSize[0] + y * (levelSize[0] * levelSize[2])] = block;
                    block.transform.position = new Vector3(msg.MapParent.transform.position.x + blockLength * x, msg.MapParent.transform.position.y + blockLength * y, msg.MapParent.transform.position.z + blockLength * z);
                    block.transform.RotateAround(msg.MapParent.transform.position, Vector3.up, msg.MapParent.transform.eulerAngles.y);
                    block.transform.parent = msg.MapParent.transform;

                    if (x == msg.Goal[0] && y == msg.Goal[1] - 1 && z == msg.Goal[2])
                    {
                        LevelObject flag = levelObjectsFactory.GetGameObjectInstance((int)Items.FlagItem);
                        flag.transform.position = new Vector3(msg.MapParent.transform.position.x + blockLength * msg.Goal[0], msg.MapParent.transform.position.y + blockLength * msg.Goal[1], msg.MapParent.transform.position.z + blockLength * msg.Goal[2]);
                        flag.transform.RotateAround(msg.MapParent.transform.position, Vector3.up, msg.MapParent.transform.eulerAngles.y);
                        flag.transform.parent = block.transform;
                    }

                    block.gameObject.SetActive(false);
                }
                yield return null;
            }
        }

        EventAggregator.Instance.Publish(new ResponseWrapper<MsgRenderMapAndItems, LevelObject[]>(msg, objectReferences));
    }

    /// <summary>
    /// Genera un bloque por su id.
    /// </summary>
    /// <param name="blockToSpawn">El id del bloque <see cref="int"/>.</param>
    /// <returns>El <see cref="LevelObject"/> generado.</returns>
    public LevelObject SpawnBlock(int blockToSpawn)
    {
        LevelObject block = levelObjectsFactory.GetGameObjectInstance(blockToSpawn);

        return block;
    }

    /// <summary>
    /// Genera el robot principal.
    /// </summary>
    /// <returns>El <see cref="GameObject"/> generado.</returns>
    public GameObject RenderMainCharacter()
    {
        return levelObjectsFactory.InstantiateMainCharacter();
    }

    /// <summary>
    /// Dado una estructura con los objetos y unas coordenadas devuelve el id del objeto en esa posición.
    /// </summary>
    /// <param name="mapAndItems">Los objetos <see cref="List{int}"/>.</param>
    /// <param name="levelSize">El tamaño del nivel <see cref="List{int}"/>.</param>
    /// <param name="x">Coordenada x.</param>
    /// <param name="y">Coordenada y.</param>
    /// <param name="z">Coordenada z.</param>
    /// <returns>El id del bloque <see cref="int"/>.</returns>
    private int Get(in List<int> mapAndItems, in List<int> levelSize, in int x, in int y, in int z)
    {
        if (x < 0 || x >= levelSize[0]) return (int)Blocks.NoBlock;
        if (y < 0 || y >= levelSize[1]) return (int)Blocks.NoBlock;
        if (z < 0 || z >= levelSize[2]) return (int)Blocks.NoBlock;
        return mapAndItems[x + z * levelSize[0] + y * (levelSize[0] * levelSize[2])];
    }
}