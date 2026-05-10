using Mirror;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// Minimal gameplay player for the Mirror room sample.
    /// Local input moves only the owned player; NetworkTransformUnreliable on
    /// the prefab synchronizes the transform to the server/other clients.
    /// </summary>
    public sealed class MirrorSampleGamePlayer : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float turnSpeed = 160f;

        [SyncVar(hook = nameof(OnDisplayNameChanged))]
        private string displayName = "Player";

        private Renderer cachedRenderer;
        private static readonly Color LocalColor = new Color(0.1f, 0.65f, 1f);
        private static readonly Color RemoteColor = new Color(1f, 0.72f, 0.16f);

        public override void OnStartClient()
        {
            base.OnStartClient();
            cachedRenderer = GetComponentInChildren<Renderer>();
            ApplyColor();
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            CmdSetDisplayName($"Player {netId}");
            ApplyColor();

            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCamera.transform.SetParent(transform, false);
                mainCamera.transform.localPosition = new Vector3(0f, 5f, -7f);
                mainCamera.transform.localRotation = Quaternion.Euler(35f, 0f, 0f);
            }
        }

        private void Update()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            Vector2 moveInput = ReadMoveInput();

            transform.Rotate(Vector3.up, moveInput.x * turnSpeed * Time.deltaTime);
            transform.position += transform.forward * (moveInput.y * moveSpeed * Time.deltaTime);
        }

        private void OnGUI()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            GUI.Label(new Rect(16, 16, 420, 28), "Mirror Sample: WASD/Arrow keys move the local player");
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

        [Command]
        private void CmdSetDisplayName(string requestedName)
        {
            displayName = string.IsNullOrWhiteSpace(requestedName) ? "Player" : requestedName;
        }

        private void OnDisplayNameChanged(string oldName, string newName)
        {
            gameObject.name = newName;
        }

        private void ApplyColor()
        {
            if (cachedRenderer == null)
            {
                cachedRenderer = GetComponentInChildren<Renderer>();
            }

            if (cachedRenderer != null)
            {
                cachedRenderer.material.color = isLocalPlayer ? LocalColor : RemoteColor;
            }
        }
    }
}
