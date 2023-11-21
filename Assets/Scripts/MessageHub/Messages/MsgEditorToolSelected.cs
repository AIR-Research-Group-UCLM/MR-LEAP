// MsgEditorToolSelected.cs
// Furious Koalas S.L.
// 2023

using static EditorLogic;

/// <summary>
/// Mensaje para seleccionar una herramienta del editor.
/// </summary>
public class MsgEditorToolSelected
{
    /// <summary>
    /// Tipo de herramienta.
    /// </summary>
    private EditorToolType toolType;

    /// <summary>
    /// Parámetro extra.
    /// </summary>
    private int toolIdentifier;

    /// <summary>
    /// Retorna el tipo de herramienta.
    /// </summary>
    public EditorToolType ToolType { get => toolType; set => toolType = value; }

    /// <summary>
    /// Retorna el parámetro extra.
    /// </summary>
    public int ToolIdentifier { get => toolIdentifier; set => toolIdentifier = value; }

    /// <summary>
    /// Crea una instancia del mensaje.
    /// </summary>
    /// <param name="toolType">Tipo de herramienta.</param>
    /// <param name="toolIdentifier">Parámetro extra.</param>
    public MsgEditorToolSelected(EditorToolType toolType, int toolIdentifier)
    {
        this.toolType = toolType;
        this.toolIdentifier = toolIdentifier;
    }
}