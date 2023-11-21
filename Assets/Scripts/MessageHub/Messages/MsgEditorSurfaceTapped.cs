// MsgEditorSurfaceTapped.cs
// Furious Koalas S.L.
// 2023

/// <summary>
/// Mensaje para avisar de que se ha tocado en la superficie del editor.
/// </summary>
public class MsgEditorSurfaceTapped
{
    /// <summary>
    /// Punto sobre el que se ha tocado.
    /// </summary>
    private EditorSurfacePoint tappedPoint;

    /// <summary>
    /// Inicializa una instancia del mensaje.
    /// </summary>
    /// <param name="tappedPoint">El punto sobre el que se ha tocado.</param>
    public MsgEditorSurfaceTapped(EditorSurfacePoint tappedPoint)
    {
        this.tappedPoint = tappedPoint;
    }

    /// <summary>
    /// Retorna el punto tocado.
    /// </summary>
    public EditorSurfacePoint TappedPoint { get => tappedPoint; }
}