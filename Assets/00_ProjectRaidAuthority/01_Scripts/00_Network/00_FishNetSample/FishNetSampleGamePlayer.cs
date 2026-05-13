using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// FishNet 룸 샘플의 게임플레이 플레이어입니다.
    /// 현재는 소유 클라이언트에서만 로컬 입력을 받고,
    /// FishNet NetworkTransform이 transform을 서버와 관찰자에게 동기화합니다.
    /// </summary>
    public sealed class FishNetSampleGamePlayer : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float turnSpeed = 160f;

        private readonly SyncVar<string> displayName = new("플레이어");

        private Renderer cachedRenderer;
        private static readonly Color LocalColor = new Color(0.1f, 0.65f, 1f);
        private static readonly Color RemoteColor = new Color(1f, 0.72f, 0.16f);

        private void Awake()
        {
            displayName.OnChange += OnDisplayNameChanged;
        }

        private void OnDestroy()
        {
            displayName.OnChange -= OnDisplayNameChanged;
        }

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

        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }

            Vector2 moveInput = ReadMoveInput();

            transform.Rotate(Vector3.up, moveInput.x * turnSpeed * Time.deltaTime);
            transform.position += transform.forward * (moveInput.y * moveSpeed * Time.deltaTime);
        }

        private void OnGUI()
        {
            if (!IsOwner)
            {
                return;
            }

            GUI.Label(new Rect(16, 16, 480, 28), "FishNet 샘플: WASD/화살표 키로 로컬 플레이어를 이동합니다");
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

        [ServerRpc]
        private void ServerSetDisplayName(string requestedName)
        {
            displayName.Value = string.IsNullOrWhiteSpace(requestedName) ? "플레이어" : requestedName;
        }

        private void OnDisplayNameChanged(string oldName, string newName, bool asServer)
        {
            gameObject.name = newName;
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
