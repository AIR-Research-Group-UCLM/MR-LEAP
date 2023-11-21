// RoadButton.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;
using static LevelButtons;

/// <summary>
/// Botón para colocar una carretera.
/// </summary>
public class RoadButton : MonoBehaviour
{
    /// <summary>
    /// Tipo del botón.
    /// </summary>
    [SerializeField] private Buttons buttonIndex = Buttons.Undefined;

    /// <summary>
    /// Click del botón.
    /// </summary>
    [SerializeField] private AudioClip buttonClick;

    /// <summary>
    /// Mesh del botón.
    /// </summary>
    public GameObject mesh;

    /// <summary>
    /// Animación del botón.
    /// </summary>
    private Animation anim;

    /// <summary>
    /// ¿Está el botón activo?
    /// </summary>
    private bool enable = true;

    /// <summary>
    /// Retorna el tipo del botón.
    /// </summary>
    public Buttons ButtonType
    {
        get
        {
            return buttonIndex;
        }
    }

    /// <summary>
    /// Awake.
    /// </summary>
    private void Awake()
    {
        if (mesh != null)
        {
            anim = mesh.GetComponent<Animation>();
        }
    }

    /// <summary>
    /// OnSelect.
    /// </summary>
    public void OnSelect()
    {
        if (enable)
        {
            Debug.Log("Pressed " + buttonIndex.ToString("g"));
            RoadPlacementLogic.Instance.AddInputFromButton(buttonIndex);

            if (mesh != null)
            {
                if (anim.isPlaying)
                {
                    return;
                }
                else
                {
                    anim.Play("ButtonPressed");
                }
            }

            EventAggregator.Instance.Publish<MsgPlaySfxAtPoint>(new MsgPlaySfxAtPoint(buttonClick, 1.0f, transform.position));
        }
    }

    /// <summary>
    /// Desactiva el botón.
    /// </summary>
    public void Disable()
    {
        enable = false;
    }

    /// <summary>
    /// Activa el botón.
    /// </summary>
    public void Enable()
    {
        enable = true;
    }
}