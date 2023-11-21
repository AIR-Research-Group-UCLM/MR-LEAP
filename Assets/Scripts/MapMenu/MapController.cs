// MapController.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// Contiene una serie de métodos utiles para activar los controles de las diferentes pantallas del juego.
/// </summary>
public class MapController : MonoBehaviour
{
    /// <summary>
    /// Posición del mapa donde se instanciarán los niveles del juego.
    /// </summary>
    [SerializeField] private Transform center;

    /// <summary>
    /// Flecha derecha del selector de mapas.
    /// </summary>
    [SerializeField] private GameObject arrowR;

    /// <summary>
    /// Flecha izquierda del selector de mapas.
    /// </summary>
    [SerializeField] private GameObject arrowL;

    /// <summary>
    /// Botones de selección de carretera.
    /// </summary>
    [SerializeField] private GameObject levelButtons;

    /// <summary>
    /// Padre de las carreteras.
    /// </summary>
    [SerializeField] private GameObject roadScaler;

    /// <summary>
    /// Superficie donde el usuario hará click para seleccionar un mapa.
    /// </summary>
    [SerializeField] private GameObject mapBounds;

    /// <summary>
    /// Botones del editor de niveles.
    /// </summary>
    [SerializeField] private GameObject editorButtons;

    /// <summary>
    /// Superficie del editor de niveles.
    /// </summary>
    [SerializeField] private GameObject editorSurface;

    /// <summary>
    /// ¿Mostrar el gizmo de las cajas de colisión?
    /// </summary>
    [SerializeField] private bool drawCollidersGizmo;

    /// <summary>
    /// OnDrawGizmos.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (drawCollidersGizmo)
        {
            BoxCollider r = GetComponent<BoxCollider>();
            if (r != null)
            {
                Gizmos.DrawWireCube(r.bounds.center, r.bounds.size);
            }

            BoxCollider[] childColliders = GetComponentsInChildren<BoxCollider>();
            foreach (BoxCollider box in childColliders)
            {
                Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);
            }
        }

        Gizmos.DrawSphere(MapControllerCenter, 0.03f);
    }

    /// <summary>
    /// Activa los controles del modo juego.
    /// </summary>
    public void EnableGameControls()
    {
        levelButtons.gameObject.SetActive(true);
        roadScaler.gameObject.SetActive(true);
        mapBounds.gameObject.SetActive(false);
        arrowR.gameObject.SetActive(false);
        arrowL.gameObject.SetActive(false);
        editorButtons.gameObject.SetActive(false);
        editorSurface.gameObject.SetActive(false);
    }

    /// <summary>
    /// Activa los controles del menú de mapas.
    /// </summary>
    public void EnableMenuControls()
    {
        levelButtons.gameObject.SetActive(false);
        roadScaler.gameObject.SetActive(false);
        mapBounds.gameObject.SetActive(true);
        arrowR.gameObject.SetActive(true);
        arrowL.gameObject.SetActive(true);
        editorButtons.gameObject.SetActive(false);
        editorSurface.gameObject.SetActive(false);
    }

    /// <summary>
    /// Activa los controles de menú principal.
    /// </summary>
    public void EnableMainMenuControls()
    {
        levelButtons.gameObject.SetActive(false);
        roadScaler.gameObject.SetActive(false);
        mapBounds.gameObject.SetActive(false);
        arrowR.gameObject.SetActive(false);
        arrowL.gameObject.SetActive(false);
        editorButtons.gameObject.SetActive(false);
        editorSurface.gameObject.SetActive(false);
    }

    /// <summary>
    /// Activa los controles del editor.
    /// </summary>
    public void EnableEditorControls()
    {
        editorButtons.gameObject.SetActive(true);
        editorSurface.gameObject.SetActive(true);
        levelButtons.gameObject.SetActive(false);
        roadScaler.gameObject.SetActive(false);
        mapBounds.gameObject.SetActive(false);
        arrowR.gameObject.SetActive(false);
        arrowL.gameObject.SetActive(false);
    }

    /// <summary>
    /// Retorna el punto donde deben aparecer los mapas.
    /// </summary>
    public Vector3 MapControllerCenter
    {
        get
        {
            if (center != null)
            {
                return center.position;
            }
            else
            {
                return transform.position;
            }
        }
    }
}