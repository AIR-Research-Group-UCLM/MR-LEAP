// EditorInstructionButton.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;
using static LevelButtons;

/// <summary>
/// Clase usada en los botones de instrucciones del editor de niveles.
/// </summary>
public class EditorInstructionButton : MonoBehaviour
{
    /// <summary>
    /// Tipo de botón.
    /// </summary>
    [SerializeField] private Buttons buttonIndex = Buttons.Undefined;

    /// <summary>
    /// Audio del botón.
    /// </summary>
    [SerializeField] private AudioClip buttonClick;

    /// <summary>
    /// Contador de instrucciones.
    /// </summary>
    [SerializeField] private Counter counter;

    /// <summary>
    /// Mesh del objeto.
    /// </summary>
    [SerializeField] private GameObject mesh;

    /// <summary>
    /// Animación de click.
    /// </summary>
    private Animation anim;

    /// <summary>
    /// Retorna el tipo de botón.
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
        EventAggregator.Instance.Subscribe<MsgEditorResetAllCounters>(ResetCounter);
    }

    /// <summary>
    /// Pone el contador a cero.
    /// </summary>
    /// <param name="msg">El mensaje <see cref="MsgEditorResetAllCounters"/>.</param>
    public void ResetCounter(MsgEditorResetAllCounters msg)
    {
        if (counter != null)
        {
            counter.SetNumber(0);
        }
    }

    /// <summary>
    /// Ejecuta la acción del botón cuando el usuario hace tap.
    /// </summary>
    public void OnSelect()
    {
        if (counter != null)
        {
            counter.SetNumber(counter.ActualNumber + 1);
            EventAggregator.Instance.Publish<MsgEditorAvailableInstructionsChanged>(new MsgEditorAvailableInstructionsChanged(buttonIndex, counter.ActualNumber));

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

            if (buttonClick != null)
            {
                EventAggregator.Instance.Publish<MsgPlaySfxAtPoint>(new MsgPlaySfxAtPoint(buttonClick, 1.0f, transform.position));
            }
        }
    }
}