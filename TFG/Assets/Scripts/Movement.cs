using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float tileSize = 1f;

    private bool isMoving = false;
    private Vector3 targetPosition;

    public GameObject spikePrefab;
    public GameObject patrollerPrefab;

    private HashSet<Vector2Int> groundTiles = new HashSet<Vector2Int>();
    private Dictionary<Vector2Int, GameObject> tileObjects = new Dictionary<Vector2Int, GameObject>();

    private Vector2 touchStartPos;
    private bool touchMoved = false;
    private float swipeThreshold = 50f;

    public GameObject groundTilePrefab;

    private int generatedHeight = 0;
    private int chunkSize = 10;
    private int safeRowsBelow = 15;

    void Start()
    {
        transform.position = new Vector3(0, 1, 0);
        targetPosition = transform.position;
        GenerateNewChunk(); // Genera el primer bloque
    }

    void Update()
    {
        if (!isMoving)
        {
            Vector3 direction = Vector3.zero;

            // Input táctil
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
                    Vector2 delta = touch.position - touchStartPos;

                    if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                        direction = (delta.x > 0) ? Vector3.right * tileSize : Vector3.left * tileSize;
                    else
                        direction = (delta.y > 0) ? Vector3.up * tileSize : Vector3.down * tileSize;
                }
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.W)) direction = Vector3.up * tileSize;
            else if (Input.GetKeyDown(KeyCode.S)) direction = Vector3.down * tileSize;
            else if (Input.GetKeyDown(KeyCode.A)) direction = Vector3.left * tileSize;
            else if (Input.GetKeyDown(KeyCode.D)) direction = Vector3.right * tileSize;
#endif

            if (direction != Vector3.zero)
            {
                Vector3 destination = transform.position + direction;
                Vector2Int gridPos = WorldToGrid(destination);

                if (groundTiles.Contains(gridPos))
                {
                    targetPosition = new Vector3(gridPos.x, gridPos.y, 0);
                    StartCoroutine(MoveToTarget());
                }
                else
                {
                    StartCoroutine(FallAndDie());
                }
            }
        }

        CheckAndExpandMap();
        CleanupOldTiles();
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

    private IEnumerator FallAndDie()
    {
        isMoving = true;

        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = Vector3.zero;
        Vector3 fallOffset = new Vector3(0, -1f, 0);

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + fallOffset;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            transform.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        //yield return new WaitForSeconds(0.5f);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        GameObject.FindObjectOfType<GameManager>().GameOver(Mathf.FloorToInt(transform.position.y));
    }

    private Vector2Int WorldToGrid(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }

    private void CheckAndExpandMap()
    {
        int playerY = Mathf.RoundToInt(transform.position.y);

        if (playerY + 10 > generatedHeight)
        {
            GenerateNewChunk();
        }
    }

    private void GenerateNewChunk()
    {
        int pathX = 0;

        for (int y = generatedHeight - 1; y >= 0; y--)
        {
            for (int x = -10; x <= 10; x++)
            {
                if (groundTiles.Contains(new Vector2Int(x, y)))
                {
                    pathX = x;
                    break;
                }
            }
            if (pathX != 0) break;
        }

        for (int i = 0; i < chunkSize; i++)
        {
            generatedHeight++;

            int pathWidth = Random.value > 0.2f ? 3 : Random.Range(2, 5);
            int offsetX = Mathf.Clamp(pathX + Random.Range(-1, 2), -5, 5);

            for (int x = offsetX; x < offsetX + pathWidth; x++)
            {
                Vector2Int tile = new Vector2Int(x, generatedHeight);
                if (!groundTiles.Contains(tile))
                {
                    groundTiles.Add(tile);
                    GameObject obj = Instantiate(groundTilePrefab, new Vector3(x, generatedHeight, 0), Quaternion.identity);
                    tileObjects[tile] = obj;
                }
            }

            pathX = offsetX + Random.Range(0, pathWidth);

            //
             if (Random.value < 0.2f)  // 20% de probabilidad por fila
        {
            int trapX = Random.Range(offsetX, offsetX + pathWidth);
            Vector2Int trapTile = new Vector2Int(trapX, generatedHeight);

             GameObject spike = Instantiate(spikePrefab, new Vector3(trapTile.x, trapTile.y, 0), Quaternion.identity);
            tileObjects[trapTile] = spike;  // opcional, para limpiar luego
        }

        // Agrega patrulleros
        if (Random.value < 0.1f)
        {
            int patrolX = offsetX + pathWidth / 2;
            GameObject enemy = Instantiate(patrollerPrefab, new Vector3(patrolX, generatedHeight, 0), Quaternion.identity);
        }
        ///
        }

        ///
       
    }

    private void CleanupOldTiles()
    {
        int playerY = Mathf.RoundToInt(transform.position.y);
        List<Vector2Int> toRemove = new List<Vector2Int>();

        foreach (var tile in groundTiles)
        {
            if (tile.y < playerY - safeRowsBelow)
            {
                toRemove.Add(tile);
            }
        }

        foreach (var tile in toRemove)
        {
            groundTiles.Remove(tile);
            if (tileObjects.ContainsKey(tile))
            {
                Destroy(tileObjects[tile]);
                tileObjects.Remove(tile);
            }
        }
    }
}
