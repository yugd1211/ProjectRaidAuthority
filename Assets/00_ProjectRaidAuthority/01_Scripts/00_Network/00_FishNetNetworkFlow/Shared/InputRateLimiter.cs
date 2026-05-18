namespace ProjectRaidAuthority.Networking
{
    /// <summary>
    /// owner 입력 전송 빈도와 변경 threshold를 판단하는 순수 C# 상태 객체입니다.
    /// 현재 시각은 호출자가 주입하므로 엔진 프레임 시계에 의존하지 않습니다.
    /// </summary>
    public sealed class InputRateLimiter
    {
        private readonly float inputChangeEpsilon;
        private PlanarDirection lastSentMoveDirection;
        private PlanarDirection lastSentLookDirection = PlanarDirection.Up;
        private float nextInputSendTime;

        public InputRateLimiter(float inputChangeEpsilon)
        {
            this.inputChangeEpsilon = inputChangeEpsilon;
        }

        public void Reset(PlanarDirection initialLookDirection)
        {
            lastSentMoveDirection = PlanarDirection.Zero;
            lastSentLookDirection = initialLookDirection;
            nextInputSendTime = 0f;
        }

        public bool TryConsume(
            float currentTime,
            float inputSendRate,
            PlanarDirection moveDirection,
            PlanarDirection lookDirection,
            bool lookChangedThisFrame)
        {
            if (currentTime < nextInputSendTime)
            {
                return false;
            }

            if (!ShouldSend(moveDirection, lookDirection, lookChangedThisFrame))
            {
                return false;
            }

            float sendInterval = GamePlayerMovementRules.IsInstantApply(inputSendRate) ? 0f : 1f / Max(1f, inputSendRate);
            nextInputSendTime = currentTime + sendInterval;
            lastSentMoveDirection = moveDirection;
            lastSentLookDirection = lookDirection;
            return true;
        }

        private bool ShouldSend(PlanarDirection moveDirection, PlanarDirection lookDirection, bool lookChangedThisFrame)
        {
            float moveDelta = moveDirection.SquaredDistanceTo(lastSentMoveDirection);
            float lookDelta = lookDirection.SquaredDistanceTo(lastSentLookDirection);
            return moveDelta > inputChangeEpsilon || (lookChangedThisFrame && lookDelta > inputChangeEpsilon);
        }

        private static float Max(float a, float b)
        {
            return a > b ? a : b;
        }
    }
}
