// EditorSurfacePoint.cs 
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// La clase <see cref="EditorSurfacePoint" /> representa al objeto físico en el que los usuarios harán click
/// al tocar la superficie del editor de niveles.
/// </summary>
public class EditorSurfacePoint : MonoBehaviour
{
    /// <summary>
    /// Posición x,y,z de este punto en el mapa.
    /// </summary>
    private int[] mapPos = new int[3];

    /// <summary>
    /// Longitud de los bloques.
    /// </summary>
    private float blockLength;

    /// <summary>
    /// Transform de la superficie del editor de niveles.
    /// </summary>
    private Transform editorSurface;

    /// <summary>
    /// La colisión para detectar los taps del usuario.
    /// </summary>
    private BoxCollider box;

    /// <summary>
    /// Retorna la longitud de los bloques.
    /// </summary>
    public float BlockLength { get => blockLength; set => blockLength = value; }

    /// <summary>
    /// Retorna la transformación de la superficie del editor.
    /// </summary>
    public Transform EditorSurface { get => editorSurface; set => editorSurface = value; }

    /// <summary>
    /// Indica a qué posición x,z corresponde este objeto.
    /// </summary>
    /// <param name="x">Coordenada x.</param>
    /// <param name="z">Coordenada z.</param>
    public void SetPosition(int x, int z)
    {
        mapPos[0] = x;
        mapPos[1] = 0;
        mapPos[2] = z;
    }

    /// <summary>
    /// Llamado cuando el usuario hace tap.
    /// </summary>
    public void OnSelect()
    {
        EventAggregator.Instance.Publish(new MsgEditorSurfaceTapped(this));
    }

    /// <summary>
    /// Sube la colisión.
    /// </summary>
    public void Up()
    {
        if (box != null)
        {
            box.center += new Vector3(0, blockLength / transform.localScale.y, 0);
        }
    }

    /// <summary>
    /// Baja la colisión.
    /// </summary>
    public void Down()
    {
        if (box != null)
        {
            box.center -= new Vector3(0, blockLength / transform.localScale.y, 0);
        }
    }

    /// <summary>
    /// Resetea la colisión.
    /// </summary>
    public void ResetBox()
    {
        if (box != null)
        {
            box.center = new Vector3(0, 0, 0);
        }
    }

    /// <summary>
    /// Retorna la posición X.
    /// </summary>
    public int SurfacePointPositionX
    {
        get
        {
            return mapPos[0];
        }
    }

    /// <summary>
    /// Retorna la posición Z.
    /// </summary>
    public int SurfacePointPositionZ
    {
        get
        {
            return mapPos[2];
        }
    }

    /// <summary>
    /// Start.
    /// </summary>
    private void Start()
    {
        box = GetComponent<BoxCollider>();
    }

    /// <summary>
    /// OnDrawGizmos.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.01f);
    }
}
