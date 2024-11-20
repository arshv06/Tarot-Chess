using UnityEngine;

[CreateAssetMenu(fileName = "FreezeCard", menuName = "Cards/Freeze")]
public class FreezeCard : Card
{
    public int freezeDuration = 2; // Number of turns to freeze

    public override void PlayCard(GameManager gameManager, bool isWhitePlayer, System.Action onEffectComplete)
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        uiManager.ShowMessage("Select an opponent's piece to freeze.");

        gameManager.StartCoroutine(gameManager.FreezePiece(isWhitePlayer, freezeDuration, onEffectComplete));
    }
}
