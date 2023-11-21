// ArrowAnim.cs
// Furious Koalas S.L.
// 2023

using System.Collections;
using UnityEngine;

/// <summary>
/// Clase que lleva a cabo la animación de las flechas del mapa.
/// </summary>
public class ArrowAnim : MonoBehaviour
{
    /// <summary>
    /// Escala inicial.
    /// </summary>
    private Vector3 originalScale;

    /// <summary>
    /// Escala final.
    /// </summary>
    private Vector3 reducedScale;

    /// <summary>
    /// Porcentaje de tamaño que debe disminuir el objeto.
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float reducedPercent = 0.5f;

    /// <summary>
    /// Velocidad de la animación.
    /// </summary>
    [Range(0.0f, 100.0f)]
    public float speed = 10f;

    /// <summary>
    /// Multiplicador de la escala.
    /// </summary>
    [Range(0.0f, 100f)]
    public float scaleMultiplier = 20f;

    /// <summary>
    /// Start.
    /// </summary>
    private void Start()
    {
        originalScale = new Vector3(scaleMultiplier, scaleMultiplier, scaleMultiplier);
        gameObject.transform.localScale = originalScale;
        reducedScale = originalScale * reducedPercent;
        StartCoroutine(Minimize());
    }

    /// <summary>
    /// OnEnable.
    /// </summary>
    private void OnEnable()
    {
        originalScale = new Vector3(scaleMultiplier, scaleMultiplier, scaleMultiplier);
        gameObject.transform.localScale = originalScale;
        reducedScale = originalScale * reducedPercent;
        StartCoroutine(Minimize());
    }

    /// <summary>
    /// Minimiza el objeto.
    /// </summary>
    /// <returns>The <see cref="IEnumerator"/>.</returns>
    private IEnumerator Minimize()
    {
        float distance = Vector3.Distance(originalScale, reducedScale);
        for (float i = 0; i <= 1;)
        {
            i += ((speed * Time.deltaTime) / distance);
            transform.localScale = Vector3.Lerp(originalScale, reducedScale, i);
            yield return null;
        }
        transform.localScale = reducedScale;
        yield return null;
        StartCoroutine(Maximize());
    }

    /// <summary>
    /// Maximiza el objeto.
    /// </summary>
    /// <returns>The <see cref="IEnumerator"/>.</returns>
    private IEnumerator Maximize()
    {
        float distance = Vector3.Distance(originalScale, reducedScale);
        for (float i = 0; i <= 1;)
        {
            i += ((speed * Time.deltaTime) / distance);
            transform.localScale = Vector3.Lerp(reducedScale, originalScale, i);
            yield return null;
        }
        transform.localScale = originalScale;
        yield return null;
        StartCoroutine(Minimize());
    }
}