// MsgPlaySfxAtPoint.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// Reproduce un SFX en un punto.
/// </summary>
public class MsgPlaySfxAtPoint
{
    /// <summary>
    /// El clip.
    /// </summary>
    public AudioClip clip;

    /// <summary>
    /// El volumen.
    /// </summary>
    public float volume;

    /// <summary>
    /// Punto donde tiene que reproducirse.
    /// </summary>
    public Vector3 point;

    /// <summary>
    /// Inicializa el mensaje.
    /// </summary>
    /// <param name="clip">El clip.</param>
    /// <param name="volume">El volumen.</param>
    /// <param name="point">La posición.</param>
    public MsgPlaySfxAtPoint(AudioClip clip, float volume, Vector3 point)
    {
        this.clip = clip;
        this.volume = volume;
        this.point = point;
    }
}