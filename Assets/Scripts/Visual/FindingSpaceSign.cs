// FindingSpaceSign.cs
// Furious Koalas S.L.
// 2023

using System.Collections;
using UnityEngine;

/// <summary>
/// Cartel que indica que se está buscando el espacio del jugador.
/// </summary>
public class FindingSpaceSign : MonoBehaviour
{
    /// <summary>
    /// Puntos suspensivos.
    /// </summary>
    [SerializeField] private GameObject[] points = new GameObject[0];

    /// <summary>
    /// Tiempo hasta que se pasa al siguiente punto.
    /// </summary>
    [SerializeField] private float time = 1f;

    /// <summary>
    /// Referencia a la corrutina de animación.
    /// </summary>
    private IEnumerator findingSpace = null;

    /// <summary>
    /// Awake.
    /// </summary>
    private void Awake()
    {
        EventAggregator.Instance.Subscribe<MsgFindingSpace>(FindingSpace);
    }

    /// <summary>
    /// Activa la animación.
    /// </summary>
    /// <param name="msg">El mensaje <see cref="MsgFindingSpace"/>.</param>
    private void FindingSpace(MsgFindingSpace msg)
    {
        if (!msg.isFindingSpace)
        {
            if (findingSpace != null)
            {
                StopCoroutine(findingSpace);
                findingSpace = null;
            }
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        else
        {
            if (findingSpace == null)
            {
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(true);
                }

                findingSpace = FindingSpaceCrt();
                StartCoroutine(findingSpace);
            }
        }
    }

    /// <summary>
    /// La corrutina de la animación.
    /// </summary>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator FindingSpaceCrt()
    {
        bool[] activePoints = new bool[points.Length];
        for (int i = 0; i < activePoints.Length; i++)
        {
            activePoints[i] = false;
        }
        yield return new WaitForSeconds(time);
        int index = 0;
        while (true)
        {
            if (index < activePoints.Length)
            {
                activePoints[index] = true;
                index++;
            }
            else
            {
                index = 0;
                for (int i = 0; i < activePoints.Length; i++)
                {
                    activePoints[i] = false;
                }
            }

            EnableOrDisablePoints(activePoints, points);
            yield return new WaitForSeconds(time);
        }
    }

    /// <summary>
    /// Dado un array de bool activa los puntos específicos.
    /// </summary>
    /// <param name="activePoints">True los que tienen que estar activos, false los que no.</param>
    /// <param name="points">Los puntos.</param>
    private void EnableOrDisablePoints(bool[] activePoints, GameObject[] points)
    {
        for (int i = 0; i < activePoints.Length; i++)
        {
            if (activePoints[i])
            {
                points[i].SetActive(true);
            }
            else
            {
                points[i].SetActive(false);
            }
        }
    }
}