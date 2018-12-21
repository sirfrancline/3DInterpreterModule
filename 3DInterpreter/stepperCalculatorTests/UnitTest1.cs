using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Movement;
using PrinterConfiguration;

namespace stepperCalculatorTests
{
    [TestClass]
    public class UnitTest1
    {
       

        [TestMethod]
        public void WhenCalculatingStepsNumberWithLowMaxSpeed()
        {
            // arange
            var stepsMM = 100;
            var distance = 20;
            var sut = new MovementCalculator(new Configuration
            {
                XMaxAcceleration = 2,
                XStepsPerMM = stepsMM,
                XMaxSpeedPerMM = 1
            });

            var expectedStepsCount = distance * stepsMM;


            // act
            var steps = sut.CalculateX(0, distance, 50);
            var actualStepsContu = steps.TailSteps.Count + steps.BodySteps.Count + steps.HeadSteps.Count;

            // assert
            Assert.AreEqual(expectedStepsCount, actualStepsContu);
        }




        [TestMethod]
        public void WhenCalculatingStepsNumberWithHighMaxSpeed()
        {
            // arange
            var stepsMM = 100;
            var distance = 20;
            var sut = new MovementCalculator(new Configuration
            {
                XMaxAcceleration = 2,
                XStepsPerMM = stepsMM,
                XMaxSpeedPerMM = 50
            });

            var expectedStepsCount = distance * stepsMM;


            // act
            var steps = sut.CalculateX(0, distance, 50);
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
            var sut = new MovementCalculator(new Configuration
            {
                XMaxAcceleration = 2,
                XStepsPerMM = stepsMM,
                XMaxSpeedPerMM = 50
            });

            var expectedStepsCount = distance * stepsMM;


            // act
            var steps = sut.CalculateX(0, distance, 50);
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
            var sut = new MovementCalculator(new Configuration
            {
                XMaxAcceleration = 2,
                XStepsPerMM = stepsMM,
                XMaxSpeedPerMM = 50
            });

            var expectedStepsCount = distance * stepsMM;


            // act
            var steps = sut.CalculateX(0, distance, 50);
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
            var sut = new MovementCalculator(new Configuration
            {
                XMaxAcceleration = 200,
                XStepsPerMM = stepsMM,
                XMaxSpeedPerMM = givenSpeed
            });

            var expectedStepsCount = distance * stepsMM;


            // act
            var steps = sut.CalculateX(0, distance, 50);
            var actualStepsContu = steps.TailSteps.Count + steps.BodySteps.Count + steps.HeadSteps.Count;

            // assert
            Assert.AreEqual(expectedStepsCount, actualStepsContu);
            Assert.IsTrue(steps.BodySteps.Count > 1);
            Assert.AreEqual(givenSpeed, steps.BodySteps[0].SpeedAfterMove);
        }



    }
}
