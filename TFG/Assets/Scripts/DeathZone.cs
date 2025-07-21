using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathZone : MonoBehaviour
{
    public float speed = 1f;
    public Transform player;
    private bool hasTriggeredGameOver = false;

    void Update()
    {
        if (hasTriggeredGameOver) return;

        transform.position += Vector3.up * speed * Time.deltaTime;

        transform.position = new Vector3(player.position.x, transform.position.y, transform.position.z);

        if (transform.position.y > player.position.y - 0.5f)
        {
            Debug.Log("El ente atrapó al jugador");
            hasTriggeredGameOver = true;
            GameObject.FindObjectOfType<GameManager>().GameOver(Mathf.FloorToInt(transform.position.y));
        }
    }
}
