using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private TMP_InputField portInput;

    [SerializeField] private GameObject connectionPanel;
    [SerializeField] private GameObject arenaSelectPanel;

    private const ushort DefaultPort = 7777;

    private void Start()
    {
        if (connectionPanel != null)
            connectionPanel.SetActive(true);

        if (arenaSelectPanel != null)
            arenaSelectPanel.SetActive(false);
    }

    public void StartHost()
    {
        ushort port = GetPort();
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        transport.SetConnectionData("127.0.0.1", port, "0.0.0.0");

        if (NetworkManager.Singleton.StartHost())
        {
            ShowArenaSelect();
            Debug.Log("Host started.");
        }
    }

    public void StartClient()
    {
        string ip = string.IsNullOrWhiteSpace(ipInput.text) ? "127.0.0.1" : ipInput.text.Trim();
        ushort port = GetPort();

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, port);

        if (NetworkManager.Singleton.StartClient())
        {
            HideConnectionOnly();
            Debug.Log($"Client started. Connecting to {ip}:{port}");
        }
    }

    private ushort GetPort()
    {
        if (ushort.TryParse(portInput.text, out ushort parsedPort))
            return parsedPort;

        return DefaultPort;
    }

    private void ShowArenaSelect()
    {
        if (connectionPanel != null)
            connectionPanel.SetActive(false);

        if (arenaSelectPanel != null)
            arenaSelectPanel.SetActive(true);
    }

    private void HideConnectionOnly()
    {
        if (connectionPanel != null)
            connectionPanel.SetActive(false);

        if (arenaSelectPanel != null)
            arenaSelectPanel.SetActive(false);
    }
}