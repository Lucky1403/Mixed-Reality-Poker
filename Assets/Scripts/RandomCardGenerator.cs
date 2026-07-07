using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Samples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grabbable)), RequireComponent(typeof(HandGrabInteractable))]
public class RandomCardGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> playingCard;

    // Reference to the components of the deck of cards
    private Grabbable grabbableDeck;

    private HandGrabInteractable handgrabInteractable;
    private HandGrabInteractor currentHandGrabInteractor;

    // Reference to the components of an individual playing card
    private Grabbable playingCardGrabbable;

    private Rigidbody playingCardRb;

    // Current index in the shuffled list
    private int currentIndex;

    private void OnEnable()
    {
        handgrabInteractable = gameObject.GetComponent<HandGrabInteractable>();
        handgrabInteractable.WhenSelectingInteractorAdded.Action += HandleSelectingHandGrabInteractorAdded;

        grabbableDeck = gameObject.GetComponent<Grabbable>();
        grabbableDeck.WhenPointerEventRaised += Grabbable_WhenPointerEventRaised;
    }

    private void Start()
    {
        foreach (var card in playingCard)
        {
            AddSnapInteractor(card);
        }

        // Shuffle the list of instantiated cards
        Shuffle(playingCard);

        // Initialize the current index
        currentIndex = 0;
    }

    private void HandleSelectingHandGrabInteractorAdded(HandGrabInteractor interactor)
    {
        currentHandGrabInteractor = interactor;
    }

    private void Grabbable_WhenPointerEventRaised(PointerEvent obj)
    {
        if (obj.Type == PointerEventType.Select)
        {
            GameObject randomCard = PickRandomCard();
            randomCard.GetComponentInChildren<Grabbable>().enabled = true;
            HandleCardSelection(randomCard);
        }

        if (obj.Type == PointerEventType.Unselect)
        {
            Debug.Log("<< Force Realsed success");
        }
    }

    private GameObject PickRandomCard()
    {
        // If all cards have been picked, return
        if (currentIndex >= playingCard.Count)
        {
            Debug.Log("All cards have been picked.");
            return null;
        }

        // Get the next card from the shuffled list
        GameObject randomCard = playingCard[currentIndex];
        currentIndex++;

        // Optionally, log or do something with the picked card
        return randomCard;
    }

    private void HandleCardSelection(GameObject card)
    {
        HandGrabInteractable cardHandInteractable = card.GetComponentInChildren<HandGrabInteractable>();

        if (currentHandGrabInteractor == null)
        {
            Debug.Log("<<< Current interactor is null");
            return;
        }

        currentHandGrabInteractor.ForceRelease();
        currentHandGrabInteractor.ForceSelect(cardHandInteractable, true);
        return;
    }

    private void AddSnapInteractor(GameObject card)
    {
        playingCardGrabbable = card.gameObject.GetComponentInChildren<Grabbable>();
        playingCardRb = card.gameObject.GetComponentInChildren<Rigidbody>();
        SnapInteractor snapInteractor = playingCardGrabbable.gameObject.AddComponent<SnapInteractor>();
        snapInteractor.InjectPointableElement(playingCardGrabbable);
        snapInteractor.InjectRigidbody(playingCardRb);
        card.SetActive(false);
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}