// VerticalButton.cs
// Furious Koalas S.L.
// 2023

using UnityEngine;
using static LevelButtons;

/// <summary>
/// Clase de los botones que activa el robot al moverse por las carreteras.
/// </summary>
public class VerticalButton : MonoBehaviour
{
    /// <summary>
    /// Mesh del objeto.
    /// </summary>
    [SerializeField] private GameObject mesh;

    /// <summary>
    /// Tipo del botón.
    /// </summary>
    [SerializeField] private Buttons buttonType;

    /// <summary>
    /// Animación de click.
    /// </summary>
    private Animation anim;

    /// <summary>
    /// Flag de bloqueo.
    /// </summary>
    private bool locked = false;

    /// <summary>
    /// False si no está bloqueado, true si lo está.
    /// </summary>
    public bool Locked
    {
        get { return locked; }
    }

    /// <summary>
    /// Retorna el tipo del botón.
    /// </summary>
    public string ButtonType
    {
        get { return buttonType.ToString(); }
    }

    /// <summary>
    /// Retorna el tipo del botón.
    /// </summary>
    public Buttons ButtonTypeE
    {
        get { return buttonType; }
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
    /// Activa el botón cuando el robot entra en contacto con él.
    /// </summary>
    /// <param name="other"><see cref="Collider"/> del objeto con el que se ha colisionado.</param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Pressed " + buttonType);
        //Quito el mensaje y lo cambio a una referencia directa ya que es muy importante que se ejecuten en
        //el orden correcto
        //EventAggregator.Instance.Publish(new MsgAddInputFromButton(buttonName));
        GameLogic.Instance.AddInputFromButton(buttonType);

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
    }

    /// <summary>
    /// Bloquea el botón.
    /// </summary>
    public void Lock()
    {
        this.locked = true;
    }

    /// <summary>
    /// Desbloquea el botón.
    /// </summary>
    public void Unlock()
    {
        this.locked = false;
    }
}