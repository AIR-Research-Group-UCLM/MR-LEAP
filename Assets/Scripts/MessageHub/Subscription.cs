using System;

/// <summary>
/// Clase <see cref="Subscription<Tmessage>" /> que simboliza la subscripción de una acción a un EventAggregator.
/// Tomado y adaptado de
/// https://www.c-sharpcorner.com/UploadFile/pranayamr/publisher-or-subscriber-pattern-with-event-or-delegate-and-e/
/// </summary>
public class Subscription<Tmessage> : IDisposable
{
    public Action<Tmessage> Action { get; private set; }
    private readonly EventAggregator EventAggregator;
    private bool isDisposed;

    public Subscription(Action<Tmessage> action, EventAggregator eventAggregator)
    {
        Action = action;
        EventAggregator = eventAggregator;
    }

    ~Subscription()
    {
        if (!isDisposed)
            Dispose();
    }

    public void Dispose()
    {
        EventAggregator.Unsubscribe(this);
        isDisposed = true;
    }
}