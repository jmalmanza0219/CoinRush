using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ArenaSpawnManager : NetworkBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        StartCoroutine(PositionPlayersWhenReady());
    }

    private IEnumerator PositionPlayersWhenReady()
    {
        float timeout = 5f;

        while (timeout > 0f)
        {
            if (AllPlayersReady())
                break;

            timeout -= Time.deltaTime;
            yield return null;
        }

        PositionPlayers();
    }

    private bool AllPlayersReady()
    {
        if (NetworkManager.Singleton == null) return false;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject == null)
                return false;
        }

        return NetworkManager.Singleton.ConnectedClientsList.Count > 0;
    }

    private void PositionPlayers()
    {
        List<NetworkClient> clients = new List<NetworkClient>(NetworkManager.Singleton.ConnectedClientsList);
        clients.Sort((a, b) => a.ClientId.CompareTo(b.ClientId));

        for (int i = 0; i < clients.Count && i < spawnPoints.Length; i++)
        {
            NetworkObject playerObject = clients[i].PlayerObject;
            if (playerObject == null) continue;

            Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.position = spawnPoints[i].position;
                rb.velocity = Vector2.zero;
            }
            else
            {
                playerObject.transform.position = spawnPoints[i].position;
            }
        }
    }
}