using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float tileSize = 1f;

    private bool isMoving = false;
    private Vector3 targetPosition;

    // Mapa lógico: casillas donde hay suelo
    private HashSet<Vector2Int> groundTiles = new HashSet<Vector2Int>();

    private Vector2 touchStartPos;
    private bool touchMoved = false;
    private float swipeThreshold = 50f;

    public GameObject groundTilePrefab;

    void Start()
    {
        targetPosition = transform.position;

        // Crear mapa simple (puede reemplazarse por generación o tilemap)
        GenerateDungeon(50); // cantidad de pasos aleatorios
        GenerateVisualMap();
    }

    void Update()
    {
        if (!isMoving)
        {
            Vector3 direction = Vector3.zero;

            // Detectar swipe táctil
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    touchStartPos = touch.position;
                    touchMoved = false;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    touchMoved = true;
                }
                else if (touch.phase == TouchPhase.Ended && touchMoved)
                {
                    Vector2 touchEndPos = touch.position;
                    Vector2 delta = touchEndPos - touchStartPos;

                    if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    {
                        direction = (delta.x > 0) ? new Vector3(tileSize, 0, 0) : new Vector3(-tileSize, 0, 0);
                    }
                    else
                    {
                        direction = (delta.y > 0) ? new Vector3(0, tileSize, 0) : new Vector3(0, -tileSize, 0);
                    }
                }
            }

#if UNITY_EDITOR
            // Para pruebas con teclado en el editor
            if (Input.GetKeyDown(KeyCode.W)) direction = new Vector3(0, tileSize, 0);
            else if (Input.GetKeyDown(KeyCode.S)) direction = new Vector3(0, -tileSize, 0);
            else if (Input.GetKeyDown(KeyCode.A)) direction = new Vector3(-tileSize, 0, 0);
            else if (Input.GetKeyDown(KeyCode.D)) direction = new Vector3(tileSize, 0, 0);
#endif

            if (direction != Vector3.zero)
            {
                Vector3 destination = transform.position + direction;
                Vector2Int gridPos = WorldToGrid(destination);

                if (groundTiles.Contains(gridPos))
                {
                    targetPosition = destination;
                    StartCoroutine(MoveToTarget());
                }
                else
                {
                    StartCoroutine(FallDown(destination));
                }
            }
        }
    }

    private IEnumerator MoveToTarget()
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }

    private IEnumerator FallDown(Vector3 fallPosition)
    {
        isMoving = true;

        float fallTime = 0.5f;
        float timer = 0f;
        Vector3 start = transform.position;
        Vector3 end = fallPosition + Vector3.down * 5f;

        while (timer < fallTime)
        {
            transform.position = Vector3.Lerp(start, end, timer / fallTime);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        isMoving = false;

        Debug.Log("¡El jugador se cayó!");
        // Aquí podés reiniciar el nivel, mostrar animación o UI
    }

    // Convierte una posición del mundo a una posición en la grilla
    private Vector2Int WorldToGrid(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }

    private void GenerateVisualMap()
    {
        foreach (Vector2Int tile in groundTiles)
        {
            Vector3 worldPos = new Vector3(tile.x, tile.y, 0);
            Instantiate(groundTilePrefab, worldPos, Quaternion.identity);
        }
    }

    private void GenerateDungeon(int height)
    {
        groundTiles.Clear();

        int currentY = 0;
        int pathX = 0; // posición actual en X del camino principal

        for (int y = 0; y < height; y++)
        {
            currentY++;

            // Randomiza ancho del camino para esta fila
            int pathWidth = Random.Range(2, 5); // entre 2 y 4 casillas de ancho

            // Randomiza desplazamiento horizontal (sin alejarse mucho)
            int offsetX = Mathf.Clamp(pathX + Random.Range(-1, 2), -5, 5);

            for (int x = offsetX; x < offsetX + pathWidth; x++)
            {
                Vector2Int tile = new Vector2Int(x, currentY);
                groundTiles.Add(tile);
            }

            // Elige nueva posición central del camino para la próxima fila
            pathX = offsetX + Random.Range(0, pathWidth);
        }

        // Posición inicial del jugador (abajo del dungeon)
        transform.position = new Vector3(0, 1, 0);
        targetPosition = transform.position;
    }


    private Vector2Int GetVerticalPathDirection()
    {
        // Solo permitimos arriba, arriba-izquierda o arriba-derecha
        Vector2Int[] directions = new Vector2Int[]
        {
        Vector2Int.up,                        // recto hacia arriba
        Vector2Int.up + Vector2Int.left,     // diagonal izquierda
        Vector2Int.up + Vector2Int.right     // diagonal derecha
        };

        return directions[Random.Range(0, directions.Length)];
    }

    private Vector2Int GetRandomDirection()
    {
        Vector2Int[] directions = new Vector2Int[]
        {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
        };

        return directions[Random.Range(0, directions.Length)];
    }
}
