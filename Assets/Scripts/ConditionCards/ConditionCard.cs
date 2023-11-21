// ConditionCard.cs
// Furious Koalas S.L.
// 2023

using System.Collections;
using UnityEngine;
using static Block;

/// <summary>
/// Clase de las cartas de condición.
/// </summary>
public class ConditionCard : MonoBehaviour
{
    /// <summary>
    /// Propiedad que debe cumplir el bloque para que resulte válida.
    /// </summary>
    [SerializeField] private BlockProperties condition;

    /// <summary>
    /// ¿Se comprueba el bloque de justo enfrente?
    /// </summary>
    [SerializeField] private bool checkFrontBlock = false;

    /// <summary>
    /// Posición final de la animación de la carta.
    /// </summary>
    [SerializeField] private Transform endPosition;

    /// <summary>
    /// Velocidad de la animación de la carta.
    /// </summary>
    [Range(0.001f, 1000f)]
    [SerializeField] private float speed = 0.2f;

    /// <summary>
    /// Delegate para informar de que la carta ha sido tocada.
    /// </summary>
    /// <param name="card">La carta tocada.</param>
    public delegate void TappedCard(ConditionCard card);

    /// <summary>
    /// Instancia del delegate para informar de que la carta ha sido tocada.
    /// </summary>
    private TappedCard informOnTap;

    /// <summary>
    /// Retorna la condición de la carta.
    /// </summary>
    public BlockProperties Condition { get => condition; }

    /// <summary>
    /// Retorna un valor indicando si hay que comprobar el bloque de enfrente.
    /// </summary>
    public bool CheckFrontBlock { get => checkFrontBlock; }

    /// <summary>
    /// Indica si las acciones han terminado.
    /// </summary>
    public bool ActionsFinished { get => actionsFinished; }

    /// <summary>
    /// Añade métodos al delegado.
    /// </summary>
    public TappedCard InformOnTap { get => informOnTap; set => informOnTap += value; }

    /// <summary>
    /// Transform del marco de la carta.
    /// </summary>
    private Transform childTransform;

    /// <summary>
    /// Awake.
    /// </summary>
    private void Awake()
    {
        ConditionCardFrame child = GetComponentInChildren<ConditionCardFrame>();
        if (child != null)
        {
            child.tappedFrameDelegate += TappedFrame;
            childTransform = child.transform;
        }
    }

    /// <summary>
    /// ¿Han terminado todas las acciones?
    /// </summary>
    private bool actionsFinished = true;

    /// <summary>
    /// Despliega la carta.
    /// </summary>
    public void ShowCard()
    {
        StartCoroutine(ShowCardCrt());
    }

    /// <summary>
    /// Esconde la carta.
    /// </summary>
    public void HideCard()
    {
        StartCoroutine(HideCardCrt());
    }

    /// <summary>
    /// Se ha tocado la carta.
    /// </summary>
    private void TappedFrame()
    {
        informOnTap(this);
    }

    /// <summary>
    /// Animación de mostrar la carta.
    /// </summary>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator ShowCardCrt()
    {
        actionsFinished = false;
        if (childTransform != null)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = endPosition.position;
            float distance = Vector3.Distance(endPos, startPos);
            for (float i = 0; i <= 1;)
            {
                startPos = transform.position;
                endPos = endPosition.position;
                distance = Vector3.Distance(endPos, startPos);
                i += ((speed * Time.deltaTime) / distance);
                childTransform.transform.position = Vector3.Lerp(startPos, endPos, i);
                yield return null;
            }
            childTransform.transform.position = endPos;
        }
        actionsFinished = true;
    }

    /// <summary>
    /// Animación de esconder la carta.
    /// </summary>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator HideCardCrt()
    {
        actionsFinished = false;
        if (childTransform != null)
        {
            Vector3 startPos = endPosition.position;
            Vector3 endPos = transform.position;
            float distance = Vector3.Distance(endPos, startPos);
            for (float i = 0; i <= 1;)
            {
                startPos = endPosition.position;
                endPos = transform.position;
                distance = Vector3.Distance(endPos, startPos);
                i += ((speed * Time.deltaTime) / distance);
                childTransform.transform.position = Vector3.Lerp(startPos, endPos, i);
                yield return null;
            }
            childTransform.transform.position = endPos;
        }
        actionsFinished = true;
    }

    /// <summary>
    /// OnDrawGizmos.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (endPosition != null)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawSphere(endPosition.position, 0.001f);
        }
    }
}