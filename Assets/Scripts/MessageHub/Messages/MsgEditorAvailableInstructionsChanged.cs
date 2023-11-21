// MsgEditorAvailableInstructionsChanged.cs
// Furious Koalas S.L.
// 2023

using static LevelButtons;

/// <summary>
/// Mensaje para cambiar el contador de número de instrucciones disponibles.
/// </summary>
public class MsgEditorAvailableInstructionsChanged
{
    /// <summary>
    /// El botón que hay que cambiar.
    /// </summary>
    private Buttons button;

    /// <summary>
    /// Número de instrucciones.
    /// </summary>
    private int numberOfInstrucions;

    /// <summary>
    /// Constructor de la clase.
    /// </summary>
    /// <param name="button">El botón que hay que cambiar.</param>
    /// <param name="numberOfInstrucions">Número de instrucciones.</param>
    public MsgEditorAvailableInstructionsChanged(Buttons button, int numberOfInstrucions)
    {
        this.button = button;
        this.numberOfInstrucions = numberOfInstrucions;
    }

    /// <summary>
    /// Retorna el tipo de botón.
    /// </summary>
    public Buttons Button { get => button; set => button = value; }

    /// <summary>
    /// Retorna el número de instrucciones.
    /// </summary>
    public int NumberOfInstructions { get => numberOfInstrucions; set => numberOfInstrucions = value; }
}