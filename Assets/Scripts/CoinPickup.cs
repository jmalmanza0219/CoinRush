using Unity.Netcode;
using UnityEngine;

public class CoinPickup : NetworkBehaviour
{
    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer || collected) return;

        if (!other.TryGetComponent<PlayerScore>(out PlayerScore playerScore)) return;

        collected = true;
        playerScore.AddPoint();

        if (MatchManager.Instance != null)
        {
            MatchManager.Instance.OnCoinCollected();
        }

        NetworkObject.Despawn();
    }
}
