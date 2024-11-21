using UnityEngine;

[CreateAssetMenu(fileName = "ReverseCard", menuName = "Cards/Reverse")]
public class ReverseCard : Card
{
    public override void PlayCard(GameManager gameManager, bool isWhitePlayer, System.Action onEffectComplete)
    {
        gameManager.ReverseLastMove();
        onEffectComplete?.Invoke();
    }
}
