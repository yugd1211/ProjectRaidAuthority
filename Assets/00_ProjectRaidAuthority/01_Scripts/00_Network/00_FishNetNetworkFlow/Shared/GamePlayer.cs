using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// Gameplay 씬 GamePlayer의 공유 상태와 생명주기입니다.
    /// Client 폴더는 입력/카메라/표시, Server 폴더는 입력 검증과 transform 확정을 담당합니다.
    /// </summary>
    public sealed partial class GamePlayer : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField, Tooltip("-1이면 서버 회전을 목표 방향으로 즉시 적용합니다.")] private float turnSpeed = 160f;
        [SerializeField, Range(-1f, 60f), Tooltip("-1이면 입력 전송 rate limit을 끄고 매 프레임 변경을 즉시 전송합니다.")] private float inputSendRate = 30f;
        [SerializeField] private float serverMoveLogInterval = 0.5f;

        private readonly SyncVar<string> displayName = new("플레이어");

        private Renderer cachedRenderer;
        private Vector2 serverMoveDirection;
        private Vector2 serverLookDirection = Vector2.up;
        private bool hasServerLookDirection;
        private float nextInputSendTime;
        private float nextServerMoveLogTime;
        private float nextServerRotationLogTime;
        private Vector2 lastSentMoveDirection;
        private Vector2 lastSentLookDirection = Vector2.up;

        private const float InputChangeEpsilon = 0.0001f;
        private const float DirectionEpsilon = 0.0001f;
        private const float InstantApplyThreshold = 0f;
        private static readonly Color LocalColor = new Color(0.1f, 0.65f, 1f);
        private static readonly Color RemoteColor = new Color(1f, 0.72f, 0.16f);

        partial void OnSharedDestroyed();

        private void Awake()
        {
            displayName.OnChange += OnDisplayNameChanged;
        }

        private void OnDestroy()
        {
            displayName.OnChange -= OnDisplayNameChanged;
            OnSharedDestroyed();
        }

        private void Update()
        {
            if (IsServerStarted)
            {
                ApplyServerMovement(Time.deltaTime);
            }

            if (IsOwner)
            {
                SendOwnedMovementInput();
            }
        }

        private void OnDisplayNameChanged(string oldName, string newName, bool asServer)
        {
            gameObject.name = newName;
        }

        private static bool IsInstantApply(float value)
        {
            return value < InstantApplyThreshold;
        }

        private static Vector2 SanitizeDirection(Vector2 direction, Vector2 fallback)
        {
            if (float.IsNaN(direction.x) || float.IsNaN(direction.y) ||
                float.IsInfinity(direction.x) || float.IsInfinity(direction.y))
            {
                return fallback;
            }

            return Vector2.ClampMagnitude(direction, 1f);
        }
    }
}
