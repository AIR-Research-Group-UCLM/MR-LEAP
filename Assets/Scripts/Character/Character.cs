// Character.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// Clase abstracta de la que descienden los robots.
/// </summary>
public abstract class Character : MonoBehaviour
{
    /// <summary>
    /// El animator.
    /// </summary>
    [SerializeField] private Animator animator;

    /// <summary>
    /// Retorna el animator.
    /// </summary>
    public Animator Animator { get => animator; }

    /// <summary>
    /// Resetea el animator.
    /// </summary>
    public void RebindAnimator()
    {
        if (animator != null)
        {
            animator.Rebind();
        }
    }

    /// <summary>
    /// Ejecuta un trigger de animación.
    /// </summary>
    /// <param name="trigger">El trigger.</param>
    protected void SetAnimationTrigger(in string trigger)
    {
        if (animator != null)
        {
            animator.SetTrigger(trigger);
        }
    }
}