using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject clientPrefab, serverPrefab;

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void CreateServer()
    {
        Instantiate(serverPrefab);
        SceneManager.LoadScene("Game");
    }

    public void JoinServer()
    {
        Instantiate(clientPrefab);
        SceneManager.LoadScene("Spectate");
    }
}