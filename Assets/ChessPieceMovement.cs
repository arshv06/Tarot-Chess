using UnityEngine;

public class ChessPieceMovement : MonoBehaviour
{
    private Vector3 originalPosition;
    private bool isDragging = false;
    private GameManager gameManager;
    private BoardManager boardManager;

    private void Start()
    {
        // Find the GameManager in the scene
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

            // Bring the piece to the front by changing the z-position
            GetComponent<SpriteRenderer>().sortingOrder = 10; // A higher value ensures the piece is on top
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


        if (IsValidMove())
        {
            // Check if there is an opponent or same-color piece at the target position
            Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position);
            foreach (Collider2D collider in colliders)
            {
                GameObject otherPiece = collider.gameObject;
                if (otherPiece != this.gameObject)
                {
                    // Capture opponent piece
                    if ((tag.StartsWith("White") && otherPiece.tag.StartsWith("Black")) ||
                        (tag.StartsWith("Black") && otherPiece.tag.StartsWith("White")))
                    {
                        Destroy(otherPiece);
                        break;
                    }
                    // Prevent move if the square is occupied by a piece of the same color
                    else if ((tag.StartsWith("White") && otherPiece.tag.StartsWith("White")) ||
                             (tag.StartsWith("Black") && otherPiece.tag.StartsWith("Black")))
                    {
                        transform.position = originalPosition; // Return to original position
                        return;
                    }
                }
            }

            SnapToGrid();
            gameManager.SwitchTurn();
        }
        else
        {
            transform.position = originalPosition; // Return to original position if the move is invalid
        }
    }
}

    private void SnapToGrid()
{
    // Adjust these values based on your board setup
    float squareSize = 1.0f;  // Ensure this is the size of each square on your board
    float offsetX = -3.5f;     // Adjust to align the grid with the board
    float offsetY = -3.5f;     // Adjust to align the grid with the board

    // Calculate the snapped position using Mathf.Round to ensure exact rounding
    float snappedX = Mathf.Round((transform.position.x - offsetX) / squareSize) * squareSize + offsetX;
    float snappedY = Mathf.Round((transform.position.y - offsetY) / squareSize) * squareSize + offsetY;

    // Add a small adjustment to help with snapping if the piece is close to the desired position
    if (Mathf.Abs(transform.position.x - snappedX) < 0.2f) // 0.2 is a tolerance value you can adjust
    {
        transform.position = new Vector3(snappedX, transform.position.y, 0);
    }
    if (Mathf.Abs(transform.position.y - snappedY) < 0.2f) // 0.2 is a tolerance value you can adjust
    {
        transform.position = new Vector3(transform.position.x, snappedY, 0);
    }
}


    private bool IsValidMove()
{
    float deltaX = Mathf.Abs(transform.position.x - originalPosition.x);
    float deltaY = Mathf.Abs(transform.position.y - originalPosition.y);
    float tolerance = 0.1f; // Use a small tolerance to handle floating-point precision issues

    // Pawn Movement Logic

    
    if (tag.Contains("Pawn"))
    {
        if (tag.StartsWith("White"))
        {
            // White Pawns: Allow moving forward one or two squares from the starting position
            if (Mathf.Abs(deltaX) < tolerance)
            {
                // Check for one-step move
                if (Mathf.Abs(transform.position.y - originalPosition.y - 1.0f) < tolerance)
                {
                    return true;
                }

                // Check for two-step move from the starting position (row 1)
                if (originalPosition.y == 1.0f && Mathf.Abs(transform.position.y - originalPosition.y - 2.0f) < tolerance)
                {
                    return true;
                }
            }
            // Check for diagonal captures
            if (Mathf.Abs(deltaX - 1.0f) < tolerance && Mathf.Abs(transform.position.y - originalPosition.y - 1.0f) < tolerance)
            {
                // Check if there is an opponent piece diagonally
                Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position);
                foreach (Collider2D collider in colliders)
                {
                    if (collider.gameObject != this.gameObject && collider.gameObject.tag.StartsWith("Black"))
                    {
                        return true;
                    }
                }
            }
        }
        else if (tag.StartsWith("Black"))
        {
            // Black Pawns: Allow moving forward one or two squares from the starting position
            if (Mathf.Abs(deltaX) < tolerance)
            {
                // Check for one-step move
                if (Mathf.Abs(originalPosition.y - transform.position.y - 1.0f) < tolerance)
                {
                    return true;
                }

                // Check for two-step move from the starting position (row 6)
                if (originalPosition.y == 6.0f && Mathf.Abs(originalPosition.y - transform.position.y - 2.0f) < tolerance)
                {
                    return true;
                }
            }
            // Check for diagonal captures
            if (Mathf.Abs(deltaX - 1.0f) < tolerance && Mathf.Abs(originalPosition.y - transform.position.y - 1.0f) < tolerance)
            {
                // Check if there is an opponent piece diagonally
                Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position);
                foreach (Collider2D collider in colliders)
                {
                    if (collider.gameObject != this.gameObject && collider.gameObject.tag.StartsWith("White"))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }


    // Rook Movement Logic
    if (tag.Contains("Rook"))
    {
        return Mathf.Abs(deltaX) < tolerance || Mathf.Abs(deltaY) < tolerance;
    }

    // Knight Movement Logic
    if (tag.Contains("Knight"))
    {
        return (Mathf.Abs(deltaX - 1.0f) < tolerance && Mathf.Abs(deltaY - 2.0f) < tolerance) || 
               (Mathf.Abs(deltaX - 2.0f) < tolerance && Mathf.Abs(deltaY - 1.0f) < tolerance);
    }

    // Bishop Movement Logic
    if (tag.Contains("Bishop"))
    {
        return Mathf.Abs(deltaX - deltaY) < tolerance;
    }

    // Queen Movement Logic
    if (tag.Contains("Queen"))
    {
        return (Mathf.Abs(deltaX) < tolerance || Mathf.Abs(deltaY) < tolerance) || 
               Mathf.Abs(deltaX - deltaY) < tolerance;
    }

    // King Movement Logic
    if (tag.Contains("King"))
    {
        return deltaX <= 1 + tolerance && deltaY <= 1 + tolerance;
    }

    return false;
}
}
