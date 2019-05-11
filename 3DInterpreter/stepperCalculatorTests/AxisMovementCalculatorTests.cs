using _3DInterpreter;
using _3DInterpreter.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace stepperCalculatorTests
{
    [TestClass]
    public class AxisMovementCalculatorTests
    {
        [TestMethod]
        public void WhenCalculatingStepsNumberWithLowMaxSpeed()
        {
            var stepsMM = 100;
            var distance = 20;
            var sut = new MovementCalculator(new AxisConfiguration
            {
                MaxAcceleration = 2,
                StepsPerMM = stepsMM,
                MaxSpeedPerMM = 1
            },"X");
            var expectedStepsCount = distance * stepsMM;
            var steps = sut.CalculateSteps(0, distance, 50);
            var actualStepsContu = steps.TailSteps.Count + steps.BodySteps.Count + steps.HeadSteps.Count;
            Assert.AreEqual(expectedStepsCount, actualStepsContu);
        }
          [TestMethod]
        public void WhenCalculatingStepsNumberWithHighMaxSpeed()
        {
            var stepsMM = 100;
            var distance = 20;
            var sut = new MovementCalculator(new AxisConfiguration
            {
                MaxAcceleration = 2,
                StepsPerMM = stepsMM,
                MaxSpeedPerMM = 50
            },"Y");
            var expectedStepsCount = distance * stepsMM;
            var steps = sut.CalculateSteps(0, distance, 50);
            var actualStepsContu = steps.TailSteps.Count + steps.BodySteps.Count + steps.HeadSteps.Count;

            // assert
            Assert.AreEqual(expectedStepsCount, actualStepsContu);
            Assert.AreEqual(0, steps.BodySteps.Count);
        }

        [TestMethod]
        public void WhenCalculatingStepsNumberWithHighMaxSpeedAndHighResultion()
        {
            // arange
            var stepsMM = 2000;
            var distance = 20;
            var sut = new MovementCalculator(new AxisConfiguration
            {
                MaxAcceleration = 2,
                StepsPerMM = stepsMM,
                MaxSpeedPerMM = 50
            }, "X");

            var expectedStepsCount = distance * stepsMM;

            // act
            var steps = sut.CalculateSteps(0, distance, 50);
            var actualStepsContu = steps.TailSteps.Count + steps.BodySteps.Count + steps.HeadSteps.Count;

            // assert
            Assert.AreEqual(expectedStepsCount, actualStepsContu);
            Assert.AreEqual(0, steps.BodySteps.Count);
        }

        [TestMethod]
        public void WhenCalculatingStepsNumberWithHighMaxSpeedAndLowResultion()
        {
            // arange
            var stepsMM = 1;
            var distance = 23;
            var sut = new MovementCalculator(new AxisConfiguration
            {
                MaxAcceleration = 2,
                StepsPerMM = stepsMM,
                MaxSpeedPerMM = 50
            },"Y");

            var expectedStepsCount = distance * stepsMM;

            // act
            var steps = sut.CalculateSteps(0, distance, 50);
            var actualStepsContu = steps.TailSteps.Count + steps.BodySteps.Count + steps.HeadSteps.Count;

            // assert
            Assert.AreEqual(expectedStepsCount, actualStepsContu);
            Assert.AreEqual(distance % 2, steps.BodySteps.Count);
            Assert.AreNotSame(50, steps.BodySteps[0].SpeedAfterMove);
        }

        [TestMethod]
        public void WhenGettingFullSpeed()
        {
            // arange
            var stepsMM = 1;
            var distance = 1000;
            var givenSpeed = 50;
            var sut = new MovementCalculator(new AxisConfiguration
            {
                MaxAcceleration = 200,
                StepsPerMM = stepsMM,
                MaxSpeedPerMM = givenSpeed
            },"X");

            var expectedStepsCount = distance * stepsMM;

            // act
            var steps = sut.CalculateSteps(0, distance, 50);
            var actualStepsContu = steps.TailSteps.Count + steps.BodySteps.Count + steps.HeadSteps.Count;

            // assert
            Assert.AreEqual(expectedStepsCount, actualStepsContu);
            Assert.IsTrue(steps.BodySteps.Count > 1);
            Assert.AreEqual(givenSpeed, steps.BodySteps[0].SpeedAfterMove);
        }
    }
}