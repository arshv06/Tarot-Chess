using System.Collections.Generic;
using UnityEngine;

public static class BoardMapping
{
    public static readonly Dictionary<string, Vector2Int> SquareToCoordinates = new Dictionary<string, Vector2Int>()
    {
        { "A1", new Vector2Int(0, 0) }, { "B1", new Vector2Int(1, 0) }, // Continue for all squares...
        { "H8", new Vector2Int(7, 7) } // Final square
    };

    public static readonly Dictionary<Vector2Int, string> CoordinatesToSquare = new Dictionary<Vector2Int, string>()
    {
        { new Vector2Int(0, 0), "A1" }, { new Vector2Int(1, 0), "B1" }, // Continue for all squares...
        { new Vector2Int(7, 7), "H8" } // Final square
    };
}
