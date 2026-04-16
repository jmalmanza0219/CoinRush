using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 localInput;
    private Vector2 serverInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!IsOwner || !IsSpawned) return;
        if (MatchManager.Instance != null && MatchManager.Instance.MatchEnded.Value) return;

        localInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        // Host is both owner and server, so it can write directly.
        if (IsServer)
        {
            serverInput = localInput;
        }
    }

    private void FixedUpdate()
    {
        if (!IsSpawned) return;

        if (MatchManager.Instance != null && MatchManager.Instance.MatchEnded.Value)
        {
            if (IsServer) rb.velocity = Vector2.zero;
            return;
        }

        // Client owner sends its input to the server.
        if (IsOwner && !IsServer)
        {
            SubmitMovementServerRpc(localInput.x, localInput.y);
        }

        // Server performs the actual movement.
        if (IsServer)
        {
            rb.velocity = serverInput * moveSpeed;
        }
    }

    [ServerRpc]
    private void SubmitMovementServerRpc(float x, float y)
    {
        serverInput = new Vector2(x, y);
    }
}
