using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // Dictionary to store mappings from square names (e.g., "A1") to grid coordinates (e.g., (0,0))
    public Dictionary<string, Vector2Int> boardCoordinates = new Dictionary<string, Vector2Int>();

    private void Awake()
    {
        InitializeBoardCoordinates();
    }

    // Initialize the dictionary with all square mappings
    private void InitializeBoardCoordinates()
    {
        // Row 1
        boardCoordinates["A1"] = new Vector2Int(0, 0);
        boardCoordinates["B1"] = new Vector2Int(1, 0);
        boardCoordinates["C1"] = new Vector2Int(2, 0);
        boardCoordinates["D1"] = new Vector2Int(3, 0);
        boardCoordinates["E1"] = new Vector2Int(4, 0);
        boardCoordinates["F1"] = new Vector2Int(5, 0);
        boardCoordinates["G1"] = new Vector2Int(6, 0);
        boardCoordinates["H1"] = new Vector2Int(7, 0);

        // Row 2
        boardCoordinates["A2"] = new Vector2Int(0, 1);
        boardCoordinates["B2"] = new Vector2Int(1, 1);
        boardCoordinates["C2"] = new Vector2Int(2, 1);
        boardCoordinates["D2"] = new Vector2Int(3, 1);
        boardCoordinates["E2"] = new Vector2Int(4, 1);
        boardCoordinates["F2"] = new Vector2Int(5, 1);
        boardCoordinates["G2"] = new Vector2Int(6, 1);
        boardCoordinates["H2"] = new Vector2Int(7, 1);

        // Row 3
        boardCoordinates["A3"] = new Vector2Int(0, 2);
        boardCoordinates["B3"] = new Vector2Int(1, 2);
        boardCoordinates["C3"] = new Vector2Int(2, 2);
        boardCoordinates["D3"] = new Vector2Int(3, 2);
        boardCoordinates["E3"] = new Vector2Int(4, 2);
        boardCoordinates["F3"] = new Vector2Int(5, 2);
        boardCoordinates["G3"] = new Vector2Int(6, 2);
        boardCoordinates["H3"] = new Vector2Int(7, 2);

        // Row 4
        boardCoordinates["A4"] = new Vector2Int(0, 3);
        boardCoordinates["B4"] = new Vector2Int(1, 3);
        boardCoordinates["C4"] = new Vector2Int(2, 3);
        boardCoordinates["D4"] = new Vector2Int(3, 3);
        boardCoordinates["E4"] = new Vector2Int(4, 3);
        boardCoordinates["F4"] = new Vector2Int(5, 3);
        boardCoordinates["G4"] = new Vector2Int(6, 3);
        boardCoordinates["H4"] = new Vector2Int(7, 3);

        // Row 5
        boardCoordinates["A5"] = new Vector2Int(0, 4);
        boardCoordinates["B5"] = new Vector2Int(1, 4);
        boardCoordinates["C5"] = new Vector2Int(2, 4);
        boardCoordinates["D5"] = new Vector2Int(3, 4);
        boardCoordinates["E5"] = new Vector2Int(4, 4);
        boardCoordinates["F5"] = new Vector2Int(5, 4);
        boardCoordinates["G5"] = new Vector2Int(6, 4);
        boardCoordinates["H5"] = new Vector2Int(7, 4);

        // Row 6
        boardCoordinates["A6"] = new Vector2Int(0, 5);
        boardCoordinates["B6"] = new Vector2Int(1, 5);
        boardCoordinates["C6"] = new Vector2Int(2, 5);
        boardCoordinates["D6"] = new Vector2Int(3, 5);
        boardCoordinates["E6"] = new Vector2Int(4, 5);
        boardCoordinates["F6"] = new Vector2Int(5, 5);
        boardCoordinates["G6"] = new Vector2Int(6, 5);
        boardCoordinates["H6"] = new Vector2Int(7, 5);

        // Row 7
        boardCoordinates["A7"] = new Vector2Int(0, 6);
        boardCoordinates["B7"] = new Vector2Int(1, 6);
        boardCoordinates["C7"] = new Vector2Int(2, 6);
        boardCoordinates["D7"] = new Vector2Int(3, 6);
        boardCoordinates["E7"] = new Vector2Int(4, 6);
        boardCoordinates["F7"] = new Vector2Int(5, 6);
        boardCoordinates["G7"] = new Vector2Int(6, 6);
        boardCoordinates["H7"] = new Vector2Int(7, 6);

        // Row 8
        boardCoordinates["A8"] = new Vector2Int(0, 7);
        boardCoordinates["B8"] = new Vector2Int(1, 7);
        boardCoordinates["C8"] = new Vector2Int(2, 7);
        boardCoordinates["D8"] = new Vector2Int(3, 7);
        boardCoordinates["E8"] = new Vector2Int(4, 7);
        boardCoordinates["F8"] = new Vector2Int(5, 7);
        boardCoordinates["G8"] = new Vector2Int(6, 7);
        boardCoordinates["H8"] = new Vector2Int(7, 7);
    }

    // Get the coordinates of a square by its name
    public Vector2Int GetSquareCoordinates(string squareName)
    {
        if (boardCoordinates.TryGetValue(squareName, out Vector2Int coordinates))
        {
            return coordinates;
        }
        Debug.LogError($"Square {squareName} not found!");
        return new Vector2Int(-1, -1); // Return an invalid coordinate if the square doesn't exist
    }

    // Optional: Get the square name by coordinates (e.g., from (0,0) to "A1")
    public string GetSquareName(Vector2Int coordinates)
    {
        foreach (var kvp in boardCoordinates)
        {
            if (kvp.Value == coordinates)
            {
                return kvp.Key;
            }
        }
        Debug.LogError($"Coordinates {coordinates} do not match any square!");
        return null;
    }
}
