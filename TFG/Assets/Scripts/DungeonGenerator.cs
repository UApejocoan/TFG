using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject tilePrefab;
    public Transform tileParent;

    public int visibleRows = 20;
    public int rowWidth = 3;
    public int rowMargin = 2;

    private HashSet<Vector2Int> groundTiles = new HashSet<Vector2Int>();
    private Dictionary<Vector2Int, GameObject> tileVisuals = new Dictionary<Vector2Int, GameObject>();

    private int highestRow = 0;

    public HashSet<Vector2Int> GroundTiles => groundTiles;

    public void GenerateInitialMap()
    {
        for (int y = 0; y < visibleRows; y++)
            GenerateRow(y);
    }

    public void UpdateMap(float playerY)
    {
        int playerRow = Mathf.RoundToInt(playerY);

        // Genera nuevas filas si estás cerca del tope
        while (highestRow < playerRow + visibleRows)
        {
            highestRow++;
            GenerateRow(highestRow);
        }

        // Elimina filas viejas
        int lowestAllowed = Mathf.FloorToInt(playerY) - 5;

        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var pos in groundTiles)
        {
            if (pos.y < lowestAllowed)
            {
                Destroy(tileVisuals[pos]);
                tileVisuals.Remove(pos);
                toRemove.Add(pos);
            }
        }

        foreach (var pos in toRemove)
        {
            groundTiles.Remove(pos);
        }
    }

    private void GenerateRow(int y)
    {
        int startX = Random.Range(-1, 2); // -1, 0 o 1

        for (int x = startX - rowMargin; x < startX + rowWidth + rowMargin; x++)
        {
            Vector2Int pos = new Vector2Int(x, y);
            if (!groundTiles.Contains(pos))
            {
                groundTiles.Add(pos);

                var go = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity, tileParent);
                tileVisuals[pos] = go;
            }
        }
    }

    public bool IsValidPosition(Vector2Int pos)
    {
        return groundTiles.Contains(pos);
    }
}
