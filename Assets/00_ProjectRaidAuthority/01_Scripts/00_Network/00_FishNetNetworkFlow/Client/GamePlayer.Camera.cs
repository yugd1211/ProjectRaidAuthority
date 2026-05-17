using UnityEngine;

namespace ProjectRaidAuthority.Networking
{
    public sealed partial class GamePlayer
    {
        [Header("Owner Camera")]
        [SerializeField] private Vector3 ownerCameraOffset = new(0f, 12f, -10f);
        [SerializeField, Range(0f, 360f)] private float ownerCameraYaw = 45f;
        [SerializeField, Range(10f, 80f)] private float ownerCameraPitch = 58f;
        [SerializeField, Range(20f, 80f)] private float ownerCameraFieldOfView = 45f;
        [SerializeField, Range(-1f, 30f), Tooltip("-1이면 카메라 위치를 목표 위치로 즉시 적용합니다.")] private float ownerCameraFollowSharpness = 16f;
        [SerializeField, Range(-1f, 30f), Tooltip("-1이면 카메라 회전과 FOV를 목표값으로 즉시 적용합니다.")] private float ownerCameraRotationSharpness = 16f;

        private Camera ownerCamera;

        private void LateUpdate()
        {
            if (IsOwner)
            {
                FollowOwnerCamera(Time.deltaTime);
            }
        }

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

            bool instantPosition = IsInstantApply(ownerCameraFollowSharpness);
            bool instantRotation = IsInstantApply(ownerCameraRotationSharpness);
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

        private Vector2 CalculateCameraRelativeMoveDirection(Vector2 localMoveInput)
        {
            localMoveInput = Vector2.ClampMagnitude(localMoveInput, 1f);
            if (localMoveInput.sqrMagnitude <= InputChangeEpsilon)
            {
                return Vector2.zero;
            }

            Quaternion yawRotation = GetOwnerCameraYawRotation();
            Vector3 right = yawRotation * Vector3.right;
            Vector3 forward = yawRotation * Vector3.forward;
            Vector3 worldMoveDirection = right * localMoveInput.x + forward * localMoveInput.y;
            worldMoveDirection.y = 0f;

            if (worldMoveDirection.sqrMagnitude <= DirectionEpsilon)
            {
                return Vector2.zero;
            }

            worldMoveDirection.Normalize();
            return new Vector2(worldMoveDirection.x, worldMoveDirection.z);
        }

        private Quaternion GetOwnerCameraYawRotation()
        {
            return Quaternion.Euler(0f, ownerCameraYaw, 0f);
        }
    }
}
