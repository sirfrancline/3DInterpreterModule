using _3DInterpreter;
using _3DInterpreter.Configuration;

using System;
using System.Collections.Generic;

namespace Movement
{
    public class MovementCalculator
    {
        private readonly AxisConfiguration _axisConf;
        private int _stepsNumber;
        private double _traveledDistance;
        private double _timeBeforeStep;
        private bool _acceleratedToMaxSpeed;
        private bool _direction;
        private Movement _move = new Movement();
        private double _speed;
        private double _distance;

        public MovementCalculator(AxisConfiguration axisConf)
        {
            _axisConf = axisConf;
        }
        public Movement CalculateSteps(double startPosition, double stop, double maxSpeedMmPerSec)
        {
            // calculate distance
            _distance = stop - startPosition;
            _direction = _distance > 0; // true forward
            // given speed
            // max printer speed
            _speed = maxSpeedMmPerSec > _axisConf.MaxSpeedPerMM
                ? _axisConf.MaxSpeedPerMM
                : maxSpeedMmPerSec;

            // get the time for a full speed (acceleration)
            /*
             * t=(v-v0)/a --> t => speed / axis max accel
             */
            var timeToFullSpeed = _speed / _axisConf.MaxAcceleration;

            /*
             * s = at^2/2 -->
             *Speed after a given distance of travel with constant acceleration:/
             */


            /*
             * now we could calculate time after a given distance
             * distance = 1/2 * accel * time^2 --> time^2 = (distance * 2) / accel ==>
             * time = sqrt((2*distance)/accel)
             *
             * v^2 = 2*a(distance) --> v = sqrt(2*a(distance))
             */


            // as we know distance traveled to get full speed, then we can calculate step than we need to go
            ResetValues();

            var accelerating = true;
            while (accelerating)
            {
                accelerating = Accelerating(startPosition, _distance, _speed, _direction, _move);
            }

            // now we can do deceleration
            var decelerationSteps = DecelarationStepData(stop);

            CalculateBodySteps(startPosition, decelerationSteps[0].SpeedAfterMove);
            _move.TailSteps.AddRange(decelerationSteps);

            return _move;
        }
        private void ResetValues()
        {
            _move = new Movement();
            _stepsNumber = 0;
            _traveledDistance = 0;
            _timeBeforeStep = 0;
            _acceleratedToMaxSpeed = true;
        }
        private void CalculateBodySteps(double startPosition, double decelerationStepsSpeedAfterMove)
        {
            var distanceToMoveWithMaxSpeed = _distance - 2 * _traveledDistance;
            var bodyMovementSpeed = _acceleratedToMaxSpeed ? _speed : decelerationStepsSpeedAfterMove;
            var timeWithFullSpeed = distanceToMoveWithMaxSpeed / _speed;
            var stepsCountWithMaxSpeed = (int)(distanceToMoveWithMaxSpeed * _axisConf.StepsPerMM);
            var maxSpeedCycleTime = timeWithFullSpeed / stepsCountWithMaxSpeed;


            for (int i = 1; i <= stepsCountWithMaxSpeed; i++)
            {
                var stepData = new StepData
                {
                    DistanceAfterStep = _stepsNumber / _axisConf.StepsPerMM,
                    HeadPositionAfterStep =
                        (_direction)
                            ? _stepsNumber / _axisConf.StepsPerMM + startPosition
                            : startPosition - _stepsNumber / _axisConf.StepsPerMM,
                    StepTime = maxSpeedCycleTime,
                    SpeedAfterMove = bodyMovementSpeed,
                    StepNumber = _stepsNumber
                };

                _move.BodySteps.Add(stepData);
                _stepsNumber++;
            }
        }
        private List<StepData> DecelarationStepData(double stop)
        {
            var deceleration = new List<StepData>();
            var decelerationStep = 0;
            foreach (var step in _move.HeadSteps)
            {
                var decStep = new StepData
                {
                    DistanceAfterStep = _traveledDistance - decelerationStep / _axisConf.StepsPerMM,
                    StepTime = step.StepTime,
                    StepNumber = -step.StepNumber,
                    SpeedAfterMove = step.SpeedAfterMove,
                    HeadPositionAfterStep = (stop - decelerationStep / _axisConf.StepsPerMM),
                    TimeStamp = step.TimeStamp
                };

                deceleration.Insert(0, decStep);

                decelerationStep++;
            }

            return deceleration;
        }
        private bool Accelerating(double startPosition, double distance, double speed, bool direction, Movement move)
        {
            bool accelerating;
            _stepsNumber++;

            var distanceAfterStep = _stepsNumber / _axisConf.StepsPerMM;
            // check if we need to brake
            var fullDistanece = 2 * distanceAfterStep;
            if (fullDistanece > distance)
            {
                accelerating = false;
                _acceleratedToMaxSpeed = false;
                return accelerating;
            }

            var speedAfterDistance = Math.Sqrt(2 * _axisConf.MaxAcceleration * distanceAfterStep);
            accelerating = speed >= speedAfterDistance;

            if (!accelerating) return accelerating;

            var timeCaculatedFromStart = Math.Sqrt(2 * distanceAfterStep / _axisConf.MaxAcceleration);
            var cycleTime = timeCaculatedFromStart - _timeBeforeStep;
            var stepData = new StepData
            {
                DistanceAfterStep = distanceAfterStep,
                HeadPositionAfterStep =
                    (direction) ? distanceAfterStep + startPosition : startPosition - distanceAfterStep,
                StepTime = cycleTime,
                SpeedAfterMove = speedAfterDistance,
                StepNumber = _stepsNumber
            };

            move.HeadSteps.Add(stepData);
            //Console.WriteLine($"HeadPositionAfterStep: {stepData.HeadPositionAfterStep} SpeedAfterMove: {stepData.SpeedAfterMove} StepTime:{stepData.StepTime}");
            // step is finished
            _traveledDistance = distanceAfterStep;
            _timeBeforeStep = timeCaculatedFromStart;
            return accelerating;
        }
    }
}

 