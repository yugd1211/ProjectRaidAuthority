using FishNet.Object;
using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    public sealed partial class GamePlayer
    {
        private void ApplyServerMovement(float deltaTime)
        {
            if (serverMoveDirection.sqrMagnitude > DirectionEpsilon)
            {
                Vector3 moveDelta = new Vector3(serverMoveDirection.x, 0f, serverMoveDirection.y) * (moveSpeed * deltaTime);
                transform.position += moveDelta;

                if (Time.unscaledTime >= nextServerMoveLogTime)
                {
                    nextServerMoveLogTime = Time.unscaledTime + serverMoveLogInterval;
                    Debug.Log($"[FishNet Authority Smoke] 서버 이동 적용: owner={OwnerId}, move={serverMoveDirection}, position={transform.position}");
                }
            }

            if (!hasServerLookDirection || serverLookDirection.sqrMagnitude <= DirectionEpsilon)
            {
                return;
            }

            Vector3 lookVector = new Vector3(serverLookDirection.x, 0f, serverLookDirection.y);
            Quaternion targetRotation = Quaternion.LookRotation(lookVector, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * deltaTime);
        }

        [ServerRpc]
        private void ServerSetMovementInput(Vector2 moveDirection, Vector2 lookDirection)
        {
            serverMoveDirection = SanitizeDirection(moveDirection, Vector2.zero);
            serverLookDirection = SanitizeDirection(lookDirection, serverLookDirection);
            hasServerLookDirection = serverLookDirection.sqrMagnitude > DirectionEpsilon;

            Debug.Log($"[FishNet Authority Smoke] 이동 입력 수신/검증: owner={OwnerId}, move={serverMoveDirection}, look={serverLookDirection}");
        }

        [ServerRpc]
        private void ServerSetDisplayName(string requestedName)
        {
            displayName.Value = string.IsNullOrWhiteSpace(requestedName) ? "플레이어" : requestedName;
        }
    }
}
