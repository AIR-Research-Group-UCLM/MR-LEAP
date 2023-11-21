// MsgPlaySfx.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// Reproduce un SFX.
/// </summary>
public class MsgPlaySfx
{
    /// <summary>
    /// El clip
    /// </summary>
    public AudioClip clip;

    /// <summary>
    /// El volumen.
    /// </summary>
    public float volume;

    /// <summary>
    /// Inicializa la clase.
    /// </summary>
    /// <param name="clip">El clip.</param>
    /// <param name="volume">El volumen.</param>
    public MsgPlaySfx(AudioClip clip, float volume)
    {
        this.clip = clip;
        this.volume = volume;
    }
}