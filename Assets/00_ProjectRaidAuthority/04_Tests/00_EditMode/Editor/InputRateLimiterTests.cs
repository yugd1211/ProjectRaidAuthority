using NUnit.Framework;
using ProjectRaidAuthority.Networking;

namespace ProjectRaidAuthority.Tests.EditMode
{
    public sealed class InputRateLimiterTests
    {
        [Test]
        public void TryConsume_SendsChangedMoveAndBlocksUntilNextInterval()
        {
            InputRateLimiter limiter = new(0.0001f);
            limiter.Reset(PlanarDirection.Up);

            bool first = limiter.TryConsume(10f, 20f, new PlanarDirection(1f, 0f), PlanarDirection.Up, false);
            bool blocked = limiter.TryConsume(10.01f, 20f, new PlanarDirection(0f, 1f), PlanarDirection.Up, false);
            bool afterInterval = limiter.TryConsume(10.06f, 20f, new PlanarDirection(0f, 1f), PlanarDirection.Up, false);

            Assert.That(first, Is.True);
            Assert.That(blocked, Is.False);
            Assert.That(afterInterval, Is.True);
        }

        [Test]
        public void TryConsume_DoesNotSendWhenChangeIsBelowThreshold()
        {
            InputRateLimiter limiter = new(0.01f);
            limiter.Reset(PlanarDirection.Up);

            bool sent = limiter.TryConsume(0f, 30f, new PlanarDirection(0.01f, 0f), PlanarDirection.Up, false);

            Assert.That(sent, Is.False);
        }

        [Test]
        public void TryConsume_InstantRateDoesNotBlockNextChangedInput()
        {
            InputRateLimiter limiter = new(0.0001f);
            limiter.Reset(PlanarDirection.Up);

            bool first = limiter.TryConsume(1f, -1f, new PlanarDirection(1f, 0f), PlanarDirection.Up, false);
            bool second = limiter.TryConsume(1f, -1f, new PlanarDirection(0f, 1f), PlanarDirection.Up, false);

            Assert.That(first, Is.True);
            Assert.That(second, Is.True);
        }
    }
}
