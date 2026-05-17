using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectRaidAuthority.Networking
{
    public sealed partial class GamePlayer
    {
        private const string PlayerActionMapName = "Player";
        private const string MoveActionName = "Move";
        private const string PointActionName = "Point";
        [Header("Owner Input")]
        [SerializeField] private InputActionReference moveActionReference;
        [SerializeField] private InputActionReference pointActionReference;
        [SerializeField] private InputActionAsset fallbackInputActions;

        private InputAction moveAction;
        private InputAction pointAction;
        private bool ownerEnabledMoveAction;
        private bool ownerEnabledPointAction;
        private Vector2 lastConfirmedLookDirection = Vector2.up;

        public override void OnStartClient()
        {
            base.OnStartClient();
            cachedRenderer = GetComponent<Renderer>();
            ApplyColor();

            if (IsOwner)
            {
                ServerSetDisplayName($"플레이어 {OwnerId}");
                InitializeOwnerClientState();
            }

            gameObject.name = displayName.Value;
        }

        public override void OnStopClient()
        {
            if (IsOwner)
            {
                CleanupOwnerClientState();
            }

            base.OnStopClient();
        }

        partial void OnSharedDestroyed()
        {
            CleanupOwnerClientState();
        }

        private void OnGUI()
        {
            if (!IsOwner)
            {
                return;
            }

            GUI.Label(new Rect(16, 16, 760, 28), "Network Flow: WASD/화살표는 카메라 기준 이동, 캐릭터 시선은 항상 마우스 위치를 따라갑니다");
        }

        private void InitializeOwnerClientState()
        {
            lastConfirmedLookDirection = CurrentForwardDirection();
            lastSentLookDirection = lastConfirmedLookDirection;
            lastSentMoveDirection = Vector2.zero;
            nextInputSendTime = 0f;

            InitializeOwnerInputActions();
            InitializeOwnerCamera();
        }

        private void CleanupOwnerClientState()
        {
            ReleaseOwnerInputActions();
            ReleaseOwnerCamera();
        }

        private void InitializeOwnerInputActions()
        {
            moveAction = ResolveOwnerAction(moveActionReference, MoveActionName);
            pointAction = ResolveOwnerAction(pointActionReference, PointActionName);
            EnableOwnerAction(moveAction, ref ownerEnabledMoveAction);
            EnableOwnerAction(pointAction, ref ownerEnabledPointAction);

            if (moveAction == null || pointAction == null)
            {
                Debug.LogWarning($"[NetworkFlow] {nameof(GamePlayer)} 입력 Action 참조가 부족합니다. " +
                                 $"{PlayerActionMapName}/{MoveActionName}, {PlayerActionMapName}/{PointActionName} 연결을 확인하세요.", this);
            }
        }

        private InputAction ResolveOwnerAction(InputActionReference actionReference, string actionName)
        {
            if (actionReference != null && actionReference.action != null)
            {
                return actionReference.action;
            }

            InputActionMap playerMap = fallbackInputActions != null
                ? fallbackInputActions.FindActionMap(PlayerActionMapName, false)
                : null;
            return playerMap?.FindAction(actionName, false);
        }

        private static void EnableOwnerAction(InputAction action, ref bool enabledByOwner)
        {
            enabledByOwner = false;
            if (action == null || action.enabled)
            {
                return;
            }

            action.Enable();
            enabledByOwner = true;
        }

        private void ReleaseOwnerInputActions()
        {
            DisableOwnerAction(moveAction, ref ownerEnabledMoveAction);
            DisableOwnerAction(pointAction, ref ownerEnabledPointAction);
            moveAction = null;
            pointAction = null;
        }

        private static void DisableOwnerAction(InputAction action, ref bool enabledByOwner)
        {
            if (action != null && enabledByOwner)
            {
                action.Disable();
            }

            enabledByOwner = false;
        }

        private Vector2 ReadMoveInput()
        {
            if (moveAction == null)
            {
                return Vector2.zero;
            }

            return Vector2.ClampMagnitude(moveAction.ReadValue<Vector2>(), 1f);
        }

        private void SendOwnedMovementInput()
        {
            if (Time.unscaledTime < nextInputSendTime)
            {
                return;
            }

            Vector2 localMoveInput = ReadMoveInput();
            Vector2 moveDirection = CalculateCameraRelativeMoveDirection(localMoveInput);
            bool lookChangedThisFrame = TryUpdateConfirmedLookDirectionFromPointer();

            if (!ShouldSendMovementInput(moveDirection, lookChangedThisFrame))
            {
                return;
            }

            float sendInterval = IsInstantApply(inputSendRate) ? 0f : 1f / Mathf.Max(1f, inputSendRate);
            nextInputSendTime = Time.unscaledTime + sendInterval;
            lastSentMoveDirection = moveDirection;
            lastSentLookDirection = lastConfirmedLookDirection;

            ServerSetMovementInput(moveDirection, lastConfirmedLookDirection);
        }

        private bool ShouldSendMovementInput(Vector2 moveDirection, bool lookChangedThisFrame)
        {
            float moveDelta = (moveDirection - lastSentMoveDirection).sqrMagnitude;
            float lookDelta = (lastConfirmedLookDirection - lastSentLookDirection).sqrMagnitude;
            return moveDelta > InputChangeEpsilon || (lookChangedThisFrame && lookDelta > InputChangeEpsilon);
        }

        private bool TryUpdateConfirmedLookDirectionFromPointer()
        {
            if (!TryReadPointerLookDirection(out Vector2 lookDirection))
            {
                return false;
            }

            if ((lookDirection - lastConfirmedLookDirection).sqrMagnitude <= InputChangeEpsilon)
            {
                return false;
            }

            lastConfirmedLookDirection = lookDirection;
            return true;
        }

        private bool TryReadPointerLookDirection(out Vector2 lookDirection)
        {
            lookDirection = lastConfirmedLookDirection;
            Camera targetCamera = GetOwnerCamera();
            if (targetCamera == null || pointAction == null)
            {
                return false;
            }

            Vector2 screenPosition = pointAction.ReadValue<Vector2>();
            Ray ray = targetCamera.ScreenPointToRay(screenPosition);
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            if (!groundPlane.Raycast(ray, out float enter))
            {
                return false;
            }

            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 lookVector = hitPoint - transform.position;
            lookVector.y = 0f;

            if (lookVector.sqrMagnitude <= DirectionEpsilon)
            {
                return false;
            }

            lookVector.Normalize();
            lookDirection = new Vector2(lookVector.x, lookVector.z);
            return true;
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

        private void ApplyColor()
        {
            if (cachedRenderer == null)
            {
                cachedRenderer = GetComponent<Renderer>();
            }

            if (cachedRenderer != null)
            {
                cachedRenderer.material.color = IsOwner ? LocalColor : RemoteColor;
            }
        }
    }
}
