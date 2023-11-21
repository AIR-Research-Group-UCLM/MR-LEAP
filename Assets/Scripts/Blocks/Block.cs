// Block.cs
// Furious Koalas S.L.
// 2023

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Define la clase <see cref="Block" />.
/// </summary>
public class Block : LevelObject
{
    /// <summary>
    /// Atributo que guarda qué tipo de bloque es la instancia de la clase.
    /// </summary>
    [SerializeField] private Blocks blockType;

    /// <summary>
    /// Radio de la esfera que marca el punto superficial del bloque (gizmo).
    /// </summary>
    [Range(0f, 100f)]
    [SerializeField] private float surfaceSphereGizmoRadious = 0f;

    /// <summary>
    /// Retorna el tipo del bloque.
    /// </summary>
    public Blocks BlockType { get => blockType; }

    /// <summary>
    /// Enum de acciones que soporta el bloque.
    /// </summary>
    public enum BlockActions
    {
        Use,
        Destroy,
        Place,
        Activate,
        Rebind
    }

    /// <summary>
    /// Enum de propiedades que pueden tener los bloques
    /// </summary>
    public enum BlockProperties
    {
        Immaterial,
        Walkable,
        Dangerous,
        Icy,
        Destructible,
        Usable,
        Freezable
    }

    /// <summary>
    /// ToString
    /// </summary>
    public override string ToString { get => blockType.ToString() + " block"; }

    /// <summary>
    /// Retorna o modifica las propiedades del bloque.
    /// </summary>
    public BlockProperties[] _BlockProperties { get => blockProperties; set => blockProperties = value; }

    /// <summary>
    /// Retorna o modifica las reacciones del bloque a efectos.
    /// </summary>
    public EffectReaction[] EffectReactions { get => effectReaction; set => effectReaction = value; }

    /// <summary>
    /// La clase <see cref="EffectReaction" /> modela las reacciones que pueden tener los items sobre este bloque.
    /// </summary>
    [System.Serializable]
    public class EffectReaction
    {
        /// <summary>
        /// Items compatibles con este efecto. Si hay 0 compatible items se activara con cualquier item.
        /// </summary>
        public Items[] compatibleItems = new Items[0];

        /// <summary>
        /// El efecto en cuestión.
        /// </summary>
        public Effects effect;

        /// <summary>
        /// ¿Hay que cambiar este bloque por otro tras ejecutar el efecto?
        /// </summary>
        public bool replaceBlock = false;

        /// <summary>
        /// Por qué bloque se cambiará.
        /// </summary>
        public Blocks block;

        /// <summary>
        /// Acciones a ejecutar durante el efecto.
        /// </summary>
        public BlockActions[] actionsToExecute = new BlockActions[0];

        /// <summary>
        /// Nuevas propiedades del bloque.
        /// </summary>
        public BlockProperties[] newProperties = new BlockProperties[0];

        /// <summary>
        /// Triggers de animación que se pueden ejecutar.
        /// </summary>
        public string[] animationTriggers = new string[0];
    }

    /// <summary>
    /// Propiedades de este bloque.
    /// </summary>
    [SerializeField] private BlockProperties[] blockProperties = new BlockProperties[0];

    /// <summary>
    /// Reacciones de este bloque a los efectos.
    /// </summary>
    [SerializeField] private EffectReaction[] effectReaction = new EffectReaction[0];

    /// <summary>
    /// Offset de la superficie del bloque respecto a su posición global.
    /// </summary>
    [SerializeField] private Vector3 surfaceOffset = new Vector3(0, 1, 0);

    /// <summary>
    /// Diccionario de acciones.
    /// </summary>
    private Dictionary<BlockActions, Action> actionsDictionary;

    /// <summary>
    /// Awake.
    /// </summary>
    private void Awake()
    {
        actionsDictionary = new Dictionary<BlockActions, Action>();
        actionsDictionary.Add(BlockActions.Use, Use);
        actionsDictionary.Add(BlockActions.Destroy, Destroy);
        actionsDictionary.Add(BlockActions.Place, Place);
        actionsDictionary.Add(BlockActions.Activate, Activate);
        actionsDictionary.Add(BlockActions.Rebind, RebindAnimator);
    }

    /// <summary>
    /// OnDrawGizmos.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(SurfacePoint, surfaceSphereGizmoRadious);
    }

    /// <summary>
    /// Calcula el centro de la superficie del bloque.
    /// </summary>
    public Vector3 SurfacePoint
    {
        get
        {
            Vector3 defOffset;
            defOffset.x = surfaceOffset.x * transform.localScale.x;
            defOffset.y = surfaceOffset.y * transform.localScale.z;
            defOffset.z = surfaceOffset.z * transform.localScale.y;
            return defOffset + transform.position;
        }
    }

    /// <summary>
    /// Ejecuta una acción.
    /// </summary>
    /// <param name="action">La acción a ejecutar <see cref="BlockActions"/>.</param>
    public void ExecuteAction(BlockActions action)
    {
        if (actionsDictionary.ContainsKey(action))
        {
            actionsDictionary[action]();
        }
        else
        {
            Debug.LogWarning("Unbinded BlockAction: " + action.ToString());
        }
    }

    /// <summary>
    /// Acción usar.
    /// </summary>
    public void Use()
    {
        SetAnimationTrigger("Use");
    }

    /// <summary>
    /// Acción destruir.
    /// </summary>
    public override void Destroy()
    {
        BlockExploder b = GetComponent<BlockExploder>();
        if (b != null)
        {
            b.Explode();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Acción de colocar el bloque.
    /// </summary>
    public override void Place()
    {
    }

    /// <summary>
    /// Acción de activar el bloque.
    /// </summary>
    public void Activate()
    {
    }

    /// <summary>
    /// Comprueba que el bloque cumpla una propiedad.
    /// </summary>
    /// <param name="property">La propiedad <see cref="BlockProperties"/>.</param>
    /// <returns>True si la cumple, false si no <see cref="bool"/>.</returns>
    public bool CheckProperty(BlockProperties property)
    {
        foreach (BlockProperties prop in blockProperties)
        {
            if (prop == property)
            {
                return true;
            }
        }

        return false;
    }
}