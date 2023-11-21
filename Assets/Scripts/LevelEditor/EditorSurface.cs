// EditorSurface.cs
// Furious Koalas S.L.
// 2023

using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Las instancias de esta clase se usan para generar una superficie en la que el usuario puede poner bloques y objetos.
/// </summary>
public class EditorSurface : MonoBehaviour
{
    /// <summary>
    /// El MessageWarehouse de esta clase.
    /// </summary>
    private MessageWarehouse msgWar;

    /// <summary>
    /// La longitud de los bloques.
    /// </summary>
    private float blockLength;

    /// <summary>
    /// El tamaño del nivel x,y,z.
    /// </summary>
    private List<int> levelSize;

    /// <summary>
    /// Array con los puntos sobre los que puede pulsar el usuario.
    /// </summary>
    private EditorSurfacePoint[] points;

    /// <summary>
    /// Material de los puntos sobre los que puede pulsar el usuario.
    /// </summary>
    [SerializeField] private Material cubeMaterial;

    /// <summary>
    /// ¿Está esta superficie preparada?
    /// </summary>
    private bool readySurface = false;

    /// <summary>
    /// Awake.
    /// </summary>
    private void Awake()
    {
        EventAggregator.Instance.Subscribe<MsgResetEditorSurface>(ResetEditorSurface);
    }

    /// <summary>
    /// Start.
    /// </summary>
    private void Start()
    {
        msgWar = new MessageWarehouse(EventAggregator.Instance);
    }

    /// <summary>
    /// OnEnable.
    /// </summary>
    private void OnEnable()
    {
        if (!readySurface)
        {
            StartCoroutine(SetUpSurface());
        }
    }

    /// <summary>
    /// Corrutina que inicializa la superficie.
    /// </summary>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator SetUpSurface()
    {
        readySurface = false;
        if (msgWar == null)
        {
            yield return null;
        }
        MsgBlockLength msg = new MsgBlockLength();
        msgWar.PublishMsgAndWaitForResponse<MsgBlockLength, float>(msg);
        yield return new WaitUntil(() => msgWar.IsResponseReceived<MsgBlockLength, float>(msg, out blockLength));

        MsgEditorMapSize msg2 = new MsgEditorMapSize();
        msgWar.PublishMsgAndWaitForResponse<MsgEditorMapSize, List<int>>(msg2);
        yield return new WaitUntil(() => msgWar.IsResponseReceived<MsgEditorMapSize, List<int>>(msg2, out levelSize));
        points = new EditorSurfacePoint[levelSize[0] * levelSize[2]];
        int index = 0;
        EditorSurfacePoint editorSurfacePoint;
        Interactable editorSurfacePointInteractable;
        GameObject objToSpawn;

        for (int x = 0; x < levelSize[0]; x++)
        {
            for (int z = 0; z < levelSize[2]; z++)
            {
                objToSpawn = GameObject.CreatePrimitive(PrimitiveType.Cube);
                objToSpawn.GetComponent<Renderer>().material = cubeMaterial;
                objToSpawn.transform.localScale = new Vector3(objToSpawn.transform.localScale.x * (blockLength - 0.001f), 0.001f, objToSpawn.transform.localScale.z * (blockLength - 0.001f));
                objToSpawn.transform.parent = transform;

                objToSpawn.transform.position = new Vector3(transform.position.x + blockLength * x, transform.position.y - blockLength / 2, transform.position.z + blockLength * z);
                objToSpawn.transform.RotateAround(transform.position, Vector3.up, transform.eulerAngles.y);

                editorSurfacePoint = objToSpawn.AddComponent<EditorSurfacePoint>();
                editorSurfacePointInteractable = objToSpawn.AddComponent<Interactable>();
                editorSurfacePointInteractable.OnClick.AddListener(editorSurfacePoint.OnSelect);

                editorSurfacePoint.EditorSurface = transform;
                editorSurfacePoint.SetPosition(x, z);
                editorSurfacePoint.BlockLength = blockLength;
                points[index] = editorSurfacePoint;
                index++;
                yield return null;
            }
        }
        readySurface = true;
    }

    /// <summary>
    /// Devuelve la superficie a su estado original.
    /// </summary>
    /// <param name="msg">El mensaje <see cref="MsgResetEditorSurface"/>.</param>
    private void ResetEditorSurface(MsgResetEditorSurface msg)
    {
        if (readySurface)
        {
            int index = 0;
            for (int x = 0; x < levelSize[0]; x++)
            {
                for (int z = 0; z < levelSize[2]; z++)
                {
                    points[index].gameObject.transform.parent = transform;
                    points[index].ResetBox();
                    points[index].SetPosition(x, z);
                    points[index].BlockLength = blockLength;
                    points[index].EditorSurface = transform;
                    index++;
                }
            }
        }
    }
}