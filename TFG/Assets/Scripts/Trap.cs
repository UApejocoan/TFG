using UnityEngine;
using UnityEngine.SceneManagement;
public class SpikeTrap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Pinchos mataron al jugador");
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            GameObject.FindObjectOfType<GameManager>().GameOver(Mathf.FloorToInt(transform.position.y));
        }
    }
}