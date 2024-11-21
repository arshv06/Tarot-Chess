using UnityEngine;

[CreateAssetMenu(fileName = "QueenTransformationCard", menuName = "Cards/QueenTransformation")]
public class QueenTransformationCard : Card
{
    public override void PlayCard(GameManager gameManager, bool isWhitePlayer, System.Action onEffectComplete)
    {
        gameManager.StartCoroutine(gameManager.TransformPawnToQueen(isWhitePlayer, onEffectComplete));
    }
}
