using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Clase <see cref="EventAggregator" /> usada como un punto central en el que los objetos del juego
/// pueden públicar mensajes y suscribirse para recibir los tipos de mensajes que les interesen.
/// Tomado y adaptado de 
/// https://www.c-sharpcorner.com/UploadFile/pranayamr/publisher-or-subscriber-pattern-with-event-or-delegate-and-e/
/// </summary>
public class EventAggregator : MonoBehaviour
{
    /// <summary>
    /// Diccionario que contiene los diferentes subscriptores y al tipo de mensaje que están suscritos.
    /// </summary>
    private Dictionary<Type, IList> subscriber = new Dictionary<Type, IList>();

    /// <summary>
    /// Instancia de la clase a los que los demás objetos pueden acceder.
    /// </summary>
    private static EventAggregator eventAgregator;

    /// <summary>
    /// Getter de la instancia de la clase.
    /// </summary>
    public static EventAggregator Instance
    {
        get
        {
            if (!eventAgregator)
            {
                eventAgregator = FindObjectOfType(typeof(EventAggregator)) as EventAggregator;

                if (!eventAgregator)
                {
                    Debug.LogError("There needs to be one active EventAggregator script on a GameObject in your scene.");
                }
            }

            return eventAgregator;
        }
    }

    /// <summary>
    /// Método para públicar mensajes
    /// </summary>
    /// <typeparam name="TMessageType">.</typeparam>
    /// <param name="message">El mensaje a publicar<see cref="TMessageType"/>.</param>
    public void Publish<TMessageType>(TMessageType message)
    {
        Type t = typeof(TMessageType);
        IList actionlst;
        if (subscriber.ContainsKey(t))
        {
            actionlst = new List<Subscription<TMessageType>>(subscriber[t].Cast<Subscription<TMessageType>>());

            foreach (Subscription<TMessageType> a in actionlst)
            {
                try
                {
                    a.Action(message);
                }
                catch
                {
                    Unsubscribe(a);
                }
            }
        }
    }

    /// <summary>
    /// Procedimiento para suscribirse a los mensajes deseados.
    /// </summary>
    /// <typeparam name="TMessageType">.</typeparam>
    /// <param name="action">Metodo al que se va a llamar cuando se reciba un mensaje
    /// <see cref="Action{TMessageType}"/>.</param>
    /// <returns>Token de suscripcion <see cref="Subscription{TMessageType}"/>.</returns>
    public Subscription<TMessageType> Subscribe<TMessageType>(Action<TMessageType> action)
    {
        Type t = typeof(TMessageType);
        IList actionlst;
        var actiondetail = new Subscription<TMessageType>(action, this);

        if (!subscriber.TryGetValue(t, out actionlst))
        {
            actionlst = new List<Subscription<TMessageType>>();
            actionlst.Add(actiondetail);
            subscriber.Add(t, actionlst);
        }
        else
        {
            actionlst.Add(actiondetail);
        }

        return actiondetail;
    }

    /// <summary>
    /// Metodo para desuscribirse.
    /// </summary>
    /// <typeparam name="TMessageType">.</typeparam>
    /// <param name="subscription">La suscripción <see cref="Subscription{TMessageType}"/>.</param>
    public void Unsubscribe<TMessageType>(Subscription<TMessageType> subscription)
    {
        Type t = typeof(TMessageType);
        if (subscriber.ContainsKey(t))
        {
            subscriber[t].Remove(subscription);
        }
    }
}