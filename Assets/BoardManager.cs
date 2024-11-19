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
}
