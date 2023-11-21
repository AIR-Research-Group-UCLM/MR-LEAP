// ConditionCardPicker.cs
// Furious Koalas S.L.
// 2023

using System.Collections;
using UnityEngine;
using static Block;

/// <summary>
/// Esta clase modela el objeto que permite elegir cartas de condición.
/// </summary>
public class ConditionCardPicker : MonoBehaviour
{
    /// <summary>
    /// Array de cartas.
    /// </summary>
    private ConditionCard[] cards;

    /// <summary>
    /// ¿Están las cartas escondidas?
    /// </summary>
    private bool hidedCards = true;

    /// <summary>
    /// ¿Cual es la carta seleccionada?
    /// </summary>
    private ConditionCard selectedCard;

    /// <summary>
    /// Start.
    /// </summary>
    private void Start()
    {
        cards = GetComponentsInChildren<ConditionCard>(true);
        selectedCard = cards[0];
        foreach (ConditionCard card in cards)
        {
            card.gameObject.SetActive(true);
            card.InformOnTap = TappedCard;
            card.gameObject.SetActive(false);
        }
        selectedCard.gameObject.SetActive(true);
    }

    /// <summary>
    /// ¿Está ocupado?
    /// </summary>
    private bool isBusy = false;

    /// <summary>
    /// ¿Está bloqueado?
    /// </summary>
    private bool locked = false;

    /// <summary>
    /// Bloquea el picker para que no responda a las acciónes del usuario.
    /// </summary>
    public void Lock()
    {
        locked = true;
    }

    /// <summary>
    /// Desbloquea el picker para que responda a las acciónes del usuario.
    /// </summary>
    public void Unlock()
    {
        locked = false;
    }

    /// <summary>
    /// Retorna la propiedad de la carta seleccionada.
    /// </summary>
    /// <returns>La propiedad <see cref="BlockProperties"/>.</returns>
    public BlockProperties GetCardProperty()
    {
        return selectedCard.Condition;
    }

    /// <summary>
    /// Se ejecuta cuando se hace tap en una carta.
    /// </summary>
    /// <param name="card">La carta de condición pulsada.</param>
    private void TappedCard(ConditionCard card)
    {
        if (!isBusy && !locked)
        {
            if (hidedCards)
            {
                hidedCards = false;
                StartCoroutine(ShowCards());
            }
            else
            {
                hidedCards = true;
                selectedCard = card;
                StartCoroutine(HideCards(card));
            }
        }
    }

    /// <summary>
    /// Corrutina para mostrar las cartas.
    /// </summary>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator ShowCards()
    {
        isBusy = true;
        foreach (ConditionCard card in cards)
        {
            card.gameObject.SetActive(true);
        }
        yield return new WaitUntil(() => AllActionsFinished());
        foreach (ConditionCard card in cards)
        {
            card.ShowCard();
        }
        yield return new WaitUntil(() => AllActionsFinished());
        isBusy = false;
    }

    /// <summary>
    /// Corrutina para esconder las cartas.
    /// </summary>
    /// <param name="cardToShow">Indica qué carta se tiene que quedar a la vista.</param>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator HideCards(ConditionCard cardToShow)
    {
        isBusy = true;
        yield return new WaitUntil(() => AllActionsFinished());
        foreach (ConditionCard card in cards)
        {
            card.gameObject.SetActive(true);
            card.HideCard();
        }
        yield return new WaitUntil(() => AllActionsFinished());
        foreach (ConditionCard card in cards)
        {
            if (card != cardToShow)
            {
                card.gameObject.SetActive(false);
            }
        }
        isBusy = false;
    }

    /// <summary>
    /// ¿Han terminado todas las acciones?
    /// </summary>
    /// <returns>True si han terminado, false si no.</returns>
    private bool AllActionsFinished()
    {
        foreach (ConditionCard card in cards)
        {
            if (!card.ActionsFinished)
            {
                return false;
            }
        }
        return true;
    }
}