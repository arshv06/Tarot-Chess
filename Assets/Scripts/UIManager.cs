using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI messageText;

    // Move history UI elements
    public Transform moveHistoryContainer;
    public GameObject moveEntryPrefab;

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

    public void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }
    }

    public void ClearMessage()
    {
        if (messageText != null)
        {
            messageText.text = "";
        }
    }
}
