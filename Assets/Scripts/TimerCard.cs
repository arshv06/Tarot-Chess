using UnityEngine;

[CreateAssetMenu(fileName = "TimerCard", menuName = "Cards/Timer")]
public class TimerCard : Card
{
    public int timeToAdd = 180; // 3 minutes in seconds

    public override void PlayCard(GameManager gameManager, bool isWhitePlayer, System.Action onEffectComplete)
    {
        gameManager.AddTimeToPlayer(isWhitePlayer, timeToAdd);
        onEffectComplete?.Invoke();
    }
}
