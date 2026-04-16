using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaLoader : MonoBehaviour
{
    [SerializeField] private string forestSceneName = "ForestArena";
    [SerializeField] private string factorySceneName = "FactoryArena";

    private string selectedSceneName = "";

    public void SelectForestArena()
    {
        selectedSceneName = forestSceneName;
        Debug.Log("Selected arena: ForestArena");
    }

    public void SelectFactoryArena()
    {
        selectedSceneName = factorySceneName;
        Debug.Log("Selected arena: FactoryArena");
    }

    public void StartGame()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogWarning("No NetworkManager found.");
            return;
        }

        if (!NetworkManager.Singleton.IsListening)
        {
            Debug.LogWarning("Network session has not started yet.");
            return;
        }

        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("Only the host can start the game.");
            return;
        }

        if (string.IsNullOrEmpty(selectedSceneName))
        {
            Debug.LogWarning("No arena selected.");
            return;
        }

        Debug.Log("Loading arena: " + selectedSceneName);
        NetworkManager.Singleton.SceneManager.LoadScene(selectedSceneName, LoadSceneMode.Single);
    }
}