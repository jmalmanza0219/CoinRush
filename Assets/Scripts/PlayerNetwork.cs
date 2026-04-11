using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 moveInput;

    private void Update()
    {
        if (!IsOwner || !IsSpawned) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(moveX, moveY).normalized;
    }

    private void FixedUpdate()
    {
        if (!IsOwner || !IsSpawned) return;

        SubmitMovementServerRpc(moveInput.x, moveInput.y, Time.fixedDeltaTime);
    }

    [ServerRpc]
    private void SubmitMovementServerRpc(float x, float y, float deltaTime)
    {
        Vector3 movement = new Vector3(x, y, 0f) * moveSpeed * deltaTime;
        transform.position += movement;
    }
}
