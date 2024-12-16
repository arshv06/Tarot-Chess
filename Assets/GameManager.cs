using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    private List<TransformedPawn> transformedPawns = new List<TransformedPawn>();

    // Board representation
    public GameObject[,] board = new GameObject[8, 8];

    // Tracking the last move
    public GameObject lastMovedPiece = null;
    public Vector2Int lastMoveFrom;
    public Vector2Int lastMoveTo;
    public GameObject capturedPieceOnLastMove = null;

    private BoardManager boardManager;

    // Card system variables
    public Deck deck;
    public Hand whitePlayerHand = new Hand();
    public Hand blackPlayerHand = new Hand();
    public bool isCardEffectActive = false;

    // Lists to track captured pieces for each player
    public List<GameObject> whiteCapturedPieces = new List<GameObject>();
    public List<GameObject> blackCapturedPieces = new List<GameObject>();

    // Move history
    public List<string> moveHistory = new List<string>();

    // Timer variables
    public float whitePlayerTime = 600f; // 10 minutes in seconds
    public float blackPlayerTime = 600f;

    [Header("Queen Prefabs")]
    public GameObject whiteQueenPrefab; // Assign via Inspector
    public GameObject blackQueenPrefab;

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

    private void Update()
    {
        UpdateTimers();
    }

    // Method to update player timers
    private void UpdateTimers()
{
    if (isCardEffectActive)
        return; // Do not update timers during card effects

    if (isWhiteTurn)
    {
        whitePlayerTime -= Time.deltaTime;
        if (whitePlayerTime <= 0)
        {
            whitePlayerTime = 0;
            EndGameDueToTimeout(false);
        }
    }
    else
    {
        blackPlayerTime -= Time.deltaTime;
        if (blackPlayerTime <= 0)
        {
            blackPlayerTime = 0;
            EndGameDueToTimeout(true);
        }
    }

    UIManager uiManager = FindObjectOfType<UIManager>();
    uiManager.UpdateTimer(true, whitePlayerTime);
    uiManager.UpdateTimer(false, blackPlayerTime);
}
    // Method to handle game end due to timeout
    private void EndGameDueToTimeout(bool whiteWins)
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        uiManager.ShowEndGamePanel(whiteWins ? "White" : "Black");
        enabled = false; // Stop the game
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

    public void UpdateBoardPosition(GameObject piece, Vector2Int? oldPos, Vector2Int? newPos)
{
    if (oldPos.HasValue && oldPos.Value.x >= 0 && oldPos.Value.x < 8 && oldPos.Value.y >= 0 && oldPos.Value.y < 8)
        board[oldPos.Value.x, oldPos.Value.y] = null;

    if (newPos.HasValue && newPos.Value.x >= 0 && newPos.Value.x < 8 && newPos.Value.y >= 0 && newPos.Value.y < 8)
        board[newPos.Value.x, newPos.Value.y] = piece;

    lastMovedPiece = piece;
    lastMoveFrom = oldPos ?? new Vector2Int(-1, -1);
    lastMoveTo = newPos ?? new Vector2Int(-1, -1);
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
    EndTurnEffects();

    isWhiteTurn = !isWhiteTurn;

    // Check for checkmate after the turn switches
    if (IsCheckmate(isWhiteTurn))
    {
        Debug.Log((isWhiteTurn ? "White" : "Black") + " is in checkmate!");
        UIManager uiManager = FindObjectOfType<UIManager>();
        uiManager.ShowEndGamePanel(isWhiteTurn ? "Black" : "White");
        enabled = false; // Stop the game
    }
    else if (IsKingInCheck(isWhiteTurn))
    {
        Debug.Log((isWhiteTurn ? "White" : "Black") + " is in check!");
        UIManager uiManager = FindObjectOfType<UIManager>();
        uiManager.ShowMessage((isWhiteTurn ? "White" : "Black") + " is in check!");
    }
}


    public bool IsWhiteTurn()
    {
        return isWhiteTurn;
    }

    // Method to handle end-of-turn effects
   public void EndTurnEffects()
{
    UIManager uiManager = FindObjectOfType<UIManager>();

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
                uiManager.ShowMessage($"{piece.tag} is no longer frozen!");
            }
        }
    }

    // Process transformed pawns
    for (int i = transformedPawns.Count - 1; i >= 0; i--)
    {
        TransformedPawn tp = transformedPawns[i];
        tp.turnsRemaining--;

        if (tp.turnsRemaining <= 0)
        {
            // Revert back to pawn
            Vector2Int position = tp.queen.GetComponent<ChessPieceMovement>().boardPosition;

            // Remove queen from board
            UpdateBoardPosition(tp.queen, position, null);
            tp.queen.SetActive(false);

            // Reactivate pawn
            tp.pawn.SetActive(true);
            tp.pawn.transform.position = tp.queen.transform.position;
            tp.pawn.GetComponent<ChessPieceMovement>().boardPosition = position;

            // Update board
            UpdateBoardPosition(tp.pawn, null, position);
            board[position.x, position.y] = tp.pawn;

            // Destroy queen GameObject
            Destroy(tp.queen);

            // Remove from transformedPawns list
            transformedPawns.RemoveAt(i);

            uiManager.ShowMessage("Pawn has reverted back.");
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

        int index = UnityEngine.Random.Range(0, availableCards.Count);
        Card drawnCard = availableCards[index];
        deck.cards.Remove(drawnCard);
        return drawnCard;
    }

    // Teleport card effect
    public IEnumerator TeleportPiece(bool isWhitePlayer, Action onEffectComplete)
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
    public IEnumerator RevivePiece(bool isWhitePlayer, Action onEffectComplete)
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
    public IEnumerator FreezePiece(bool isWhitePlayer, int duration, Action onEffectComplete)
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

    // Queen Transformation card effect
   public class TransformedPawn
{
    public GameObject pawn;
    public GameObject queen;
    public int turnsRemaining;

    public TransformedPawn(GameObject pawn, GameObject queen, int turns)
    {
        this.pawn = pawn;
        this.queen = queen;
        this.turnsRemaining = turns;
    }
}

// Then modify your TransformPawnToQueen coroutine as follows:
public IEnumerator TransformPawnToQueen(bool isWhitePlayer, Action onEffectComplete)
{
    isCardEffectActive = true;
    UIManager uiManager = FindObjectOfType<UIManager>();
    uiManager.ShowMessage("Select a pawn to transform into a queen.");

    // Select the pawn
    ChessPieceMovement selectedPawn = null;
    bool pawnSelected = false;

    while (!pawnSelected)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D collider = Physics2D.OverlapPoint(mousePosition);

            if (collider != null)
            {
                ChessPieceMovement piece = collider.GetComponent<ChessPieceMovement>();
                if (piece != null && piece.tag.StartsWith(isWhitePlayer ? "WhitePawn" : "BlackPawn"))
                {
                    selectedPawn = piece;
                    pawnSelected = true;
                }
            }
        }
        yield return null;
    }

    // Transform pawn to queen
    GameObject pawnGO = selectedPawn.gameObject;
    Vector2Int position = selectedPawn.boardPosition;

    // Instantiate queen prefab at pawn's position
    GameObject queenPrefab = isWhitePlayer ? whiteQueenPrefab : blackQueenPrefab;
    if (queenPrefab == null)
    {
        Debug.LogError("Queen prefab not assigned in GameManager!");
        isCardEffectActive = false;
        onEffectComplete?.Invoke();
        yield break;
    }

    GameObject queenGO = Instantiate(queenPrefab, pawnGO.transform.position, Quaternion.identity);
    ChessPieceMovement queenScript = queenGO.GetComponent<ChessPieceMovement>();
    queenScript.boardPosition = position;

    // Update board
    UpdateBoardPosition(queenGO, new Vector2Int(-1, -1), position);
    board[position.x, position.y] = queenGO;

    // Deactivate the pawn
    selectedPawn.gameObject.SetActive(false);

    // Remove the pawn from captured list if it's there
    whiteCapturedPieces.Remove(selectedPawn.gameObject);
    blackCapturedPieces.Remove(selectedPawn.gameObject);

    // Add to transformedPawns list
    transformedPawns.Add(new TransformedPawn(selectedPawn.gameObject, queenGO, 5)); // 5 turns

    uiManager.ClearMessage();
    isCardEffectActive = false;

    // Invoke the callback to indicate the effect is complete
    onEffectComplete?.Invoke();
}

    // Reverse last move
    public void ReverseLastMove()
    {
        if (lastMovedPiece == null)
        {
            Debug.Log("No move to reverse.");
            return;
        }

        // Move the piece back to its original position
        Vector2Int currentPos = lastMoveTo;
        Vector2Int originalPos = lastMoveFrom;

        UpdateBoardPosition(lastMovedPiece, currentPos, originalPos);
        lastMovedPiece.GetComponent<ChessPieceMovement>().boardPosition = originalPos;
        lastMovedPiece.transform.position = boardManager.GetSquareWorldPosition(originalPos);

        // Reactivate captured piece if any
        if (capturedPieceOnLastMove != null)
        {
            capturedPieceOnLastMove.SetActive(true);
            ChessPieceMovement capturedPieceScript = capturedPieceOnLastMove.GetComponent<ChessPieceMovement>();
            capturedPieceScript.boardPosition = currentPos;
            capturedPieceOnLastMove.transform.position = boardManager.GetSquareWorldPosition(currentPos);
            UpdateBoardPosition(capturedPieceOnLastMove, new Vector2Int(-1, -1), currentPos);

            // Remove from captured pieces list
            if (capturedPieceOnLastMove.tag.StartsWith("White"))
                whiteCapturedPieces.Remove(capturedPieceOnLastMove);
            else
                blackCapturedPieces.Remove(capturedPieceOnLastMove);

            capturedPieceOnLastMove = null;
        }
        else
        {
            // No piece was captured, so clear the board position
            board[currentPos.x, currentPos.y] = null;
        }

        // Remove the last move from move history
        if (moveHistory.Count > 0)
        {
            moveHistory.RemoveAt(moveHistory.Count - 1);
            UIManager uiManager = FindObjectOfType<UIManager>();
            uiManager.UpdateMoveHistory(moveHistory);
        }

        // Update captured pieces UI
        UpdateCapturedPiecesUI();

        // Reset last moved piece
        lastMovedPiece = null;
    }

    // Update the captured pieces UI
    public void UpdateCapturedPiecesUI()
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        uiManager.UpdateCapturedPieces(whiteCapturedPieces, blackCapturedPieces);
    }

    // Move history methods

    // Method to generate move notation
    public string GenerateMoveNotation(ChessPieceMovement piece, Vector2Int from, Vector2Int to, bool isCapture)
    {
        string notation = "";

        // Get piece notation (e.g., N for Knight, B for Bishop)
        string pieceNotation = GetPieceNotation(piece);

        // Determine if it's a capture
        string captureNotation = isCapture ? "x" : "";

        // Get the destination square name
        string toSquare = boardManager.GetSquareNameFromCoordinates(to);

        // Handle pawn moves
        if (piece.tag.Contains("Pawn"))
        {
            if (isCapture)
            {
                // For pawn captures, include the file of the pawn's starting position
                char fromFile = GetFileChar(from.x);
                notation = $"{fromFile}x{toSquare}";
            }
            else
            {
                notation = toSquare;
            }
        }
        else
        {
            notation = $"{pieceNotation}{captureNotation}{toSquare}";
        }

        // TODO: Add disambiguation, check, checkmate, castling, promotions if necessary

        return notation;
    }

    // Method to add move to history and maintain last ten moves
    public void AddMoveToHistory(string move)
    {
        moveHistory.Add(move);

        // Keep only the last ten moves
        if (moveHistory.Count > 12)
        {
            moveHistory.RemoveAt(0);
        }

        // Update the move history UI
        UIManager uiManager = FindObjectOfType<UIManager>();
        uiManager.UpdateMoveHistory(moveHistory);
    }

    // Helper methods
    private string GetPieceNotation(ChessPieceMovement piece)
    {
        if (piece.tag.Contains("Knight")) return "N";
        if (piece.tag.Contains("Bishop")) return "B";
        if (piece.tag.Contains("Rook")) return "R";
        if (piece.tag.Contains("Queen")) return "Q";
        if (piece.tag.Contains("King")) return "K";
        // Pawns are represented by an empty string
        return "";
    }

    private char GetFileChar(int x)
    {
        // Files are from 'a' to 'h' corresponding to x = 0 to 7
        return (char)('a' + x);
    }

    // Check and Checkmate Detection Methods

    public bool IsKingInCheck(bool isWhitePlayer)
    {
        Vector2Int kingPosition = FindKingPosition(isWhitePlayer);
        return IsSquareUnderAttack(kingPosition, !isWhitePlayer);
    }

    private Vector2Int FindKingPosition(bool isWhitePlayer)
    {
        foreach (GameObject piece in board)
        {
            if (piece != null)
            {
                ChessPieceMovement pieceScript = piece.GetComponent<ChessPieceMovement>();
                if (pieceScript.tag == (isWhitePlayer ? "WhiteKing" : "BlackKing"))
                {
                    return pieceScript.boardPosition;
                }
            }
        }
        return new Vector2Int(-1, -1); // King not found
    }

    public bool IsSquareUnderAttack(Vector2Int square, bool byWhite)
    {
        // Check all enemy pieces to see if they can move to the square
        foreach (GameObject piece in board)
        {
            if (piece != null)
            {
                ChessPieceMovement pieceScript = piece.GetComponent<ChessPieceMovement>();
                if (pieceScript.tag.StartsWith(byWhite ? "White" : "Black"))
                {
                    if (pieceScript.CanMoveTo(square))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool IsCheckmate(bool isWhitePlayer)
    {
        if (!IsKingInCheck(isWhitePlayer))
            return false;

        // Get all player's pieces
        List<ChessPieceMovement> playerPieces = new List<ChessPieceMovement>();
        foreach (GameObject piece in board)
        {
            if (piece != null)
            {
                ChessPieceMovement pieceScript = piece.GetComponent<ChessPieceMovement>();
                if (pieceScript.tag.StartsWith(isWhitePlayer ? "White" : "Black"))
                {
                    playerPieces.Add(pieceScript);
                }
            }
        }

        // Check if any legal move can get the king out of check
        foreach (ChessPieceMovement piece in playerPieces)
        {
            List<Vector2Int> possibleMoves = piece.GetPossibleMoves();
            foreach (Vector2Int move in possibleMoves)
            {
                // Simulate the move
                Vector2Int originalPosition = piece.boardPosition;
                GameObject capturedPiece = GetPieceAtPosition(move);
                UpdateBoardPosition(piece.gameObject, originalPosition, move);
                piece.boardPosition = move;

                bool isStillInCheck = IsKingInCheck(isWhitePlayer);

                // Undo the move
                UpdateBoardPosition(piece.gameObject, move, originalPosition);
                piece.boardPosition = originalPosition;
                if (capturedPiece != null)
                {
                    UpdateBoardPosition(capturedPiece, new Vector2Int(-1, -1), move);
                }

                if (!isStillInCheck)
                {
                    // Legal move found to escape check
                    return false;
                }
            }
        }

        // No legal moves to escape check
        return true;
    }

    // Method to add time to a player's clock
    public void AddTimeToPlayer(bool isWhitePlayer, int seconds)
    {
        if (isWhitePlayer)
        {
            whitePlayerTime += seconds;
        }
        else
        {
            blackPlayerTime += seconds;
        }
        UIManager uiManager = FindObjectOfType<UIManager>();
        uiManager.UpdateTimer(isWhitePlayer, isWhitePlayer ? whitePlayerTime : blackPlayerTime);
    }
}
