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

    public GameObject[,] board = new GameObject[8, 8];

    public GameObject lastMovedPiece = null;
    public Vector2Int lastMoveFrom;
    public Vector2Int lastMoveTo;

    private BoardManager boardManager;

    private void Start()
    {
        boardManager = FindObjectOfType<BoardManager>();
        if (boardManager == null)
        {
            Debug.LogError("BoardManager not found in the scene!");
        }

        SetInitialPiecePositions();
        InitializeBoard();
    }

    public void SetInitialPiecePositions()
    {
        foreach (ChessPieceMovement piece in FindObjectsOfType<ChessPieceMovement>())
        {
            string squareName = GetStartingSquareForPiece(piece.tag);
            if (squareName != null)
            {
                Vector2Int coord = boardManager.GetSquareCoordinates(squareName);
                Vector3 worldPosition = boardManager.GetSquareWorldPosition(coord);
                piece.transform.position = worldPosition;
                piece.boardPosition = coord;
            }
            else
            {
                Debug.LogError($"Starting square for piece {piece.tag} not defined.");
            }
        }
    }

    private string GetStartingSquareForPiece(string pieceTag)
    {
        // Define the starting positions based on the piece's tag
        switch (pieceTag)
        {
            // White pieces
            case "WhiteKing": return "E1";
            case "WhiteQueen": return "D1";
            case "WhiteRook1": return "A1";
            case "WhiteRook2": return "H1";
            case "WhiteBishop1": return "C1";
            case "WhiteBishop2": return "F1";
            case "WhiteKnight1": return "B1";
            case "WhiteKnight2": return "G1";
            case "WhitePawn1": return "A2";
            case "WhitePawn2": return "B2";
            case "WhitePawn3": return "C2";
            case "WhitePawn4": return "D2";
            case "WhitePawn5": return "E2";
            case "WhitePawn6": return "F2";
            case "WhitePawn7": return "G2";
            case "WhitePawn8": return "H2";
            // Black pieces
            case "BlackKing": return "E8";
            case "BlackQueen": return "D8";
            case "BlackRook1": return "A8";
            case "BlackRook2": return "H8";
            case "BlackBishop1": return "C8";
            case "BlackBishop2": return "F8";
            case "BlackKnight1": return "B8";
            case "BlackKnight2": return "G8";
            case "BlackPawn1": return "A7";
            case "BlackPawn2": return "B7";
            case "BlackPawn3": return "C7";
            case "BlackPawn4": return "D7";
            case "BlackPawn5": return "E7";
            case "BlackPawn6": return "F7";
            case "BlackPawn7": return "G7";
            case "BlackPawn8": return "H7";
            default:
                return null;
        }
    }

    public void InitializeBoard()
    {
        ChessPieceMovement[] allPieces = FindObjectsOfType<ChessPieceMovement>();
        foreach (ChessPieceMovement piece in allPieces)
        {
            Vector2Int coord = piece.GetBoardPosition();
            Debug.Log($"Placing {piece.tag} at coordinates: {coord}");
            board[coord.x, coord.y] = piece.gameObject;
            Debug.Log($"Placing {piece.tag} at {coord}");
        }
    }


    public void UpdateBoardPosition(GameObject piece, Vector2Int oldPos, Vector2Int newPos)
    {
        board[oldPos.x, oldPos.y] = null;
        board[newPos.x, newPos.y] = piece;

        lastMovedPiece = piece;
        lastMoveFrom = oldPos;
        lastMoveTo = newPos;
    }

    public GameObject GetPieceAtPosition(Vector2Int position)
    {
        if (position.x >= 0 && position.x < 8 && position.y >= 0 && position.y < 8)
        {
            return board[position.x, position.y];
        }
        else
        {
            return null;
        }
    }

    public void SwitchTurn()
    {
        isWhiteTurn = !isWhiteTurn;
    }

    public bool IsWhiteTurn()
    {
        return isWhiteTurn;
    }
}