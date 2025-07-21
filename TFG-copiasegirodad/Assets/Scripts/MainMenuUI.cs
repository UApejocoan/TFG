using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene"); // Cambiá esto por el nombre de tu escena principal
    }

    public void OpenShop()
    {
        SceneManager.LoadScene("ShopScene"); // Creamos esta después
    }

    public void ChangePlayer()
    {
        SceneManager.LoadScene("CharacterSelectScene"); // La haremos luego
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Salir del juego (solo funciona fuera del editor)");
    }
}
