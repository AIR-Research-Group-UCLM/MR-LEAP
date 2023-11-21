// SoundEngine.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// Pequeña clase con métodos relativos al sonido.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SoundEngine : MonoBehaviour
{
    /// <summary>
    /// AudioSource.
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Awake.
    /// </summary>
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        EventAggregator.Instance.Subscribe<MsgPlaySfx>(PlaySfx);
        EventAggregator.Instance.Subscribe<MsgPlaySfxAtPoint>(PlaySfxAtPoint);
    }

    /// <summary>
    /// Reproduce un SFX.
    /// </summary>
    /// <param name="msg">El mensaje <see cref="MsgPlaySfx"/>.</param>
    private void PlaySfx(MsgPlaySfx msg)
    {
        audioSource.PlayOneShot(msg.clip, msg.volume);
    }

    /// <summary>
    /// Reproduce un SFX en un punto.
    /// </summary>
    /// <param name="msg">El mensaje <see cref="MsgPlaySfxAtPoint"/>.</param>
    private void PlaySfxAtPoint(MsgPlaySfxAtPoint msg)
    {
        AudioSource.PlayClipAtPoint(msg.clip, msg.point, msg.volume);
    }
}