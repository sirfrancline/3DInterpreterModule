using System;
using System.Collections.Generic;

namespace _3DInterpreter.Commands
{
    public class MovementCalculator
    {
        private readonly AxisConfiguration _axisConf;
        private readonly string _axisName;
        private int _stepsNumber;
        private decimal _traveledDistance;
        private decimal _timeBeforeStep;

        /// <summary>
        /// a flag that indicates if true that the move is on full speed
        /// </summary>
        private bool _acceleratedToMaxSpeed;

        private Movement _move = new Movement();
        private decimal _speed;
        private decimal _distance;

        /// <summary>
        ///  in case of very short move that is only one step, we can not decelerate if true
        /// </summary>
        private bool _onlyOneStep;

        public MovementCalculator(AxisConfiguration axisConf, string axisName)
        {
            _axisConf = axisConf;
            _axisName = axisName;
        }

        public Movement CalculateSteps(decimal startPosition, decimal stop, decimal maxSpeedMmPerSec)
        {
            ResetValues();
            Console.WriteLine($"calculating for: startPosition:{startPosition}," +
            $" stop:{stop}, maxSpeedMmPerSec:{maxSpeedMmPerSec}");

            // calculate distance assumption 0,0 is in the corner, so we don't have negative not
            // covering delta printer https://www.youtube.com/watch?v=AYs6jASd_Ww future work we
            // could have a situation, printing area,, woork area
            _distance = stop - startPosition;
            Console.WriteLine(_distance);
            var tolerance = 1 / _axisConf.StepsPerMM;  // Why tolerance needded, this is added as we could loose precision when operate on double values
            if (Math.Abs(_distance) < tolerance )
            {
                Console.WriteLine("no move");
                return _move;
            }
            _move.Direction = _distance > 0; // true means forward
            // given speed
            // max printer speed
            _speed = maxSpeedMmPerSec > _axisConf.MaxSpeedPerMM
                ? _axisConf.MaxSpeedPerMM
                : maxSpeedMmPerSec;
            /*   get the time for a full speed (acceleration)
              * t=(v-v0)/a --> t => speed / axis max accel
              */
            var timeToFullSpeed = _speed / _axisConf.MaxAcceleration; // example target speed = 30m/s , acc = 20m/s^2, then, time = 30/20= 1.5 sec;
            // as we know distance traveled to get full speed, then we can calculate step than we need to go
            var accelerating = true;
            while (accelerating)
            {
                accelerating = Accelerating(startPosition, _distance, _speed, _move.Direction, _move);
            }
            // as we could have a edge case where only one step need to be added,
            // accelarating method need to give a flag to stop processing

            // now we can do deceleration
            if (!_onlyOneStep) // prevent creating deceleration steps when only one need to be added to the move
            {
                var decelerationSteps = DecelarationStepData(stop);
                CalculateBodySteps(startPosition, decelerationSteps[0].SpeedAfterMove);
                _move.TailSteps.AddRange(decelerationSteps);
            }
            return _move;
        }

        private void ResetValues()
        {
            _move = new Movement();
            _stepsNumber = 0;
            _traveledDistance = 0;
            _timeBeforeStep = 0;
            _acceleratedToMaxSpeed = true;

            _speed = 0;
            _distance = 0;
        }

        private void CalculateBodySteps(decimal startPosition, decimal decelerationStepsSpeedAfterMove)
        {
            var distanceToMoveWithMaxSpeed = _distance - 2 * _traveledDistance;

            if (distanceToMoveWithMaxSpeed <= 1 / _axisConf.StepsPerMM)
            {
                return;
            }

            var bodyMovementSpeed = _acceleratedToMaxSpeed ? _speed : decelerationStepsSpeedAfterMove;
            var timeWithFullSpeed = distanceToMoveWithMaxSpeed / _speed;
            var stepsCountWithMaxSpeed = (int)(distanceToMoveWithMaxSpeed * _axisConf.StepsPerMM);
            var maxSpeedCycleTime = timeWithFullSpeed / stepsCountWithMaxSpeed;

            for (int i = 1; i <= stepsCountWithMaxSpeed; i++)
            {
                var stepData = new StepData
                {
                    DistanceAfterStep = _stepsNumber / _axisConf.StepsPerMM,
                    PositionAfterStep =
                        (_move.Direction)
                            ? _stepsNumber / _axisConf.StepsPerMM + startPosition
                            : startPosition - _stepsNumber / _axisConf.StepsPerMM,
                    StepTime = maxSpeedCycleTime,
                    SpeedAfterMove = bodyMovementSpeed,
                    StepNumber = _stepsNumber,
                    AxisName = _axisName
                };

                _move.BodySteps.Add(stepData);
                _stepsNumber++;
            }

            _move.TotalTime += maxSpeedCycleTime * stepsCountWithMaxSpeed;
        }

        private List<StepData> DecelarationStepData(decimal stop)
        {
            // deceleration has same steps as acceleration but the reverse order
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
                    PositionAfterStep = (stop - decelerationStep / _axisConf.StepsPerMM),
                    TimeStamp = step.TimeStamp,
                    AxisName = _axisName
                };

                deceleration.Insert(0, decStep);
                _move.TotalTime += step.StepTime;
                decelerationStep++;
            }

            return deceleration;
        }

        private bool Accelerating(decimal startPosition, decimal distance, decimal speed, bool direction, Movement move)
        {
            bool accelerating; _stepsNumber++;
            var distanceAfterStep = _stepsNumber / _axisConf.StepsPerMM;
            // check if we need to brake
            /*              * now we could calculate time after a given distance
             * distance = 1/2 * accel * time^2 --> time^2 = (distance * 2) / accel ==>
             * time = sqrt((2*distance)/accel)            *
             * v^2 = 2*a(distance) --> v = sqrt(2*a(distance))             */
            var speedAfterDistance = (decimal)Math.Sqrt((double)(2 * _axisConf.MaxAcceleration * distanceAfterStep));
            accelerating = speed >= speedAfterDistance;
             if (!accelerating) return accelerating; // if we reach the max speed, then stop accelerating
            decimal timeCaculatedFromStart = (decimal)Math.Sqrt((double)(2 * distanceAfterStep / _axisConf.MaxAcceleration));
            var cycleTime = timeCaculatedFromStart - _timeBeforeStep;
            var stepData = new StepData
            {
                DistanceAfterStep = distanceAfterStep,
                PositionAfterStep =
                    (direction) ? distanceAfterStep + startPosition : startPosition - distanceAfterStep,
                StepTime = cycleTime,
                SpeedAfterMove = speedAfterDistance,
                StepNumber = _stepsNumber,
                AxisName = _axisName
            };
            // check if we need to brake incase of short movement when we are not going to full speed
            var fullDistance = 2 * distanceAfterStep;
            if (fullDistance > Math.Abs(distance))
            {
                accelerating = false;
                _acceleratedToMaxSpeed = false; //if true, then the move is on full speed
                // if we do first step then add it to the steps
                if (_stepsNumber > 1)
                {
                    return accelerating;
                }
                else
                {
                    _onlyOneStep = true;
                }
            }
            _move.TotalTime += stepData.StepTime;
            move.HeadSteps.Add(stepData);
            //Console.WriteLine($"HeadPositionAfterStep: {stepData.HeadPositionAfterStep} SpeedAfterMove: {stepData.SpeedAfterMove} StepTime:{stepData.StepTime}");
            // step is finished
            _traveledDistance = distanceAfterStep;
            _timeBeforeStep = timeCaculatedFromStart;
            return accelerating;
        }
    }
}