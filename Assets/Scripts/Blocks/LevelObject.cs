// LevelObject.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// Define la clase <see cref="LevelObject" /> que son los objetos del juego tales como items y bloques.
/// </summary>
public abstract class LevelObject : MonoBehaviour
{
    /// <summary>
    /// Identificadores de los bloques de juego.
    /// </summary>
    public enum Blocks
    {
        NoBlock = 0,
        WaterBlock = 1,
        LavaBlock = 2,
        SolidBlock = 3,
        LiftBlock = 4,
        SpikesBlock = 5,
        IceBlock = 6
    };

    /// <summary>
    /// Identificadores de los items.
    /// </summary>
    public enum Items
    {
        PlankItem = 25,
        FanItem = 26,
        FlagItem = 27,
        ActivatorItem = 28,
        BombItem = 29
    }

    /// <summary>
    /// Identificadores de los efectos de un item.
    /// </summary>
    public enum Effects
    {
        None,
        Freeze,
        Destroy,
        Activate
    };

    /// <summary>
    /// Animator de este bloque o item.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Retorna el animator.
    /// </summary>
    public Animator _Animator { get => animator; }

    /// <summary>
    /// Acción de colocar el bloque.
    /// </summary>
    public abstract void Place();

    /// <summary>
    /// Acción de destruir el bloque.
    /// </summary>
    public abstract void Destroy();

    /// <summary>
    /// Resetea el animator.
    /// </summary>
    protected void RebindAnimator()
    {
        if (animator != null)
        {
            animator.Rebind();
        }
    }

    /// <summary>
    /// Ejecuta un trigger en el animator del objeto.
    /// </summary>
    /// <param name="trigger">El trigger <see cref="string"/>.</param>
    public void SetAnimationTrigger(in string trigger)
    {
        if (animator != null)
        {
            animator.SetTrigger(trigger);
        }
    }

    /// <summary>
    /// Start.
    /// </summary>
    public void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogWarning("Animator not set in " + ToString);
        }
    }

    /// <summary>
    /// ¿Es un bloque?
    /// </summary>
    /// <returns>True si lo es, false si no <see cref="bool"/>.</returns>
    public bool IsBlock()
    {
        if (this.GetType() == typeof(Block))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// ¿Es un item?
    /// </summary>
    /// <returns>True si lo es, false si no <see cref="bool"/>.</returns>
    public bool IsItem()
    {
        if (this.GetType() == typeof(Item))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// ToString.
    /// </summary>
    public abstract new string ToString { get; }
}