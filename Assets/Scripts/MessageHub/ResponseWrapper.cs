// ResponseWrapper.cs
// Furious Koalas S.L.
// 2023

/// <summary>
/// Clase para generar respuesta a un mensaje.
/// </summary>
/// <typeparam name="TPetition">.</typeparam>
/// <typeparam name="TResponse">.</typeparam>
public class ResponseWrapper<TPetition, TResponse>
{
    /// <summary>
    /// Retorna la petición.
    /// </summary>
    public TPetition Petition { get; }

    /// <summary>
    /// Retorna la respuesta.
    /// </summary>
    public TResponse Response { get; }

    /// <summary>
    /// Inicializa una instancia de la clase <see cref="ResponseWrapper{TPetition, TResponse}"/>.
    /// </summary>
    /// <param name="petition">Mensaje al que se responde.</param>
    /// <param name="response">La respuesta.</param>
    public ResponseWrapper(TPetition petition, TResponse response)
    {
        this.Petition = petition;
        this.Response = response;
    }
}