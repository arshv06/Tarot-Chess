Chess Game Project
This project is a 2D Chess game built using Unity. The game features standard chess rules and logic but with an added upcoming twist (to be added)

Features
Efficient Movement Validation: Uses a BoardMapping system to convert named squares into numeric coordinates for easy validation.
Getting Started
Prerequisites
Unity Editor (version 2021 or later recommended)
Basic understanding of Unity and C# scripting
Installation
Clone the repository:
bash
Copy code
git clone https://github.com/your-username/chess-game.git
Open the project in Unity.
Play the game in the Unity Editor to start playing chess!
Code Structure
Scripts
ChessPieceMovement.cs: Handles the movement logic for each chess piece and validates moves using the BoardMapping system.
BoardMapping.cs: Contains dictionaries to map square names (e.g., A1, H7) to numeric coordinates and vice versa.

Board Mapping
The BoardMapping script maps named squares to an 8x8 numeric grid for efficient movement validation.
Example:
csharp
Copy code
public static readonly Dictionary<string, Vector2Int> SquareToCoordinates = new Dictionary<string, Vector2Int>()
{
    { "A1", new Vector2Int(0, 0) },
    { "H8", new Vector2Int(7, 7) }
    // Add all squares
};
How to Play
Drag and drop pieces to make moves.
Follow standard chess rules.
Future Improvements
Implement special chess rules like castling, en passant, and pawn promotion.
Add visual feedback for selected pieces and valid moves.
Added Special Sauce

License
MIT
