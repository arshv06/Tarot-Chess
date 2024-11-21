using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class UIManager : MonoBehaviour
{
    // Message display
    public TextMeshProUGUI messageText;

    // Move history UI elements
    public Transform moveHistoryContainer;
    public GameObject moveEntryPrefab;

    // End game panel
    public GameObject endGamePanel;
    public TextMeshProUGUI endGameText;

    // Captured pieces display
    public Transform whiteCapturedPiecesContainer;
    public Transform blackCapturedPiecesContainer;
    public GameObject capturedPieceIconPrefab;
    // Timer display
    public TextMeshProUGUI whitePlayerTimerText;
    public TextMeshProUGUI blackPlayerTimerText;

    // Method to update the move history UI
    public void UpdateMoveHistory(List<string> moveHistory)
    {
        // Clear existing entries
        foreach (Transform child in moveHistoryContainer)
        {
            Destroy(child.gameObject);
        }

        // Display each move
        foreach (string move in moveHistory)
        {
            GameObject moveEntryGO = Instantiate(moveEntryPrefab, moveHistoryContainer);
            TextMeshProUGUI moveText = moveEntryGO.GetComponentInChildren<TextMeshProUGUI>();
            if (moveText != null)
            {
                moveText.text = move;
            }
            else
            {
                Debug.LogError("MoveEntry prefab does not have a TextMeshProUGUI component.");
            }
        }
    }

    // Method to show messages to the player
    public void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }
    }

    // Method to clear the message display
    public void ClearMessage()
    {
        if (messageText != null)
        {
            messageText.text = "";
        }
    }

    // Method to display the end game panel
    public void ShowEndGamePanel(string winner)
    {
        if (endGamePanel != null && endGameText != null)
        {
            endGameText.text = winner + " wins!";
            endGamePanel.SetActive(true);
        }
    }

    // Method to update the captured pieces display
    public void UpdateCapturedPieces(List<GameObject> whiteCaptured, List<GameObject> blackCaptured)
{
    // Clear existing icons
    foreach (Transform child in whiteCapturedPiecesContainer)
    {
        Destroy(child.gameObject);
    }
    foreach (Transform child in blackCapturedPiecesContainer)
    {
        Destroy(child.gameObject);
    }

    // Display white captured pieces
    foreach (GameObject piece in whiteCaptured)
    {
        GameObject icon = Instantiate(capturedPieceIconPrefab, whiteCapturedPiecesContainer);
        Image iconImage = icon.GetComponent<Image>();
        // Set the sprite based on the captured piece's SpriteRenderer
        SpriteRenderer pieceSpriteRenderer = piece.GetComponent<SpriteRenderer>();
        if (pieceSpriteRenderer != null)
        {
            iconImage.sprite = pieceSpriteRenderer.sprite;
        }
        else
        {
            Debug.LogError("Captured piece does not have a SpriteRenderer component.");
        }
    }

    // Display black captured pieces
    foreach (GameObject piece in blackCaptured)
    {
        GameObject icon = Instantiate(capturedPieceIconPrefab, blackCapturedPiecesContainer);
        Image iconImage = icon.GetComponent<Image>();
        // Set the sprite based on the captured piece's SpriteRenderer
        SpriteRenderer pieceSpriteRenderer = piece.GetComponent<SpriteRenderer>();
        if (pieceSpriteRenderer != null)
        {
            iconImage.sprite = pieceSpriteRenderer.sprite;
        }
        else
        {
            Debug.LogError("Captured piece does not have a SpriteRenderer component.");
        }
    }
}

    // Method to update the timer display
    public void UpdateTimer(bool isWhitePlayer, float timeInSeconds)
{
    int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
    int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
    string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);

    if (isWhitePlayer)
    {
        whitePlayerTimerText.text = timeString;
    }
    else
    {
        blackPlayerTimerText.text = timeString;
    }
}

}
