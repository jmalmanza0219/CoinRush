using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchManager : NetworkBehaviour
{
    public static MatchManager Instance { get; private set; }

    [Header("Match Settings")]
    [SerializeField] private float matchLength = 60f;
    [SerializeField] private bool autoLoadResultsScene = false;
    [SerializeField] private float resultsDelay = 3f;
    [SerializeField] private string resultsSceneName = "Results";

    public NetworkVariable<float> TimeRemaining = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<bool> MatchEnded = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<ulong> WinnerClientId = new NetworkVariable<ulong>(
        ulong.MaxValue,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<int> CoinsRemaining = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private readonly List<PlayerScore> players = new List<PlayerScore>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        TimeRemaining.Value = matchLength;
        MatchEnded.Value = false;
        WinnerClientId.Value = ulong.MaxValue;
        CoinsRemaining.Value = FindObjectsOfType<CoinPickup>().Length;

        StartCoroutine(RefreshPlayersAfterSceneLoad());
    }

    private IEnumerator RefreshPlayersAfterSceneLoad()
    {
        yield return null;
        yield return null;

        RefreshPlayerList();
        Debug.Log("Players registered in MatchManager: " + players.Count);
    }

    private void RefreshPlayerList()
    {
        players.Clear();

        if (NetworkManager.Singleton == null) return;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject == null) continue;

            PlayerScore score = client.PlayerObject.GetComponent<PlayerScore>();
            if (score != null)
            {
                players.Add(score);
            }
        }
    }

    private void Update()
    {
        if (!IsServer || !IsSpawned) return;
        if (MatchEnded.Value) return;

        TimeRemaining.Value -= Time.deltaTime;

        if (TimeRemaining.Value <= 0f)
        {
            TimeRemaining.Value = 0f;
            EndMatch();
        }
    }

    public void OnCoinCollected()
    {
        if (!IsServer) return;
        if (MatchEnded.Value) return;

        CoinsRemaining.Value--;

        if (CoinsRemaining.Value <= 0)
        {
            CoinsRemaining.Value = 0;
            EndMatch();
        }
    }

    private void EndMatch()
    {
        if (MatchEnded.Value) return;

        RefreshPlayerList();
        MatchEnded.Value = true;

        if (players.Count == 0)
        {
            WinnerClientId.Value = ulong.MaxValue;
            Debug.Log("No players found. Result = Draw");
        }
        else if (players.Count == 1)
        {
            WinnerClientId.Value = players[0].OwnerClientId;
            Debug.Log("Only one player found. Winner = " + WinnerClientId.Value);
        }
        else
        {
            PlayerScore playerA = players[0];
            PlayerScore playerB = players[1];

            Debug.Log($"P1 Score: {playerA.Score.Value} | P2 Score: {playerB.Score.Value}");

            if (playerA.Score.Value > playerB.Score.Value)
            {
                WinnerClientId.Value = playerA.OwnerClientId;
            }
            else if (playerB.Score.Value > playerA.Score.Value)
            {
                WinnerClientId.Value = playerB.OwnerClientId;
            }
            else
            {
                WinnerClientId.Value = ulong.MaxValue; // draw
            }
        }

        Debug.Log("WinnerClientId = " + WinnerClientId.Value);

        if (autoLoadResultsScene)
        {
            StartCoroutine(LoadResultsAfterDelay());
        }
    }

    private IEnumerator LoadResultsAfterDelay()
    {
        yield return new WaitForSeconds(resultsDelay);

        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(resultsSceneName, LoadSceneMode.Single);
        }
    }
}