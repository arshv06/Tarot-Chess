using UnityEngine;

[CreateAssetMenu(fileName = "ReviveCard", menuName = "Cards/Revive")]
public class ReviveCard : Card
{
    public override void PlayCard(GameManager gameManager, bool isWhitePlayer, System.Action onEffectComplete)
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        uiManager.ShowMessage("Reviving your last captured piece...");

        gameManager.StartCoroutine(gameManager.RevivePiece(isWhitePlayer, onEffectComplete));
    }
}
