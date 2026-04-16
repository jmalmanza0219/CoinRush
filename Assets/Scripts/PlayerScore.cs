using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerScore : NetworkBehaviour
{
    public static event Action<ulong, int> OnScoreChanged;

    public NetworkVariable<int> Score = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        Score.OnValueChanged += HandleScoreChanged;
        HandleScoreChanged(Score.Value, Score.Value);
    }

    public override void OnNetworkDespawn()
    {
        Score.OnValueChanged -= HandleScoreChanged;
    }

    private void HandleScoreChanged(int oldValue, int newValue)
    {
        OnScoreChanged?.Invoke(OwnerClientId, newValue);
    }

    public void AddPoint()
    {
        if (!IsServer) return;
        Score.Value++;
    }
}