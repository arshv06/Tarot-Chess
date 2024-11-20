using UnityEngine;

[CreateAssetMenu(fileName = "TeleportCard", menuName = "Cards/Teleport")]
public class TeleportCard : Card
{
    public override void PlayCard(GameManager gameManager, bool isWhitePlayer, System.Action onEffectComplete)
    {
        Debug.Log("TeleportCard Used.");

        UIManager uiManager = FindObjectOfType<UIManager>();
        uiManager.ShowMessage("Select a piece to teleport.");

        gameManager.StartCoroutine(gameManager.TeleportPiece(isWhitePlayer, onEffectComplete));
    }
}
