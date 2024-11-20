using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandUI : MonoBehaviour
{
    public GameManager gameManager;
    public bool isWhitePlayer;
    public Transform cardContainer;
    public GameObject cardPrefab;

    private Hand playerHand;

    private void Start()
    {
        // Get the player's hand from the GameManager
        playerHand = isWhitePlayer ? gameManager.whitePlayerHand : gameManager.blackPlayerHand;

        // Subscribe to the OnHandChanged event
        playerHand.OnHandChanged += UpdateHandDisplay;

        // Initialize the hand display
        UpdateHandDisplay();
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event when the script is destroyed to prevent memory leaks
        if (playerHand != null)
        {
            playerHand.OnHandChanged -= UpdateHandDisplay;
        }
    }

    void UpdateHandDisplay()
    {
        // Clear existing cards
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        // Display each card in the player's hand
        foreach (Card card in playerHand.cardsInHand)
        {
            // Instantiate a new card UI element
            GameObject cardGO = Instantiate(cardPrefab, cardContainer);

            // Set the card's name on the UI
            Text cardText = cardGO.GetComponentInChildren<Text>();
            if (cardText != null)
            {
                cardText.text = card.cardName;
            }

            // Set the card's artwork (if you have an Image component)
            Image[] images = cardGO.GetComponentsInChildren<Image>();
            foreach (Image img in images)
            {
                if (img.gameObject.name == "ArtworkImage")
                {
                    img.sprite = card.artwork;
                    break;
                }
            }

            // Add a listener to the button to handle clicks
            Button cardButton = cardGO.GetComponent<Button>();
            if (cardButton != null)
            {
                Card capturedCard = card; // Capture the current card in the loop

                cardButton.onClick.AddListener(() => OnCardClicked(capturedCard));
            }
            else
            {
                Debug.LogWarning("No Button component found on the card prefab.");
            }
        }
    }

            void OnCardClicked(Card card)
    {
        Debug.Log($"Card {card.cardName} clicked.");

        // Check if it's the player's turn
        bool isPlayerTurn = (gameManager.isWhiteTurn && isWhitePlayer) || (!gameManager.isWhiteTurn && !isWhitePlayer);

        if (!isPlayerTurn)
        {
            Debug.Log("It's not your turn!");
            return;
        }

        // Play the card's effect with a callback to switch turn after completion
        card.PlayCard(gameManager, isWhitePlayer, () =>
        {
            // Remove the card from the player's hand
            playerHand.RemoveCard(card);

            // Switch the turn after playing the card
            gameManager.SwitchTurn();
        });

        // Note: Do not remove the card or switch the turn here, as the effect may be asynchronous
    }

}
    

