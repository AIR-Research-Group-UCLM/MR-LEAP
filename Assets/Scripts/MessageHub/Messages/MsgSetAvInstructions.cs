// MsgSetAvInstructions.cs
// Furious Koalas S.L.
// 2023

/// <summary>
/// Cambia los contadores de instrucciones restantes.
/// </summary>
public class MsgSetAvInstructions
{
    /// <summary>
    /// Instrucciones restantes.
    /// </summary>
    public AvailableInstructions avInst;

    /// <summary>
    /// Inicializa la clase.
    /// </summary>
    /// <param name="avInst">Instrucciones restantes.</param>
    public MsgSetAvInstructions(AvailableInstructions avInst)
    {
        this.avInst = avInst;
    }
}