// BlockExploder.cs
// Furious Koalas S.L.
// 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase que hace explotar un bloque como una serie de cubos.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class BlockExploder : MonoBehaviour
{
    /// <summary>
    /// Tamaño del cubo.
    /// </summary>
    public float cubeSize = 0.2f;

    /// <summary>
    /// Cubos en una fila.
    /// </summary>
    public int cubesInRow = 5;

    /// <summary>
    /// Duración de las partículas.
    /// </summary>
    public float particleDuration = 1f;

    /// <summary>
    /// Fuerza de la explosión.
    /// </summary>
    public float explosionForce = 50f;

    /// <summary>
    /// Radio de la explosión.
    /// </summary>
    public float explosionRadius = 4f;

    /// <summary>
    /// Hace que la explosión mueva los objetos hacia arriba.
    /// </summary>
    public float explosionUpward = 0.4f;

    /// <summary>
    /// Masa de las partículas.
    /// </summary>
    public float mass = 0.1f;

    /// <summary>
    /// Audio de la explosión.
    /// </summary>
    [SerializeField] private AudioClip explosionSfx;

    /// <summary>
    /// Material de las partículas.
    /// </summary>
    [SerializeField] private Material particleMaterial;

    /// <summary>
    /// Lista de partículas.
    /// </summary>
    private List<GameObject> pieces;

    /// <summary>
    /// Start.
    /// </summary>
    internal void Start()
    {
       /* Renderer r = GetComponent<Renderer>();
        if (r != null)
        {
            Material m = r.material;
            if (m != null)
            {
                particleMaterial = m;
            }
        }*/

        pieces = new List<GameObject>();
    }

    /// <summary>
    /// Hace explotar el bloque.
    /// </summary>
    public void Explode()
    {
        transform.position = new Vector3(transform.position.x, -1f, transform.position.z);
        //Creamos las partículas en las coordenadas x,y,z.
        for (int x = 0; x < cubesInRow; x++)
        {
            for (int y = 0; y < cubesInRow; y++)
            {
                for (int z = 0; z < cubesInRow; z++)
                {
                    CreateParticle(x, y, z);
                }
            }
        }
        GetComponent<Renderer>().enabled = false;
        //Posición de la explosión
        Vector3 explosionPos = transform.position;

        //Tomamos los objetos con colisión en el radio de la explosión.
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        //Se añade fuerza a todos esos objetos.
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
            }
        }

        if (explosionSfx != null)
        {
            EventAggregator.Instance.Publish<MsgPlaySfxAtPoint>(new MsgPlaySfxAtPoint(explosionSfx, 0.7f, transform.position));
        }

        StartCoroutine(DestroyParticlesAndGameobject(pieces, particleDuration));
    }

    /// <summary>
    /// Crea una partícula.
    /// </summary>
    /// <param name="x">Coordenada x.</param>
    /// <param name="y">Coordenada y.</param>
    /// <param name="z">Coordenada z.</param>
    private void CreateParticle(int x, int y, int z)
    {
        //Creamos la partícula.
        GameObject piece;

        piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pieces.Add(piece);
        piece.GetComponent<Renderer>().material = particleMaterial;
        //Ponemos su posición y escala.
        Vector3 cubesPivot = new Vector3(0, 1, 0);
        piece.transform.parent = transform;
        piece.transform.position = transform.position + new Vector3(cubeSize * x, cubeSize * y, cubeSize * z) - cubesPivot;
        piece.transform.rotation = transform.rotation;
        piece.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);

        //Se añade rigidbody y se pone la masa.
        piece.AddComponent<Rigidbody>();
        piece.GetComponent<Rigidbody>().mass = mass;
    }

    /// <summary>
    /// Corrutina para destruir las partículas pasado un tiempo.
    /// </summary>
    /// <param name="particles">Lista de partículas.</param>
    /// <param name="seconds">Segundos que pasan hasta que se destruyen.</param>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator DestroyParticlesAndGameobject(List<GameObject> particles, float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        foreach (GameObject g in particles)
        {
            g.SetActive(false);
        }
        yield return null;
        foreach (GameObject g in particles)
        {
            Destroy(g);
            yield return null;
        }

        Destroy(this.gameObject);
    }
}