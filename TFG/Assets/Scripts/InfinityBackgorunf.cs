using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    public Transform player;
    public float scrollSpeed = 1f;
    public Transform background1;
    public Transform background2;

    private float height;

    void Start()
    {
        height = background1.GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void Update()
    {
        // Mover los fondos en función del jugador
        float playerY = player.position.y;
        transform.position = new Vector3(0, playerY, 10); // ancla el fondo a la cámara o jugador

        // Reposicionar si uno se va por debajo
        if (playerY > background1.position.y + height)
        {
            background1.position += new Vector3(0, 2 * height-0.5f, 0);
            SwapBackgrounds();
        }
        else if (playerY > background2.position.y + height)
        {
            background2.position += new Vector3(0, 2 * height-0.5f, 0);
            SwapBackgrounds();
        }
    }

    void SwapBackgrounds()
    {
        // Esto asegura que siempre background1 sea el más bajo
        if (background2.position.y > background1.position.y)
        {
            var temp = background1;
            background1 = background2;
            background2 = temp;
        }
    }
}
