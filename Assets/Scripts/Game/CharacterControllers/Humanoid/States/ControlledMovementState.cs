using System;
using Discovery.Game.Game.CharacterControllers.Core;
using Discovery.Game.Game.CharacterControllers.Core.Infos;
using UnityEngine;

namespace Discovery.Game.Game.CharacterControllers.Humanoid.States
{
    public abstract class ControlledMovementState<T> : MovementState<HumanoidCharacter, T>
        where T : struct, IControlledMovementStatus
    {

        [Flags]
        public enum MovementAxis
        {
            Horizontal = 1,
            Vertical = 2,
        }

        [field: Header("Base")]
        [field: SerializeField, Min(0)]
        public float MaxSpeed { get; private set; } = 5;
        [field: SerializeField, Min(0)]
        public float RotationStrength { get; private set; } = 25f;

        [field: SerializeField, Min(0.000001f)]
        public float StopThreshold { get; private set; } = .001f;
        [field: SerializeField]
        public MovementAxis Axis { get; private set; } = MovementAxis.Horizontal;

        [field: Header("Acceleration")]
        [field: SerializeField, Min(0)]
        public int AccelerationFrameCount { get; private set; } = 5;
        [field: SerializeField, Min(0)]
        public int AccelerationStrength { get; private set; } = 5;
        [field: SerializeField]
        public AnimationCurve AccelerationCurve { get; private set; }

        [field: Header("Deceleration")]
        [field: SerializeField, Min(0)]
        public int DecelerationFrameCount { get; private set; } = 5;
        [field: SerializeField, Min(0)]
        public int DecelerationStrength { get; private set; } = 5;

        [field: SerializeField]
        public AnimationCurve DecelerationCurve { get; private set; }

        [field: Header("Half turn")]
        [field: SerializeField]
        public bool EnableHalfTurn { get; private set; } = true;
        [field: SerializeField, Range(-1, 1)]
        public float HalfTurnThreshold { get; private set; } = 0;
        [field: SerializeField, Min(0)]
        public float HalfTurnStrength { get; private set; } = 20;

        [field: Header("Stop")]
        [field: SerializeField, Min(0)]
        public float StopStrength { get; private set; } = 20;

        public override T Initialize(in HumanoidCharacter character)
        {
            return new T()
            {
                PhaseFrames = 0,
                CurrentPhase = ControlledMovementPhase.Waiting,
            };
        }

        public override void Enter(in HumanoidCharacter character, ref T status)
        {
            status.PhaseFrames = 0;
            status.CurrentPhase = ControlledMovementPhase.Waiting;
        }

        public override void Exit(in HumanoidCharacter character, ref T status)
        {
            status.PhaseFrames = 0;
            status.CurrentPhase = ControlledMovementPhase.Waiting;
        }

        public override MovementInfos GetStateVelocity(in float deltaTime, in HumanoidCharacter character, ref T status)
        {
            Vector3 currentVelocity = character.CurrentVelocity;

            Vector3 horizontalVelocity = character.GetHorizontalVector(currentVelocity);
            Vector3 verticalVelocity = currentVelocity - horizontalVelocity;

            Vector3 affectedVelocity =  Axis switch
            {
                MovementAxis.Horizontal => horizontalVelocity,
                MovementAxis.Vertical => verticalVelocity,
                MovementAxis.Vertical | MovementAxis.Horizontal => currentVelocity,
                _ => Vector3.zero
            };

            Vector3 newVelocity = Vector3.zero;
            float currentSqrSpeed = affectedVelocity.sqrMagnitude;

            ControlledMovementPhase phase = ControlledMovementPhase.Waiting;

            if (character.WantsToStop || character.InputDirection.sqrMagnitude <= 0.1f)
            {
                var isAlreadyStopped = currentSqrSpeed < StopThreshold * StopThreshold;
                phase = isAlreadyStopped ? ControlledMovementPhase.Waiting : ControlledMovementPhase.Stopping;
            }
            else
            {
                float dot = Vector3.Dot(character.InputDirection.normalized, affectedVelocity.normalized);

                if (EnableHalfTurn && dot < HalfTurnThreshold && currentSqrSpeed > .05f)
                    phase = ControlledMovementPhase.DoingHalfTurn;
                else if (currentSqrSpeed > MaxSpeed * MaxSpeed)
                    phase = ControlledMovementPhase.Decelerating;
                else
                    phase = ControlledMovementPhase.Accelerating;
            }


            switch (phase)
            {
                case ControlledMovementPhase.Stopping:
                    newVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, StopStrength);
                    break;
                case ControlledMovementPhase.DoingHalfTurn:
                    newVelocity = Vector3.Lerp(affectedVelocity, Vector3.zero, HalfTurnStrength);
                    break;
                case ControlledMovementPhase.Accelerating or ControlledMovementPhase.Decelerating:
                    bool isAcceleration = phase == ControlledMovementPhase.Accelerating;
                    AnimationCurve curve = isAcceleration ? AccelerationCurve : DecelerationCurve;
                    float strength = isAcceleration ? AccelerationStrength : DecelerationStrength;
                    float frames = isAcceleration ? AccelerationFrameCount : DecelerationFrameCount;

                    float effectiveStrength = curve.Evaluate(status.PhaseFrames / frames) * strength;
                    Vector3 targetVelocity =  GetInputDirection(in character, in status).normalized * MaxSpeed;
                    newVelocity = Vector3.RotateTowards(affectedVelocity,
                        targetVelocity,
                        RotationStrength * Mathf.Deg2Rad * deltaTime,
                        effectiveStrength * deltaTime);
                    break;
            }

            //State frame increment
            if (status.CurrentPhase == phase)
                status.PhaseFrames++;
            else
            {
                status.CurrentPhase = phase;
                status.PhaseFrames = 0;
            }
            return new MovementInfos()
            {
                velocity = Axis switch
                {
                    MovementAxis.Horizontal => newVelocity + verticalVelocity,
                    MovementAxis.Vertical => newVelocity + horizontalVelocity,
                    MovementAxis.Vertical | MovementAxis.Horizontal => newVelocity,
                    _ =>  Vector3.zero
                },

                snapToGround = CanSnapToGround(in character, in status),
                gravityMultiplier = GravityMultiplier(in character, in status),
            };
        }

        protected virtual Vector3 GetInputDirection(in HumanoidCharacter character, in T status) =>
            character.InputDirection;

        protected virtual float GravityMultiplier(in HumanoidCharacter character, in T status) => 1f;
        protected virtual bool CanSnapToGround(in HumanoidCharacter character, in T status) => false;
    }
}