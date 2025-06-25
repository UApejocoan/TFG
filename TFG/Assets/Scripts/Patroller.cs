using UnityEngine;
using UnityEngine.SceneManagement;

public class Patroller : MonoBehaviour
{
    public float speed = 2f;
    public float patrolRange = 2f;

    private Vector3 startPosition;
    private int direction = 1;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.position += Vector3.right * speed * direction * Time.deltaTime;

        if (Mathf.Abs(transform.position.x - startPosition.x) >= patrolRange)
        {
            direction *= -1;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("El patrullero mató al jugador");
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            GameObject.FindObjectOfType<GameManager>().GameOver(Mathf.FloorToInt(transform.position.y));
        }
    }
}
