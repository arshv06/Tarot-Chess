using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Dictionary<string, Vector2Int> boardCoordinates = new Dictionary<string, Vector2Int>();

    private void Awake()
    {
        InitializeBoardCoordinates();
    }

    private void InitializeBoardCoordinates()
    {
        string[] columns = { "A", "B", "C", "D", "E", "F", "G", "H" };
        for (int y = 0; y < 8; y++)
        {
            int row = y + 1;
            for (int x = 0; x < 8; x++)
            {
                string squareName = columns[x] + row.ToString();
                boardCoordinates[squareName] = new Vector2Int(x, y);
            }
        }
    }

    public Vector2Int GetSquareCoordinates(string squareName)
    {
        if (boardCoordinates.TryGetValue(squareName, out Vector2Int coordinates))
        {
            Debug.Log($"Square {squareName} translated to coordinates {coordinates}");
            return coordinates;
        }
        Debug.LogError($"Square {squareName} not found!");
        return new Vector2Int(-1, -1);
    }

    public string GetSquareName(Vector2Int coordinates)
    {
        foreach (var square in boardCoordinates)
        {
            if (square.Value == coordinates)
            {
                return square.Key;
            }
        }
        Debug.LogError($"Coordinates {coordinates} do not match any square!");
        return null;
    }

    public string GetSquareNameFromCoordinates(Vector2Int coord)
{
    if (coord.x < 0 || coord.x > 7 || coord.y < 0 || coord.y > 7)
        return null;

    char file = (char)('A' + coord.x); // Files from 'A' to 'H'
    int rank = coord.y + 1; // Ranks from '1' to '8'

    return $"{file}{rank}";
}


    public bool IsSquareEmpty(string squareName)
    {
        Vector2Int coords = GetSquareCoordinates(squareName);
        Collider2D[] colliders = Physics2D.OverlapPointAll(new Vector2(coords.x, coords.y));
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != null)
            {
                return false;
            }
        }
        return true;
    }

    public Vector3 GetSquareWorldPosition(Vector2Int coordinates)
    {
        float x = coordinates.x - 3.5f;
        float y = coordinates.y - 3.5f;
        return new Vector3(x, y, 0);
    }

    public string GetSquareNameFromPosition(Vector3 position)
    {
        float squareSize = 1.0f;
        float offsetX = -3.5f; // Adjust based on your board's leftmost position
        float offsetY = -3.5f; // Adjust based on your board's bottom position

        int x = Mathf.RoundToInt((position.x - offsetX) / squareSize);
        int y = Mathf.RoundToInt((position.y - offsetY) / squareSize);
        Vector2Int approxCoord = new Vector2Int(x, y);

        string squareName = GetSquareName(approxCoord);
        return squareName;
    }
}
