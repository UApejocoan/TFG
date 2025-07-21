using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    public static GridMap Instance;

    // Posiciones donde hay suelo
    public HashSet<Vector2Int> groundTiles = new HashSet<Vector2Int>();

    private void Awake()
    {
        Instance = this;

        // Demo: Agregamos suelo en algunas casillas
        for (int x = -5; x <= 5; x++)
        {
            for (int y = -5; y <= 5; y++)
            {
                groundTiles.Add(new Vector2Int(x, y));
            }
        }
    }

    public bool IsGround(Vector2Int gridPos)
    {
        return groundTiles.Contains(gridPos);
    }
}
