using Axiom.Math;
using Core;
using NUnit.Framework;
using System;

namespace Tests
{
    [TestFixture]
    public class QuaternionTest
    {
        private const int CYCLES = 1;

        [Test]
        public void TestSlerpSmall(
            [Range(-180, 180, 90)] int angle,
            [Range(-CYCLES, CYCLES)] int startCycle,
            [Range(-CYCLES, CYCLES)] int endCycle)
        {
            AssertSlerpQuarterAnglesEqual(10 + angle, 30 + angle, 0 + angle, 40 + angle, startCycle, endCycle);
        }

        [Test]
        public void TestSlerpBig(
            [Range(-180, 180, 90)] int angle,
            [Range(-CYCLES, CYCLES)] int startCycle,
            [Range(-CYCLES, CYCLES)] int endCycle)
        {
            AssertSlerpQuarterAnglesEqual(-40 + angle, 240 + angle, 0 + angle, 200 + angle, startCycle, endCycle);
        }

        [Test]
        public void TestAnglesEqual([Values(-360, -359, -1, 0, 1, 359, 360)] float almostZero)
        {
            Assert.That(AnglesEqual(almostZero, 0, 2));
        }

        [Test]
        public void TestAnglesUnequal([Values(-355, -180, -5, 5, 180, 355)] float farFromZero)
        {
            Assert.That(!AnglesEqual(farFromZero, 0, 2));
        }

        private void AssertSlerpQuarterAnglesEqual(float expectedFirstQuarter, float expectedSecondQuarter,
            float start, float end, int startCycle, int endCycle)
        {
            var startDegrees = start + 360 * startCycle;
            var endDegrees = end + 360 * endCycle;
            AssertDegreesEqual(expectedFirstQuarter, GetSlerpQuarterAngle(startDegrees, endDegrees));
            AssertDegreesEqual(expectedSecondQuarter, GetSlerpQuarterAngle(endDegrees, startDegrees));
        }

        private void AssertDegreesEqual(float expected, float actual)
        {
            Assert.That(AnglesEqual(expected, actual, 1e-4f),
                "Expected {0} degrees but was {1} degrees", expected, actual);
        }

        private bool AnglesEqual(float a, float b, float delta)
        {
            return Math.Abs((System.Math.Abs(a - b) + 180) % 360 - 180) < delta;
        }

        private float GetSlerpQuarterAngle(float startDegrees, float endDegrees)
        {
            var from = Quaternion.FromAngleAxis(Utility.DegreesToRadians(startDegrees), Vector3.UnitY);
            var to = Quaternion.FromAngleAxis(Utility.DegreesToRadians(endDegrees), Vector3.UnitY);
            return Util.SlerpShortest(0.25f, from, to).YawInDegrees;
            // Axiom.Math.Quaternion.Slerp would fail some tests:
            // return Quaternion.Slerp(0.25f, from, to, true).YawInDegrees;
        }
    }
}
