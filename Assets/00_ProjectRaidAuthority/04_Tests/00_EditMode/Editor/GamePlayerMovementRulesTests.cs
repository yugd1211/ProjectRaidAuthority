using NUnit.Framework;
using ProjectRaidAuthority.Networking;

namespace ProjectRaidAuthority.Tests.EditMode
{
    public sealed class GamePlayerMovementRulesTests
    {
        [Test]
        public void SanitizeDirection_ReplacesInvalidInputWithFallback()
        {
            PlanarDirection fallback = new(0.25f, 0.75f);

            PlanarDirection sanitized = GamePlayerMovementRules.SanitizeDirection(new PlanarDirection(float.NaN, 1f), fallback);

            Assert.That(sanitized.X, Is.EqualTo(fallback.X));
            Assert.That(sanitized.Y, Is.EqualTo(fallback.Y));
        }

        [Test]
        public void SanitizeDirection_ClampsMagnitudeToOne()
        {
            PlanarDirection sanitized = GamePlayerMovementRules.SanitizeDirection(new PlanarDirection(3f, 4f), PlanarDirection.Zero);

            Assert.That(sanitized.SqrMagnitude, Is.LessThanOrEqualTo(1.0001f));
        }

        [Test]
        public void SanitizeServerInput_UsesPreviousLookWhenRequestedLookIsInvalid()
        {
            PlanarDirection previousLook = new(0f, 1f);

            ServerMovementInput input = GamePlayerMovementRules.SanitizeServerInput(
                new PlanarDirection(2f, 0f),
                new PlanarDirection(float.PositiveInfinity, 0f),
                previousLook,
                0.0001f);

            Assert.That(input.MoveDirection.SqrMagnitude, Is.LessThanOrEqualTo(1.0001f));
            Assert.That(input.LookDirection.X, Is.EqualTo(previousLook.X));
            Assert.That(input.LookDirection.Y, Is.EqualTo(previousLook.Y));
            Assert.That(input.HasLookDirection, Is.True);
        }
    }
}
