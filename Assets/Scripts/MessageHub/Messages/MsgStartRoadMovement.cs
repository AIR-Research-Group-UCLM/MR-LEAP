// MsgStartRoadMovement.cs
// Furious Koalas S.L.
// 2023

/// <summary>
/// Mensaje para que el robot pequeño empiece a moverse.
/// </summary>
public class MsgStartRoadMovement
{
    /// <summary>
    /// Input inicial.
    /// </summary>
    public RoadInput input;

    /// <summary>
    /// Output final.
    /// </summary>
    public RoadOutput output;

    /// <summary>
    /// Inicializa una instancia del mensaje.
    /// </summary>
    /// <param name="input">El input.</param>
    /// <param name="output">El output.</param>
    public MsgStartRoadMovement(RoadInput input, RoadOutput output)
    {
        this.input = input;
        this.output = output;
    }
}