using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ProjectRaidAuthority.Networking
{
    public sealed partial class GamePlayer
    {
        public override void OnStartClient()
        {
            base.OnStartClient();
            cachedRenderer = GetComponentInChildren<Renderer>();
            ApplyColor();

            if (IsOwner)
            {
                ServerSetDisplayName($"플레이어 {OwnerId}");
                AttachCamera();
            }

            gameObject.name = displayName.Value;
        }

        private void OnGUI()
        {
            if (!IsOwner)
            {
                return;
            }

            GUI.Label(new Rect(16, 16, 640, 28), "Network Flow: 마우스로 방향을 잡고 WASD/화살표 키 입력을 서버 권한 이동으로 보냅니다");
        }

        private static Vector2 ReadMoveInput()
        {
            Vector2 input = Vector2.zero;

#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                {
                    input.x -= 1f;
                }

                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                {
                    input.x += 1f;
                }

                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                {
                    input.y -= 1f;
                }

                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                {
                    input.y += 1f;
                }
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            if (input == Vector2.zero)
            {
                input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            }
#endif

            return Vector2.ClampMagnitude(input, 1f);
        }

        private void SendOwnedMovementInput()
        {
            if (Time.unscaledTime < nextInputSendTime)
            {
                return;
            }

            Vector2 localMoveInput = ReadMoveInput();
            Vector2 lookDirection = ReadLookDirection();
            Vector2 moveDirection = CalculateWorldMoveDirection(localMoveInput, lookDirection);

            if (!ShouldSendMovementInput(moveDirection, lookDirection))
            {
                return;
            }

            float sendInterval = 1f / Mathf.Max(1f, inputSendRate);
            nextInputSendTime = Time.unscaledTime + sendInterval;
            lastSentMoveDirection = moveDirection;
            lastSentLookDirection = lookDirection;

            ServerSetMovementInput(moveDirection, lookDirection);
        }

        private bool ShouldSendMovementInput(Vector2 moveDirection, Vector2 lookDirection)
        {
            float moveDelta = (moveDirection - lastSentMoveDirection).sqrMagnitude;
            float lookDelta = (lookDirection - lastSentLookDirection).sqrMagnitude;
            return moveDelta > InputChangeEpsilon || lookDelta > InputChangeEpsilon;
        }

        private Vector2 ReadLookDirection()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                return CurrentForwardDirection();
            }

#if ENABLE_INPUT_SYSTEM
            Mouse mouse = Mouse.current;
            if (mouse != null)
            {
                return ScreenPointToLookDirection(mainCamera, mouse.position.ReadValue());
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            return ScreenPointToLookDirection(mainCamera, Input.mousePosition);
#else
            return CurrentForwardDirection();
#endif
        }

        private Vector2 ScreenPointToLookDirection(Camera mainCamera, Vector2 screenPosition)
        {
            Ray ray = mainCamera.ScreenPointToRay(screenPosition);
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            if (!groundPlane.Raycast(ray, out float enter))
            {
                return CurrentForwardDirection();
            }

            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 lookVector = hitPoint - transform.position;
            lookVector.y = 0f;

            if (lookVector.sqrMagnitude <= DirectionEpsilon)
            {
                return CurrentForwardDirection();
            }

            lookVector.Normalize();
            return new Vector2(lookVector.x, lookVector.z);
        }

        private Vector2 CurrentForwardDirection()
        {
            Vector3 forward = transform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude <= DirectionEpsilon)
            {
                return Vector2.up;
            }

            forward.Normalize();
            return new Vector2(forward.x, forward.z);
        }

        private static Vector2 CalculateWorldMoveDirection(Vector2 localMoveInput, Vector2 lookDirection)
        {
            localMoveInput = Vector2.ClampMagnitude(localMoveInput, 1f);
            lookDirection = SanitizeDirection(lookDirection, Vector2.up);

            Vector2 rightDirection = new Vector2(lookDirection.y, -lookDirection.x);
            Vector2 worldMoveDirection = rightDirection * localMoveInput.x + lookDirection * localMoveInput.y;
            return Vector2.ClampMagnitude(worldMoveDirection, 1f);
        }

        private void AttachCamera()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                return;
            }

            mainCamera.transform.SetParent(transform, false);
            mainCamera.transform.localPosition = new Vector3(0f, 5f, -7f);
            mainCamera.transform.localRotation = Quaternion.Euler(35f, 0f, 0f);
        }

        private void ApplyColor()
        {
            if (cachedRenderer == null)
            {
                cachedRenderer = GetComponentInChildren<Renderer>();
            }

            if (cachedRenderer != null)
            {
                cachedRenderer.material.color = IsOwner ? LocalColor : RemoteColor;
            }
        }
    }
}
