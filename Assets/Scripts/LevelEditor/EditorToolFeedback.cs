// EditorToolFeedback.cs
// Furious Koalas S.L.
// 2023

using System.Collections;
using UnityEngine;

/// <summary>
/// Ejecuta una pequeña animación cuando se hace tap en un objeto.
/// </summary>
public class EditorToolFeedback : MonoBehaviour
{
    /// <summary>
    /// Escala original del objeto.
    /// </summary>
    private Vector3 originalScale;

    /// <summary>
    /// ¿Está el objeto preparado?
    /// </summary>
    private bool ready = true;

    /// <summary>
    /// Velocidad de la animación.
    /// </summary>
    [SerializeField] private float speed = 1f;

    /// <summary>
    /// Porcentaje objetivo de la escala.
    /// </summary>
    [Range(0.001f, 0.999f)]
    [SerializeField] private float scalePercent = 0.5f;

    /// <summary>
    /// OnEnable.
    /// </summary>
    private void OnEnable()
    {
        transform.localScale = originalScale;
    }

    /// <summary>
    /// OnDisable.
    /// </summary>
    private void OnDisable()
    {
        ready = true;
    }

    /// <summary>
    /// Awake.
    /// </summary>
    private void Awake()
    {
        originalScale = transform.localScale;
    }

    /// <summary>
    /// Ejecuta la animación cuando el usuario hace tap.
    /// </summary>
    public void OnSelect()
    {
        if (ready)
        {
            StartCoroutine(Animate());
        }
    }

    /// <summary>
    /// Corrutina que lleva a cabo la animación.
    /// </summary>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator Animate()
    {
        ready = false;
        Vector3 reducedScale = originalScale * scalePercent;
        transform.localScale = originalScale;
        float distance = Vector3.Distance(originalScale, reducedScale);
        for (float i = 0; i <= 1;)
        {
            i += ((speed * Time.deltaTime) / distance);
            transform.localScale = Vector3.Lerp(originalScale, reducedScale, i);
            yield return null;
        }
        transform.localScale = reducedScale;
        yield return null;
        distance = Vector3.Distance(originalScale, reducedScale);
        for (float i = 0; i <= 1;)
        {
            i += ((speed * Time.deltaTime) / distance);
            transform.localScale = Vector3.Lerp(reducedScale, originalScale, i);
            yield return null;
        }
        transform.localScale = originalScale;
        ready = true;
    }
}