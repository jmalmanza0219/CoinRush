using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private TMP_InputField portInput;
    [SerializeField] private GameObject connectionPanel;

    private const ushort DefaultPort = 7777;

    public void StartHost()
    {
        ushort port = GetPort();
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        // Host listens on all local addresses.
        transport.SetConnectionData("127.0.0.1", port, "0.0.0.0");

        if (NetworkManager.Singleton.StartHost())
        {
            connectionPanel.SetActive(false);
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
            connectionPanel.SetActive(false);
            Debug.Log($"Client started. Connecting to {ip}:{port}");
        }
    }

    private ushort GetPort()
    {
        if (ushort.TryParse(portInput.text, out ushort parsedPort))
            return parsedPort;

        return DefaultPort;
    }
}