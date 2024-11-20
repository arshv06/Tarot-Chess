using UnityEngine;

public class ChessPieceMovement : MonoBehaviour
{
    private Vector3 originalPosition;
    private bool isDragging = false;
    private GameManager gameManager;
    private BoardManager boardManager;

    public bool hasMoved = false;

    public Vector2Int boardPosition;
    public Vector2Int startingPosition; // For revival

    public int frozenForTurns = 0; // For freezing effect

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }

        boardManager = FindObjectOfType<BoardManager>();
        if (boardManager == null)
        {
            Debug.LogError("BoardManager not found in the scene!");
        }

        boardPosition = boardManager.GetSquareCoordinates(GetSquareNameFromPosition(transform.position));
        startingPosition = boardPosition; // Store starting position
    }

    public Vector2Int GetBoardPosition()
    {
        return boardPosition;
    }
     private void OnMouseDown()
    {
        if (gameManager != null)
        {
            if (gameManager.isCardEffectActive)
            {
                // Do not allow piece movement during a card effect
                return;
            }

            if (frozenForTurns > 0)
            {
                Debug.Log("This piece is frozen and cannot move.");
                UIManager uiManager = FindObjectOfType<UIManager>();
                uiManager.ShowMessage("This piece is frozen and cannot move.");
                return;
            }

            if ((gameManager.IsWhiteTurn() && tag.StartsWith("White")) ||
                (!gameManager.IsWhiteTurn() && tag.StartsWith("Black")))
            {
                originalPosition = transform.position;
                isDragging = true;
                GetComponent<SpriteRenderer>().sortingOrder = 10;
            }
        }
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            transform.position = mousePosition;
        }
    }
    public void FreezeForTurns(int turns)
    {
        frozenForTurns = turns;
    }

     private void OnMouseUp()
    {
        if (isDragging && gameManager != null)
        {
            isDragging = false;
            GetComponent<SpriteRenderer>().sortingOrder = 1;

            Vector2Int originalCoord = boardManager.GetSquareCoordinates(GetSquareNameFromPosition(originalPosition));
            Vector2Int targetCoord = boardManager.GetSquareCoordinates(GetSquareNameFromPosition(transform.position));

            if (IsValidMove(originalCoord, targetCoord))
            {
                GameObject targetPiece = gameManager.GetPieceAtPosition(targetCoord);
                if (targetPiece != null && targetPiece != this.gameObject)
                {
                    if ((tag.StartsWith("White") && targetPiece.tag.StartsWith("Black")) ||
                        (tag.StartsWith("Black") && targetPiece.tag.StartsWith("White")))
                    {
                        // Award a card before capturing the piece
                        gameManager.AwardCard(targetPiece.tag, tag.StartsWith("White"));

                        // Add the captured piece to the captured pieces list
                        if (tag.StartsWith("White"))
                        {
                            gameManager.blackCapturedPieces.Add(targetPiece);
                        }
                        else
                        {
                            gameManager.whiteCapturedPieces.Add(targetPiece);
                        }

                        // Deactivate the captured piece instead of destroying it
                        targetPiece.SetActive(false);
                    }
                    else
                    {
                        // Invalid capture (same color)
                        transform.position = originalPosition;
                        return;
                    }
                }

                gameManager.UpdateBoardPosition(this.gameObject, originalCoord, targetCoord);
                boardPosition = targetCoord;

                hasMoved = true;

                // Update movement flags for special moves
                if (tag.Contains("King"))
                {
                    if (tag.StartsWith("White"))
                        gameManager.whiteKingMoved = true;
                    else
                        gameManager.blackKingMoved = true;
                }
                else if (tag.Contains("Rook"))
                {
                    if (tag.StartsWith("White"))
                    {
                        if (originalCoord.x == 0 && originalCoord.y == 0)
                            gameManager.whiteRookQueenSideMoved = true;
                        else if (originalCoord.x == 7 && originalCoord.y == 0)
                            gameManager.whiteRookKingSideMoved = true;
                    }
                    else
                    {
                        if (originalCoord.x == 0 && originalCoord.y == 7)
                            gameManager.blackRookQueenSideMoved = true;
                        else if (originalCoord.x == 7 && originalCoord.y == 7)
                            gameManager.blackRookKingSideMoved = true;
                    }
                }

                SnapToGrid();
                gameManager.SwitchTurn();
            }
            else
            {
                // Invalid move; reset position
                transform.position = originalPosition;
                Debug.Log("Invalid move. Try again.");
            }
        }
    }

    private string GetSquareNameFromPosition(Vector3 position)
    {
        float squareSize = 1.0f;
        float offsetX = -3.5f; // Adjust based on your board's leftmost position
        float offsetY = -3.5f; // Adjust based on your board's bottom position

        int x = Mathf.RoundToInt((position.x - offsetX) / squareSize);
        int y = Mathf.RoundToInt((position.y - offsetY) / squareSize);
        Vector2Int approxCoord = new Vector2Int(x, y);

        string squareName = boardManager.GetSquareName(approxCoord);
        if (squareName != null)
        {
            Debug.Log($"Position {position} corresponds to square {squareName}");
        }
        else
        {
            Debug.LogError($"Position {position} corresponds to invalid coordinate {approxCoord}");
        }
        return squareName;
    }

    private void SnapToGrid()
{
    transform.position = boardManager.GetSquareWorldPosition(boardPosition);
}


    private bool IsValidMove(Vector2Int from, Vector2Int to)
    {
        bool valid = false;

        if (tag.Contains("Pawn"))
        {
            valid = ValidatePawnMove(from, to);
        }
        else if (tag.Contains("Rook"))
        {
            valid = ValidateRookMove(from, to);
        }
        else if (tag.Contains("Knight"))
        {
            valid = ValidateKnightMove(from, to);
        }
        else if (tag.Contains("Bishop"))
        {
            valid = ValidateBishopMove(from, to);
        }
        else if (tag.Contains("Queen"))
        {
            valid = ValidateQueenMove(from, to);
        }
        else if (tag.Contains("King"))
        {
            valid = ValidateKingMove(from, to);
        }

        if (!valid)
            return false;

        GameObject originalTargetPiece = gameManager.GetPieceAtPosition(to);
        Vector2Int originalPosition = boardPosition;

        gameManager.board[from.x, from.y] = null;
        gameManager.board[to.x, to.y] = this.gameObject;
        boardPosition = to;

        if (originalTargetPiece != null)
        {
            gameManager.board[to.x, to.y] = null;
        }

        bool kingInCheck = IsKingInCheck(tag.StartsWith("White"));

        gameManager.board[from.x, from.y] = this.gameObject;
        gameManager.board[to.x, to.y] = originalTargetPiece;
        boardPosition = originalPosition;

        if (kingInCheck)
        {
            return false;
        }

        return true;
    }

    private bool ValidatePawnMove(Vector2Int from, Vector2Int to)
    {
        int direction = (tag.StartsWith("White")) ? 1 : -1;

        if (from.x == to.x)
        {
            if (to.y - from.y == direction && gameManager.GetPieceAtPosition(to) == null)
            {
                return true;
            }
            if (!hasMoved && to.y - from.y == 2 * direction && gameManager.GetPieceAtPosition(to) == null)
            {
                Vector2Int intermediatePos = new Vector2Int(from.x, from.y + direction);
                if (gameManager.GetPieceAtPosition(intermediatePos) == null)
                {
                    return true;
                }
            }
        }
        if (Mathf.Abs(to.x - from.x) == 1 && to.y - from.y == direction)
        {
            GameObject targetPiece = gameManager.GetPieceAtPosition(to);
            if (targetPiece != null && targetPiece.tag.StartsWith(tag.StartsWith("White") ? "Black" : "White"))
            {
                return true;
            }
            else if (CheckEnPassant(from, to))
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckEnPassant(Vector2Int from, Vector2Int to)
    {
        if (gameManager.lastMovedPiece == null)
            return false;

        ChessPieceMovement lastMovedPieceScript = gameManager.lastMovedPiece.GetComponent<ChessPieceMovement>();
        if (lastMovedPieceScript == null)
            return false;

        if (!gameManager.lastMovedPiece.tag.Contains("Pawn"))
            return false;

        int lastMoveDistance = Mathf.Abs(gameManager.lastMoveTo.y - gameManager.lastMoveFrom.y);
        if (lastMoveDistance != 2)
            return false;

        if (gameManager.lastMoveTo.y != from.y)
            return false;

        if (Mathf.Abs(gameManager.lastMoveTo.x - from.x) != 1)
            return false;

        Vector2Int enPassantCapturePos = new Vector2Int(gameManager.lastMoveTo.x, from.y + (tag.StartsWith("White") ? 1 : -1));

        if (to == enPassantCapturePos)
        {
            Destroy(gameManager.lastMovedPiece);
            gameManager.board[gameManager.lastMoveTo.x, gameManager.lastMoveTo.y] = null;
            return true;
        }

        return false;
    }

    private bool ValidateRookMove(Vector2Int from, Vector2Int to)
    {
        if (from.x == to.x)
        {
            int direction = (to.y > from.y) ? 1 : -1;
            for (int y = from.y + direction; y != to.y; y += direction)
            {
                if (gameManager.GetPieceAtPosition(new Vector2Int(from.x, y)) != null)
                {
                    return false;
                }
            }
            return true;
        }
        else if (from.y == to.y)
        {
            int direction = (to.x > from.x) ? 1 : -1;
            for (int x = from.x + direction; x != to.x; x += direction)
            {
                if (gameManager.GetPieceAtPosition(new Vector2Int(x, from.y)) != null)
                {
                    return false;
                }
            }
            return true;
        }

        return false;
    }

    private bool ValidateKnightMove(Vector2Int from, Vector2Int to)
    {
        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);

        if ((dx == 1 && dy == 2) || (dx == 2 && dy == 1))
        {
            return true;
        }

        return false;
    }

    private bool ValidateBishopMove(Vector2Int from, Vector2Int to)
    {
        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);

        if (dx == dy)
        {
            int xDirection = (to.x > from.x) ? 1 : -1;
            int yDirection = (to.y > from.y) ? 1 : -1;

            for (int i = 1; i < dx; i++)
            {
                if (gameManager.GetPieceAtPosition(new Vector2Int(from.x + i * xDirection, from.y + i * yDirection)) != null)
                {
                    return false;
                }
            }
            return true;
        }

        return false;
    }

    private bool ValidateQueenMove(Vector2Int from, Vector2Int to)
    {
        return ValidateRookMove(from, to) || ValidateBishopMove(from, to);
    }

    private bool ValidateKingMove(Vector2Int from, Vector2Int to)
    {
        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);

        if (dx <= 1 && dy <= 1)
        {
            return true;
        }
        else if (!hasMoved && dx == 2 && dy == 0)
        {
            return CheckCastling(from, to);
        }

        return false;
    }

    private bool CheckCastling(Vector2Int from, Vector2Int to)
    {
        int direction = (to.x - from.x > 0) ? 1 : -1;

        if ((tag.StartsWith("White") && gameManager.whiteKingMoved) ||
            (tag.StartsWith("Black") && gameManager.blackKingMoved))
            return false;

        Vector2Int rookPosition = new Vector2Int((direction > 0) ? 7 : 0, from.y);
        GameObject rookObject = gameManager.GetPieceAtPosition(rookPosition);

        if (rookObject == null)
            return false;

        ChessPieceMovement rookScript = rookObject.GetComponent<ChessPieceMovement>();
        if (rookScript == null || !rookObject.tag.Contains("Rook") || rookScript.hasMoved)
            return false;

        if ((tag.StartsWith("White") && direction > 0 && gameManager.whiteRookKingSideMoved) ||
            (tag.StartsWith("White") && direction < 0 && gameManager.whiteRookQueenSideMoved) ||
            (tag.StartsWith("Black") && direction > 0 && gameManager.blackRookKingSideMoved) ||
            (tag.StartsWith("Black") && direction < 0 && gameManager.blackRookQueenSideMoved))
            return false;

        int start = Mathf.Min(from.x, rookPosition.x) + 1;
        int end = Mathf.Max(from.x, rookPosition.x) - 1;

        for (int x = start; x <= end; x++)
        {
            if (gameManager.GetPieceAtPosition(new Vector2Int(x, from.y)) != null)
                return false;
        }

        if (IsSquareUnderAttack(from, tag.StartsWith("White") ? "Black" : "White"))
            return false;

        Vector2Int passThroughSquare = new Vector2Int(from.x + direction, from.y);
        if (IsSquareUnderAttack(passThroughSquare, tag.StartsWith("White") ? "Black" : "White"))
            return false;

        if (IsSquareUnderAttack(to, tag.StartsWith("White") ? "Black" : "White"))
            return false;

        Vector2Int rookNewPosition = new Vector2Int(from.x + direction, from.y);
        rookObject.transform.position = boardManager.GetSquareWorldPosition(rookNewPosition);
        gameManager.UpdateBoardPosition(rookObject, rookPosition, rookNewPosition);
        rookScript.boardPosition = rookNewPosition;
        rookScript.hasMoved = true;

        if (tag.StartsWith("White"))
        {
            if (direction > 0)
                gameManager.whiteRookKingSideMoved = true;
            else
                gameManager.whiteRookQueenSideMoved = true;
        }
        else
        {
            if (direction > 0)
                gameManager.blackRookKingSideMoved = true;
            else
                gameManager.blackRookQueenSideMoved = true;
        }

        return true;
    }

    private bool IsKingInCheck(bool isWhite)
    {
        Vector2Int kingPosition = new Vector2Int(-1, -1);
        foreach (GameObject pieceObject in gameManager.board)
        {
            if (pieceObject != null && pieceObject.tag == (isWhite ? "WhiteKing" : "BlackKing"))
            {
                ChessPieceMovement kingScript = pieceObject.GetComponent<ChessPieceMovement>();
                kingPosition = kingScript.boardPosition;
                break;
            }
        }

        if (kingPosition.x == -1)
        {
            Debug.LogError("King not found on the board!");
            return false;
        }

        string opponentTagPrefix = isWhite ? "Black" : "White";

        return IsSquareUnderAttack(kingPosition, opponentTagPrefix);
    }

    private bool IsSquareUnderAttack(Vector2Int square, string opponentTagPrefix)
    {
        foreach (GameObject pieceObject in gameManager.board)
        {
            if (pieceObject != null && pieceObject.tag.StartsWith(opponentTagPrefix))
            {
                ChessPieceMovement pieceScript = pieceObject.GetComponent<ChessPieceMovement>();
                if (pieceScript != null)
                {
                    if (pieceScript.CanAttackSquare(square))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool CanAttackSquare(Vector2Int square)
    {
        Vector2Int from = boardPosition;
        Vector2Int to = square;

        if (tag.Contains("Pawn"))
        {
            int direction = (tag.StartsWith("White")) ? 1 : -1;
            if (Mathf.Abs(to.x - from.x) == 1 && to.y - from.y == direction)
            {
                return true;
            }
        }
        else if (tag.Contains("Rook"))
        {
            return ValidateRookMove(from, to);
        }
        else if (tag.Contains("Knight"))
        {
            return ValidateKnightMove(from, to);
        }
        else if (tag.Contains("Bishop"))
        {
            return ValidateBishopMove(from, to);
        }
        else if (tag.Contains("Queen"))
        {
            return ValidateQueenMove(from, to);
        }
        else if (tag.Contains("King"))
        {
            int dx = Mathf.Abs(to.x - from.x);
            int dy = Mathf.Abs(to.y - from.y);
            if (dx <= 1 && dy <= 1)
            {
                return true;
            }
        }

        return false;
    }
}
