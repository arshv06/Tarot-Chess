using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isWhiteTurn = true;
    public bool whiteKingMoved = false;
    public bool whiteRookKingSideMoved = false;
    public bool whiteRookQueenSideMoved = false;
    public bool blackKingMoved = false;
    public bool blackRookKingSideMoved = false;
    public bool blackRookQueenSideMoved = false;


    public void SwitchTurn()
    {
        isWhiteTurn = !isWhiteTurn;
    }

    public bool IsWhiteTurn()
    {
        return isWhiteTurn;
    }
}
