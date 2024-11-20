using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Turn management variables
    public bool isWhiteTurn = true;

    // Movement flags for special moves (e.g., castling)
    public bool whiteKingMoved = false;
    public bool whiteRookKingSideMoved = false;
    public bool whiteRookQueenSideMoved = false;
    public bool blackKingMoved = false;
    public bool blackRookKingSideMoved = false;
    public bool blackRookQueenSideMoved = false;

    // Board representation
    public GameObject[,] board = new GameObject[8, 8];

    // Tracking the last move
    public GameObject lastMovedPiece = null;
    public Vector2Int lastMoveFrom;
    public Vector2Int lastMoveTo;

    private BoardManager boardManager;

    // Card system variables
    public Deck deck;
    public Hand whitePlayerHand = new Hand();
    public Hand blackPlayerHand = new Hand();
    public bool isCardEffectActive = false;

    // Lists to track captured pieces for each player
    public List<GameObject> whiteCapturedPieces = new List<GameObject>();
    public List<GameObject> blackCapturedPieces = new List<GameObject>();

    private void Start()
    {
        boardManager = FindObjectOfType<BoardManager>();
        if (boardManager == null)
        {
            Debug.LogError("BoardManager not found in the scene!");
        }

        SetInitialPiecePositions();
        InitializeBoard();

        // Uncomment the following lines to add test cards to the white player's hand
        
        /*
        for (int i = 0; i < 2; i++)
        {
            Card testCard = ScriptableObject.CreateInstance<ReviveCard>();
            testCard.cardName = "Revive";
            whitePlayerHand.AddCard(testCard);
        }
        */
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
                piece.startingPosition = coord; // Store starting position for revival
            }
            else
            {
                Debug.LogError($"Starting square for piece {piece.tag} not defined.");
            }
        }
    }

    private string GetStartingSquareForPiece(string pieceTag)
    {
        // Map each piece's tag to its starting square
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
            board[coord.x, coord.y] = piece.gameObject;
        }
    }

    public void UpdateBoardPosition(GameObject piece, Vector2Int oldPos, Vector2Int newPos)
    {
        if (oldPos.x >= 0 && oldPos.y >= 0)
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
        if (isCardEffectActive)
        {
            // Do not switch turn if a card effect is still in progress
            return;
        }

        // Handle end-of-turn effects before switching turn
        EndTurnEffects();

        isWhiteTurn = !isWhiteTurn;
    }

    public bool IsWhiteTurn()
    {
        return isWhiteTurn;
    }

    // Method to handle end-of-turn effects
    public void EndTurnEffects()
    {
        // Decrement freeze durations on all pieces
        ChessPieceMovement[] allPieces = FindObjectsOfType<ChessPieceMovement>();
        foreach (ChessPieceMovement piece in allPieces)
        {
            if (piece.frozenForTurns > 0)
            {
                piece.frozenForTurns--;
                if (piece.frozenForTurns == 0)
                {
                    Debug.Log($"{piece.tag} is no longer frozen.");
                    UIManager uiManager = FindObjectOfType<UIManager>();
                    uiManager.ShowMessage($"{piece.tag} is no longer frozen.");
                }
            }
        }
    }

    // Card system methods

    public void AwardCard(string capturedPieceTag, bool isWhitePlayer)
    {
        // Determine card rarity based on captured piece
        CardRarity rarity = GetRarityFromCapturedPiece(capturedPieceTag);

        // Draw a card of the appropriate rarity
        Card drawnCard = DrawCardOfRarity(rarity);

        if (drawnCard != null)
        {
            if (isWhitePlayer)
            {
                whitePlayerHand.AddCard(drawnCard);
            }
            else
            {
                blackPlayerHand.AddCard(drawnCard);
            }
            Debug.Log($"{(isWhitePlayer ? "White" : "Black")} player received a {drawnCard.cardName} card!");
        }
    }

    private CardRarity GetRarityFromCapturedPiece(string pieceTag)
    {
        if (pieceTag.Contains("Pawn"))
        {
            return CardRarity.Common;
        }
        else if (pieceTag.Contains("Knight") || pieceTag.Contains("Bishop"))
        {
            return CardRarity.Rare;
        }
        else if (pieceTag.Contains("Rook"))
        {
            return CardRarity.Epic;
        }
        else if (pieceTag.Contains("Queen"))
        {
            return CardRarity.Legendary;
        }
        else
        {
            return CardRarity.Common;
        }
    }

    private Card DrawCardOfRarity(CardRarity rarity)
    {
        // Filter the deck for cards of the desired rarity
        List<Card> availableCards = deck.cards.FindAll(card => card.rarity == rarity);

        if (availableCards.Count == 0)
        {
            Debug.LogWarning($"No cards of rarity {rarity} left in the deck!");
            return null;
        }

        int index = Random.Range(0, availableCards.Count);
        Card drawnCard = availableCards[index];
        deck.cards.Remove(drawnCard);
        return drawnCard;
    }

    // Teleport card effect
    public IEnumerator TeleportPiece(bool isWhitePlayer, System.Action onEffectComplete)
{
    isCardEffectActive = true;

    UIManager uiManager = FindObjectOfType<UIManager>();
    uiManager.ShowMessage("Select a piece to teleport.");

    // Wait for player to select a piece
    ChessPieceMovement selectedPiece = null;
    bool pieceSelected = false;

    while (!pieceSelected)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D collider = Physics2D.OverlapPoint(mousePosition);

            if (collider != null)
            {
                ChessPieceMovement piece = collider.GetComponent<ChessPieceMovement>();
                if (piece != null && piece.tag.StartsWith(isWhitePlayer ? "White" : "Black"))
                {
                    selectedPiece = piece;
                    pieceSelected = true;
                }
            }
        }
        yield return null;
    }

    uiManager.ShowMessage("Select a destination square.");

    // Wait for player to select an empty square
    Vector2Int targetCoord = new Vector2Int(-1, -1);
    bool destinationSelected = false;

    while (!destinationSelected)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            string squareName = boardManager.GetSquareNameFromPosition(mousePosition);
            Vector2Int coord = boardManager.GetSquareCoordinates(squareName);

            if (coord.x >= 0 && coord.y >= 0 && GetPieceAtPosition(coord) == null)
            {
                targetCoord = coord;
                destinationSelected = true;
            }
        }
        yield return null;
    }

    // Move the selected piece to the target coordinate
    Vector2Int originalCoord = selectedPiece.boardPosition;
    UpdateBoardPosition(selectedPiece.gameObject, originalCoord, targetCoord);
    selectedPiece.boardPosition = targetCoord;
    selectedPiece.transform.position = boardManager.GetSquareWorldPosition(targetCoord);

    uiManager.ClearMessage();
    isCardEffectActive = false;

    // Invoke the callback to indicate the effect is complete
    onEffectComplete?.Invoke();
}


    // Revive piece card effect
    public IEnumerator RevivePiece(bool isWhitePlayer, System.Action onEffectComplete)
{
    isCardEffectActive = true;

    UIManager uiManager = FindObjectOfType<UIManager>();
    List<GameObject> capturedPieces = isWhitePlayer ? whiteCapturedPieces : blackCapturedPieces;

    if (capturedPieces.Count == 0)
    {
        uiManager.ShowMessage("No captured pieces to revive.");
        yield return new WaitForSeconds(2);
        uiManager.ClearMessage();
        isCardEffectActive = false;

        // Invoke the callback even if no piece was revived
        onEffectComplete?.Invoke();
        yield break;
    }

    // Revive the last captured piece
    GameObject pieceToRevive = capturedPieces[capturedPieces.Count - 1];
    ChessPieceMovement pieceScript = pieceToRevive.GetComponent<ChessPieceMovement>();

    // Check if the starting position is empty
    if (GetPieceAtPosition(pieceScript.startingPosition) != null)
    {
        uiManager.ShowMessage("Starting position is occupied. Cannot revive.");
        yield return new WaitForSeconds(2);
        uiManager.ClearMessage();
        isCardEffectActive = false;

        // Invoke the callback
        onEffectComplete?.Invoke();
        yield break;
    }

    // Place the piece back on the board
    pieceScript.boardPosition = pieceScript.startingPosition;
    pieceToRevive.transform.position = boardManager.GetSquareWorldPosition(pieceScript.startingPosition);
    UpdateBoardPosition(pieceToRevive, new Vector2Int(-1, -1), pieceScript.startingPosition);

    // Remove the piece from the captured list
    capturedPieces.Remove(pieceToRevive);

    // Reactivate the piece
    pieceToRevive.SetActive(true);

    uiManager.ShowMessage($"{pieceScript.tag} has been revived!");
    yield return new WaitForSeconds(2);
    uiManager.ClearMessage();

    isCardEffectActive = false;

    // Invoke the callback
    onEffectComplete?.Invoke();
}


    // Freeze piece card effect
    public IEnumerator FreezePiece(bool isWhitePlayer, int duration, System.Action onEffectComplete)
    {
        isCardEffectActive = true;

        UIManager uiManager = FindObjectOfType<UIManager>();
        uiManager.ShowMessage("Select an opponent's piece to freeze.");

        ChessPieceMovement selectedPiece = null;
        bool pieceSelected = false;

        while (!pieceSelected)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D collider = Physics2D.OverlapPoint(mousePosition);
                if (collider != null)
                {
                    ChessPieceMovement piece = collider.GetComponent<ChessPieceMovement>();
                    if (piece != null && piece.tag.StartsWith(isWhitePlayer ? "Black" : "White"))
                    {
                        selectedPiece = piece;
                        pieceSelected = true;
                    }
                }
            }
            yield return null;
        }

        // Apply the freeze effect
        selectedPiece.FreezeForTurns(duration);
        uiManager.ShowMessage($"{selectedPiece.tag} is frozen for {duration} turns!");

        yield return new WaitForSeconds(2);

        uiManager.ClearMessage();
        isCardEffectActive = false;

        // Invoke the callback to indicate the effect is complete
        onEffectComplete?.Invoke();
    }

}
