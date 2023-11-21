// RoadIO.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// Entradas y salidas de una carretera.
/// </summary>
public abstract class RoadIO : MonoBehaviour
{
    /// <summary>
    /// Dirección a la que apunta la instancia.
    /// </summary>
    public enum IODirection
    {
        Forward = 0,
        Back = 1,
        Left = 2,
        Right = 3,
        Undefined
    };

    /// <summary>
    /// Retorna el color del gizmo.
    /// </summary>
    /// <returns><see cref="Color"/>.</returns>
    public abstract Color Color();

    /// <summary>
    /// Dirección a la que apunta la instancia.
    /// </summary>
    [SerializeField] private IODirection pointsTo = IODirection.Forward;

    /// <summary>
    /// Retorna la dirección a la que apunta la instancia.
    /// </summary>
    public IODirection Direction { get => pointsTo; }

    /// <summary>
    /// A qué IO está conectada.
    /// </summary>
    private RoadIO connectedTo;

    /// <summary>
    /// Si se puede usar como IO seleccionada.
    /// </summary>
    [SerializeField] private bool canBeSelected = true;

    /// <summary>
    /// ID, aleatorio y único.
    /// </summary>
    [UniqueIdentifier, SerializeField] private string id;

    /// <summary>
    /// Retorna el ID.
    /// </summary>
    public string IOIdentifier
    {
        get
        {
            return id;
        }
    }

    /// <summary>
    /// Retorna a qué está conectada esta IO o la conecta con otra.
    /// </summary>
    public RoadIO ConnectedTo
    {
        get
        {
            return connectedTo;
        }

        set
        {
            if (value != null)
            {
                if (this.GetType() == value.GetType())
                {
                    Debug.LogWarning("Connecting IO of the same type");
                }
                connectedTo = value;
                value.connectedTo = this;
            }
            else
            {
                connectedTo = null;
            }
        }
    }

    /// <summary>
    /// Retorna si puede ser seleccionada.
    /// </summary>
    public bool CanBeSelected { get => canBeSelected; }

    /// <summary>
    /// Calcula la dirección opuesta a otra.
    /// </summary>
    /// <param name="direction">La dirección de entrada.</param>
    /// <returns>La <see cref="IODirection"/> de salida.</returns>
    public static IODirection GetOppositeDirection(IODirection direction)
    {
        switch (direction)
        {
            case IODirection.Forward:
                return IODirection.Back;

            case IODirection.Back:
                return IODirection.Forward;

            case IODirection.Left:
                return IODirection.Right;

            case IODirection.Right:
                return IODirection.Left;

            default:
                return IODirection.Undefined;
        }
    }

    /// <summary>
    /// OnDrawGizmos.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color();
        if (ConnectedTo != null)
        {
            Gizmos.color = UnityEngine.Color.magenta;
        }
        Gizmos.DrawSphere(transform.position, 0.002f);

        switch (pointsTo)
        {
            case IODirection.Forward:
                DrawArrow.ForGizmo(transform.position, Vector3.forward * 0.05f);
                break;

            case IODirection.Back:
                DrawArrow.ForGizmo(transform.position, Vector3.back * 0.05f);
                break;

            case IODirection.Left:
                DrawArrow.ForGizmo(transform.position, Vector3.left * 0.05f);
                break;

            case IODirection.Right:
                DrawArrow.ForGizmo(transform.position, Vector3.right * 0.05f);
                break;
        }
    }

    /// <summary>
    /// Mueve la carretera a un punto usando esta IO como centro.
    /// </summary>
    /// <param name="newPos">La nueva posición.</param>
    public void MoveRoadTo(in Vector3 newPos)
    {
        transform.parent.position = MoveRoadFromPoint(transform.position, newPos, transform.parent.position);
    }

    /// <summary>
    /// Mueve la carretera a un punto usando esta IO como centro.
    /// </summary>
    /// <param name="point">La posición de la IO.</param>
    /// <param name="newPositionOfPoint">La nueva posición de la carretera.</param>
    /// <param name="roadPosition">La posición de la carretera.</param>
    /// <returns>Punto al que se debe poner la carretera.</returns>
    private Vector3 MoveRoadFromPoint(in Vector3 point, in Vector3 newPositionOfPoint, in Vector3 roadPosition)
    {
        /*
         * Los struct (por ejemplo Vector3) se pasan siempre por valor, es decir que se copian.
         * No es necesario hacer "new" en los struct, de hecho es mucho más lento.
         * Para pasar por referencia y ahorrar el tiempo de copiarlos se pueden usar las palabras
         * in, ref y out.
         * ref -> requiere la variable inicializada.
         * out -> no hace falta inicializar la variable.
         * in -> requiere la variable inicializada pero no dejará que se ejecuten cambios sobre ella.
         *       (es decir, point.x=3 dará error).
         */
        Vector3 result;
        result.x = roadPosition.x + (newPositionOfPoint.x - point.x);
        result.y = roadPosition.y + (newPositionOfPoint.y - point.y);
        result.z = roadPosition.z + (newPositionOfPoint.z - point.z);
        return result;
    }

    /// <summary>
    /// Retorna la carretera padre.
    /// </summary>
    /// <returns>La carretera padre.</returns>
    public Road GetParentRoad()
    {
        return transform.parent.GetComponent<Road>();
    }
}