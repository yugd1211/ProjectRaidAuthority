using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// Gameplay 씬 GamePlayer의 FishNet/Unity adapter입니다.
    /// 네트워크 계약은 이 파일에 모으고, 순수 계산/정책은 collaborator로 위임합니다.
    /// </summary>
    public sealed partial class GamePlayer : NetworkBehaviour
    {
        #region Serialized refs/settings

        [Header("Server Movement")]
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField, Tooltip("-1이면 서버 회전을 목표 방향으로 즉시 적용합니다.")] private float turnSpeed = 160f;
        [SerializeField] private float serverMoveLogInterval = 0.5f;

        [Header("Owner Input")]
        [SerializeField, Range(-1f, 60f), Tooltip("-1이면 입력 전송 rate limit을 끄고 매 프레임 변경을 즉시 전송합니다.")] private float inputSendRate = 30f;
        [SerializeField] private InputActionReference moveActionReference;
        [SerializeField] private InputActionReference pointActionReference;
        [SerializeField] private InputActionAsset fallbackInputActions;

        [Header("Owner Camera")]
        [SerializeField] private Vector3 ownerCameraOffset = new(0f, 12f, -10f);
        [SerializeField, Range(0f, 360f)] private float ownerCameraYaw = 45f;
        [SerializeField, Range(10f, 80f)] private float ownerCameraPitch = 58f;
        [SerializeField, Range(20f, 80f)] private float ownerCameraFieldOfView = 45f;
        [SerializeField, Range(-1f, 30f), Tooltip("-1이면 카메라 위치를 목표 위치로 즉시 적용합니다.")] private float ownerCameraFollowSharpness = 16f;
        [SerializeField, Range(-1f, 30f), Tooltip("-1이면 카메라 회전과 FOV를 목표값으로 즉시 적용합니다.")] private float ownerCameraRotationSharpness = 16f;

        #endregion

        #region SyncVar/state

        private const string PlayerActionMapName = "Player";
        private const string MoveActionName = "Move";
        private const string PointActionName = "Point";
        private const float InputChangeEpsilon = 0.0001f;
        private const float DirectionEpsilon = 0.0001f;

        private static readonly Color LocalColor = new(0.1f, 0.65f, 1f);
        private static readonly Color RemoteColor = new(1f, 0.72f, 0.16f);

        private readonly SyncVar<string> displayName = new("플레이어");
        private readonly InputRateLimiter inputRateLimiter = new(InputChangeEpsilon);

        private Renderer cachedRenderer;
        private InputAction moveAction;
        private InputAction pointAction;
        private Camera ownerCamera;
        private PlanarDirection serverMoveDirection;
        private PlanarDirection serverLookDirection = PlanarDirection.Up;
        private PlanarDirection lastConfirmedLookDirection = PlanarDirection.Up;
        private bool hasServerLookDirection;
        private bool ownerEnabledMoveAction;
        private bool ownerEnabledPointAction;
        private float nextServerMoveLogTime;
        private float nextServerRotationLogTime;

        #endregion

        #region Unity/FishNet lifecycle

        private void Awake()
        {
            displayName.OnChange += OnDisplayNameChanged;
        }

        private void OnDestroy()
        {
            displayName.OnChange -= OnDisplayNameChanged;
            CleanupOwnerClientState();
        }

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

        private void Update()
        {
            if (IsServerStarted)
            {
                ApplyServerMovement(Time.deltaTime);
            }

            if (IsOwner)
            {
                SendOwnedMovementInput(Time.unscaledTime);
                SendOwnedLootInput();
            }
        }

        private void LateUpdate()
        {
            if (IsOwner)
            {
                FollowOwnerCamera(Time.deltaTime);
            }
        }

        #endregion

        #region Owner input adapter

        private void OnGUI()
        {
            if (!IsOwner)
            {
                return;
            }

            GUI.Label(new Rect(16, 16, 760, 28), "Network Flow: WASD/화살표는 카메라 기준 이동, 캐릭터 시선은 항상 마우스 위치를 따라갑니다");
            GUI.Label(new Rect(16, 44, 760, 28), "Loot Smoke: 가까운 LootItem 앞에서 E 키를 누르면 서버 권한 획득 요청을 보냅니다");
        }

        private void InitializeOwnerClientState()
        {
            lastConfirmedLookDirection = CurrentForwardDirection();
            inputRateLimiter.Reset(lastConfirmedLookDirection);
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

        private void SendOwnedMovementInput(float currentTime)
        {
            Vector2 localMoveInput = ReadMoveInput();
            PlanarDirection moveDirection = CalculateCameraRelativeMoveDirection(localMoveInput);
            bool lookChangedThisFrame = TryUpdateConfirmedLookDirectionFromPointer();

            if (!inputRateLimiter.TryConsume(currentTime, inputSendRate, moveDirection, lastConfirmedLookDirection, lookChangedThisFrame))
            {
                return;
            }

            ServerSetMovementInput(ToVector2(moveDirection), ToVector2(lastConfirmedLookDirection));
        }

        #endregion

        #region Server RPC/apply

        [ServerRpc]
        private void ServerSetMovementInput(Vector2 moveDirection, Vector2 lookDirection)
        {
            ServerMovementInput sanitizedInput = GamePlayerMovementRules.SanitizeServerInput(
                ToPlanarDirection(moveDirection),
                ToPlanarDirection(lookDirection),
                serverLookDirection,
                DirectionEpsilon);

            serverMoveDirection = sanitizedInput.MoveDirection;
            serverLookDirection = sanitizedInput.LookDirection;
            hasServerLookDirection = sanitizedInput.HasLookDirection;

            // Debug.Log($"[FishNet Authority Smoke] 이동 입력 수신/검증: owner={OwnerId}, move={ToVector2(serverMoveDirection)}, look={ToVector2(serverLookDirection)}");
        }

        [ServerRpc]
        private void ServerSetDisplayName(string requestedName)
        {
            displayName.Value = string.IsNullOrWhiteSpace(requestedName) ? "플레이어" : requestedName;
        }

        private void ApplyServerMovement(float deltaTime)
        {
            if (GamePlayerMovementRules.HasDirection(serverMoveDirection, DirectionEpsilon))
            {
                Vector3 moveDelta = new(serverMoveDirection.X, 0f, serverMoveDirection.Y);
                transform.position += moveDelta * (moveSpeed * deltaTime);

                if (Time.unscaledTime >= nextServerMoveLogTime)
                {
                    nextServerMoveLogTime = Time.unscaledTime + serverMoveLogInterval;
                    Debug.Log($"[FishNet Authority Smoke] 서버 이동 적용: owner={OwnerId}, move={ToVector2(serverMoveDirection)}, position={transform.position}");
                }
            }

            if (!hasServerLookDirection || !GamePlayerMovementRules.HasDirection(serverLookDirection, DirectionEpsilon))
            {
                return;
            }

            Vector3 lookVector = new(serverLookDirection.X, 0f, serverLookDirection.Y);
            Quaternion targetRotation = Quaternion.LookRotation(lookVector, Vector3.up);
            transform.rotation = GamePlayerMovementRules.IsInstantApply(turnSpeed)
                ? targetRotation
                : Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * deltaTime);

            if (Time.unscaledTime >= nextServerRotationLogTime)
            {
                nextServerRotationLogTime = Time.unscaledTime + serverMoveLogInterval;
                float yawDelta = Quaternion.Angle(transform.rotation, targetRotation);
                // Debug.Log($"[FishNet Authority Smoke] 서버 회전 적용: owner={OwnerId}, currentYaw={transform.eulerAngles.y:F1}, targetYaw={targetRotation.eulerAngles.y:F1}, yawDelta={yawDelta:F2}, look={ToVector2(serverLookDirection)}");
            }
        }

        #endregion

        #region Camera adapter

        private void InitializeOwnerCamera()
        {
            ownerCamera = Camera.main;
            if (ownerCamera == null)
            {
                Debug.LogWarning("[NetworkFlow] owner Camera.main을 찾지 못해 카메라 follow를 건너뜁니다.", this);
                return;
            }

            ownerCamera.transform.SetParent(null, true);
            ownerCamera.fieldOfView = ownerCameraFieldOfView;
            FollowOwnerCamera(0f);
        }

        private void ReleaseOwnerCamera()
        {
            ownerCamera = null;
        }

        private Camera GetOwnerCamera()
        {
            if (ownerCamera == null)
            {
                ownerCamera = Camera.main;
            }

            return ownerCamera;
        }

        private void FollowOwnerCamera(float deltaTime)
        {
            Camera targetCamera = GetOwnerCamera();
            if (targetCamera == null)
            {
                return;
            }

            Quaternion yawRotation = GetOwnerCameraYawRotation();
            Quaternion targetRotation = Quaternion.Euler(ownerCameraPitch, ownerCameraYaw, 0f);
            Vector3 targetPosition = transform.position + yawRotation * ownerCameraOffset;

            if (deltaTime <= 0f)
            {
                targetCamera.transform.SetPositionAndRotation(targetPosition, targetRotation);
                targetCamera.fieldOfView = ownerCameraFieldOfView;
                return;
            }

            bool instantPosition = GamePlayerMovementRules.IsInstantApply(ownerCameraFollowSharpness);
            bool instantRotation = GamePlayerMovementRules.IsInstantApply(ownerCameraRotationSharpness);
            float positionT = instantPosition ? 1f : 1f - Mathf.Exp(-ownerCameraFollowSharpness * deltaTime);
            float rotationT = instantRotation ? 1f : 1f - Mathf.Exp(-ownerCameraRotationSharpness * deltaTime);
            targetCamera.transform.position = instantPosition
                ? targetPosition
                : Vector3.Lerp(targetCamera.transform.position, targetPosition, positionT);
            targetCamera.transform.rotation = instantRotation
                ? targetRotation
                : Quaternion.Slerp(targetCamera.transform.rotation, targetRotation, rotationT);
            targetCamera.fieldOfView = instantRotation
                ? ownerCameraFieldOfView
                : Mathf.Lerp(targetCamera.fieldOfView, ownerCameraFieldOfView, rotationT);
        }

        private PlanarDirection CalculateCameraRelativeMoveDirection(Vector2 localMoveInput)
        {
            localMoveInput = Vector2.ClampMagnitude(localMoveInput, 1f);
            if (localMoveInput.sqrMagnitude <= InputChangeEpsilon)
            {
                return PlanarDirection.Zero;
            }

            Quaternion yawRotation = GetOwnerCameraYawRotation();
            Vector3 right = yawRotation * Vector3.right;
            Vector3 forward = yawRotation * Vector3.forward;
            Vector3 worldMoveDirection = right * localMoveInput.x + forward * localMoveInput.y;
            worldMoveDirection.y = 0f;

            if (worldMoveDirection.sqrMagnitude <= DirectionEpsilon)
            {
                return PlanarDirection.Zero;
            }

            worldMoveDirection.Normalize();
            return new PlanarDirection(worldMoveDirection.x, worldMoveDirection.z);
        }

        private Quaternion GetOwnerCameraYawRotation()
        {
            return Quaternion.Euler(0f, ownerCameraYaw, 0f);
        }

        #endregion

        #region Private helpers

        private bool TryUpdateConfirmedLookDirectionFromPointer()
        {
            if (!TryReadPointerLookDirection(out PlanarDirection lookDirection))
            {
                return false;
            }

            if (lookDirection.SquaredDistanceTo(lastConfirmedLookDirection) <= InputChangeEpsilon)
            {
                return false;
            }

            lastConfirmedLookDirection = lookDirection;
            return true;
        }

        private bool TryReadPointerLookDirection(out PlanarDirection lookDirection)
        {
            lookDirection = lastConfirmedLookDirection;
            Camera targetCamera = GetOwnerCamera();
            if (targetCamera == null || pointAction == null)
            {
                return false;
            }

            Vector2 screenPosition = pointAction.ReadValue<Vector2>();
            Ray ray = targetCamera.ScreenPointToRay(screenPosition);
            Plane groundPlane = new(Vector3.up, transform.position);
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
            lookDirection = new PlanarDirection(lookVector.x, lookVector.z);
            return true;
        }

        private PlanarDirection CurrentForwardDirection()
        {
            Vector3 forward = transform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude <= DirectionEpsilon)
            {
                return PlanarDirection.Up;
            }

            forward.Normalize();
            return new PlanarDirection(forward.x, forward.z);
        }

        private void OnDisplayNameChanged(string oldName, string newName, bool asServer)
        {
            gameObject.name = newName;
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

        private static PlanarDirection ToPlanarDirection(Vector2 direction)
        {
            return new PlanarDirection(direction.x, direction.y);
        }

        private static Vector2 ToVector2(PlanarDirection direction)
        {
            return new Vector2(direction.X, direction.Y);
        }

        #endregion
    }
}
