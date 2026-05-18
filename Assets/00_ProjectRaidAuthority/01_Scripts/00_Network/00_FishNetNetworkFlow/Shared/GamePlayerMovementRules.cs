using System;

namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// GamePlayer 서버 권한 이동 입력을 검증하는 순수 C# 규칙입니다.
    /// 엔진/네트워크 타입을 참조하지 않아 EditMode 단위 테스트로 고정할 수 있습니다.
    /// </summary>
    public static class GamePlayerMovementRules
    {
        private const float InstantApplyThreshold = 0f;

        public static bool IsInstantApply(float value)
        {
            return value < InstantApplyThreshold;
        }

        public static bool HasDirection(PlanarDirection direction, float epsilon)
        {
            return direction.SqrMagnitude > epsilon;
        }

        public static PlanarDirection SanitizeDirection(PlanarDirection direction, PlanarDirection fallback)
        {
            if (float.IsNaN(direction.X) || float.IsNaN(direction.Y) ||
                float.IsInfinity(direction.X) || float.IsInfinity(direction.Y))
            {
                return fallback;
            }

            return ClampMagnitude(direction, 1f);
        }

        public static ServerMovementInput SanitizeServerInput(
            PlanarDirection requestedMove,
            PlanarDirection requestedLook,
            PlanarDirection previousLook,
            float directionEpsilon)
        {
            PlanarDirection move = SanitizeDirection(requestedMove, PlanarDirection.Zero);
            PlanarDirection look = SanitizeDirection(requestedLook, previousLook);
            bool hasLook = HasDirection(look, directionEpsilon);
            return new ServerMovementInput(move, look, hasLook);
        }

        private static PlanarDirection ClampMagnitude(PlanarDirection direction, float maxMagnitude)
        {
            float sqrMagnitude = direction.SqrMagnitude;
            float maxSqrMagnitude = maxMagnitude * maxMagnitude;
            if (sqrMagnitude <= maxSqrMagnitude)
            {
                return direction;
            }

            float magnitude = (float)Math.Sqrt(sqrMagnitude);
            if (magnitude <= 0f)
            {
                return PlanarDirection.Zero;
            }

            float scale = maxMagnitude / magnitude;
            return new PlanarDirection(direction.X * scale, direction.Y * scale);
        }
    }

    public readonly struct ServerMovementInput
    {
        public ServerMovementInput(PlanarDirection moveDirection, PlanarDirection lookDirection, bool hasLookDirection)
        {
            MoveDirection = moveDirection;
            LookDirection = lookDirection;
            HasLookDirection = hasLookDirection;
        }

        public PlanarDirection MoveDirection { get; }
        public PlanarDirection LookDirection { get; }
        public bool HasLookDirection { get; }
    }

    public readonly struct PlanarDirection
    {
        public static readonly PlanarDirection Zero = new(0f, 0f);
        public static readonly PlanarDirection Up = new(0f, 1f);

        public PlanarDirection(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; }
        public float Y { get; }
        public float SqrMagnitude => X * X + Y * Y;

        public float SquaredDistanceTo(PlanarDirection other)
        {
            float deltaX = X - other.X;
            float deltaY = Y - other.Y;
            return deltaX * deltaX + deltaY * deltaY;
        }
    }
}
