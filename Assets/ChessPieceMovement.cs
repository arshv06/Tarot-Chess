using UnityEngine;

public class ChessPieceMovement : MonoBehaviour
{
    private Vector3 originalPosition;
    private bool isDragging = false;
    private GameManager gameManager;
    private BoardManager boardManager;

    // Flags for special moves
    private bool hasMoved = false;

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
    }

    private void OnMouseDown()
    {
        if (gameManager != null)
        {
            if ((gameManager.IsWhiteTurn() && tag.StartsWith("White")) || 
                (!gameManager.IsWhiteTurn() && tag.StartsWith("Black")))
            {
                originalPosition = transform.position;
                isDragging = true;
                GetComponent<SpriteRenderer>().sortingOrder = 10; // Bring piece to front
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

    private void OnMouseUp()
{
    if (isDragging && gameManager != null)
    {
        isDragging = false;
        GetComponent<SpriteRenderer>().sortingOrder = 1;

        Vector2Int originalCoord = boardManager.GetSquareCoordinates(GetSquareNameFromPosition(originalPosition));
        Vector2Int targetCoord = boardManager.GetSquareCoordinates(GetSquareNameFromPosition(transform.position));

        Debug.Log($"Trying to move from {originalCoord} to {targetCoord}");

        if (IsValidMove())
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position);
            foreach (Collider2D collider in colliders)
            {
                GameObject otherPiece = collider.gameObject;
                if (otherPiece != this.gameObject)
                {
                    if ((tag.StartsWith("White") && otherPiece.tag.StartsWith("Black")) ||
                        (tag.StartsWith("Black") && otherPiece.tag.StartsWith("White")))
                    {
                        Destroy(otherPiece);
                        break;
                    }
                    else if ((tag.StartsWith("White") && otherPiece.tag.StartsWith("White")) ||
                             (tag.StartsWith("Black") && otherPiece.tag.StartsWith("Black")))
                    {
                        transform.position = originalPosition;
                        return;
                    }
                }
            }
            SnapToGrid();
            gameManager.SwitchTurn();
        }
        else
        {
            transform.position = originalPosition;
        }
    }
}

private string GetSquareNameFromPosition(Vector3 position)
{
    Vector2Int approxCoord = new Vector2Int(Mathf.RoundToInt(position.x + 3.5f), Mathf.RoundToInt(position.y + 3.5f));
    string squareName = boardManager.GetSquareName(approxCoord);
    Debug.Log($"Position {position} corresponds to square {squareName}");
    return squareName;
}


    private void SnapToGrid()
    {
        float squareSize = 1.0f;
        float offsetX = -3.5f;
        float offsetY = -3.5f;

        float snappedX = Mathf.Round((transform.position.x - offsetX) / squareSize) * squareSize + offsetX;
        float snappedY = Mathf.Round((transform.position.y - offsetY) / squareSize) * squareSize + offsetY;

        transform.position = new Vector3(snappedX, snappedY, 0);
    }
    
    private bool IsValidMove()
{
    float deltaX = Mathf.Abs(transform.position.x - originalPosition.x);
    float deltaY = transform.position.y - originalPosition.y; // Keep deltaY signed for direction
    float tolerance = 0.1f;

    if (tag.Contains("Pawn"))
    {
        return ValidatePawnMove(deltaX, deltaY, tolerance);
    }
    else if (tag.Contains("Rook"))
    {
        return deltaX < tolerance || Mathf.Abs(deltaY) < tolerance;
    }
    else if (tag.Contains("Knight"))
    {
        return (Mathf.Abs(deltaX - 1.0f) < tolerance && Mathf.Abs(deltaY - 2.0f) < tolerance) || 
               (Mathf.Abs(deltaX - 2.0f) < tolerance && Mathf.Abs(deltaY - 1.0f) < tolerance);
    }
    else if (tag.Contains("Bishop"))
    {
        return Mathf.Abs(deltaX - Mathf.Abs(deltaY)) < tolerance;
    }
    else if (tag.Contains("Queen"))
    {
        return deltaX < tolerance || Mathf.Abs(deltaY) < tolerance || Mathf.Abs(deltaX - Mathf.Abs(deltaY)) < tolerance;
    }
    else if (tag.Contains("King"))
    {
        return ValidateKingMove(deltaX, deltaY, tolerance);
    }

    return false;
}

    private bool ValidatePawnMove(float deltaX, float deltaY, float tolerance)
{
    if (tag.StartsWith("White"))
    {
        // White pawn moving forward (upwards on the board)
        if (deltaX < tolerance)
        {
            // Single step forward
            if (Mathf.Abs(deltaY - 1.0f) < tolerance)
            {
                return true;
            }
            // Double step forward if on starting row
            if (originalPosition.y == 1.0f && Mathf.Abs(deltaY - 2.0f) < tolerance && !hasMoved)
            {
                return true;
            }
        }
        // Diagonal capture (one step diagonally forward)
        if (Mathf.Abs(deltaX - 1.0f) < tolerance && Mathf.Abs(deltaY - 1.0f) < tolerance)
        {
            return CheckForOpponentPiece() || CheckEnPassant();
        }
    }
    else if (tag.StartsWith("Black"))
    {
        // Black pawn moving forward (downwards on the board)
        if (deltaX < tolerance)
        {
            // Single step forward
            if (Mathf.Abs(deltaY + 1.0f) < tolerance)
            {
                return true;
            }
            // Double step forward if on starting row
            if (originalPosition.y == 6.0f && Mathf.Abs(deltaY + 2.0f) < tolerance && !hasMoved)
            {
                return true;
            }
        }
        // Diagonal capture (one step diagonally forward)
        if (Mathf.Abs(deltaX - 1.0f) < tolerance && Mathf.Abs(deltaY + 1.0f) < tolerance)
        {
            return CheckForOpponentPiece() || CheckEnPassant();
        }
    }
    return false;
}

    private bool CheckEnPassant()
    {
        // Logic to check for en passant conditions, possibly by querying the GameManager
        // Example: return gameManager.IsEnPassantPossible(this);
        return false;
    }

    private bool ValidateKingMove(float deltaX, float deltaY, float tolerance)
    {
        if (deltaX <= 1 + tolerance && Mathf.Abs(deltaY) <= 1 + tolerance)
        {
            return true;
        }
        if (deltaX == 2 && deltaY == 0)
        {
        Debug.Log("Attempting castling move");
        // Additional logic to validate castling conditions here
        return true; // Only for testing; add real castling checks later
        }
            
        return false;
    }
    private bool CheckForOpponentPiece()
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != this.gameObject && 
                ((tag.StartsWith("White") && collider.gameObject.tag.StartsWith("Black")) ||
                (tag.StartsWith("Black") && collider.gameObject.tag.StartsWith("White"))))
            {
                return true;
            }
        }
        return false;
    }


}
