// Item.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;

/// <summary>
/// Define la clase <see cref="Item" />.
/// </summary>
public class Item : LevelObject
{
    /// <summary>
    /// Guarda el tipo del item de esta instancia.
    /// </summary>
    [SerializeField] private Items itemType;

    /// <summary>
    /// Retorna el tipo del item de esta instancia.
    /// </summary>
    public Items ItemType { get => itemType; }

    /// <summary>
    /// Indica si el item se puede recoger o no.
    /// </summary>
    public bool Pickable { get => pickable; }

    /// <summary>
    /// ToString.
    /// </summary>
    public override string ToString { get => itemType.ToString() + " item"; }

    /// <summary>
    /// Efecto que provoca este item.
    /// </summary>
    public Effects Effect { get => effect; }

    /// <summary>
    /// Indica si al usarlo se tiene que convertir en hijo del bloque sobre el que se usa.
    /// </summary>
    public bool ParentToBlockParent { get => parentToBlockParent; }

    /// <summary>
    /// Indica si hay que usarlo en el bloque de enfrente.
    /// </summary>
    public bool UseOnFrontBlock { get => useOnFrontBlock; set => useOnFrontBlock = value; }

    /// <summary>
    /// Indica si hay que usarlo en el bloque de enfrente y abajo
    /// </summary>
    public bool UseOnFrontBelowBlock { get => useOnFrontBelowBlock; set => useOnFrontBelowBlock = value; }

    /// <summary>
    /// ¿Se tiene que usar en la mano del jugador?
    /// </summary>
    public bool UseOnPlayersHand { get => useOnPlayersHand; set => useOnPlayersHand = value; }

    /// <summary>
    /// Distancia a la que tiene que seguir al jugador.
    /// </summary>
    public Vector3 FollowOffset { get => followOffset; set => followOffset = value; }

    /// <summary>
    /// ¿Se puede coger?.
    /// </summary>
    [SerializeField] private bool pickable;

    /// <summary>
    /// ¿Se tiene que hacer padre del bloque sobre el que se usa?.
    /// </summary>
    [SerializeField] private bool parentToBlockParent;

    /// <summary>
    /// Efecto del bloque.
    /// </summary>
    [SerializeField] private Effects effect;

    /// <summary>
    /// ¿Hay que usarlo en el bloque de enfrente?
    /// </summary>
    [SerializeField] private bool useOnFrontBlock;

    /// <summary>
    /// ¿Hay que usarlo en el bloque de enfrente y abajo?
    /// </summary>
    [SerializeField] private bool useOnFrontBelowBlock;

    /// <summary>
    /// ¿Hay que usarlo en la mano del jugador?
    /// </summary>
    [SerializeField] private bool useOnPlayersHand;

    /// <summary>
    /// ¿Se vuelve inactivo al usarse?
    /// </summary>
    [SerializeField] private bool inactiveOnUse = true;

    /// <summary>
    /// Transform que sigue al colocarse en el inventario.
    /// </summary>
    private Transform transformToFollow;

    /// <summary>
    /// Distancia con la que sigue al objetivo.
    /// </summary>
    private Vector3 followOffset;

    /// <summary>
    /// Update.
    /// </summary>
    private void Update()
    {
        if (transformToFollow != null)
        {
            transform.position = Vector3.Lerp(transform.position, transformToFollow.position + followOffset, 1);
        }
    }

    /// <summary>
    /// Acción de usar el item.
    /// </summary>
    public void Use()
    {
        transformToFollow = null;
        if (inactiveOnUse)
        {
            gameObject.SetActive(false);
        }
        else
        {
            SetAnimationTrigger("Use");
        }
    }

    /// <summary>
    /// Acción de tomar el item.
    /// </summary>
    /// <param name="transformToFollow">Objetivo que tiene que seguir el item <see cref="Transform"/>.</param>
    /// <param name="followOffset">Distancia a la que seguir al objetivo <see cref="Vector3"/>.</param>
    public void Pick(Transform transformToFollow, Vector3 followOffset)
    {
        this.transformToFollow = transformToFollow;
        this.followOffset = followOffset;
    }

    /// <summary>
    /// Acción de destruir el bloque.
    /// </summary>
    public override void Destroy()
    {
    }

    /// <summary>
    /// Acción de colocar el bloque.
    /// </summary>
    public override void Place()
    {
    }
}