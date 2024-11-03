using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isWhiteTurn = true;

    public void SwitchTurn()
    {
        isWhiteTurn = !isWhiteTurn;
    }

    public bool IsWhiteTurn()
    {
        return isWhiteTurn;
    }
}
