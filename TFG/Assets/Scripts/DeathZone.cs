using UnityEngine;
using UnityEngine.SceneManagement;
public class DeathZone : MonoBehaviour
{
    public float speed = 1f;
    public Transform player;

    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;

        if (transform.position.y > player.position.y - 0.5f)
        {
            Debug.Log("El ente atrapó al jugador");
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            GameObject.FindObjectOfType<GameManager>().GameOver(Mathf.FloorToInt(transform.position.y));
        }
    }
}