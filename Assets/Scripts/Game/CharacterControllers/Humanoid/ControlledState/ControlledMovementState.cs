using System;
using Discovery.Game.CharacterControllers.Infos;
using UnityEngine;

namespace Discovery.Game.CharacterControllers.Humanoid.States
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

        [field: Header("Animation")]
        [field: SerializeField]
        public string DefaultStateName { get; private set; }
        [field: SerializeField]
        public string StopStateName { get; private set; }
        [field: SerializeField]
        public string HalfTurnStateName { get; private set; }

        [field: Header("Base")]
        [field: SerializeField, Min(0)]
        public float MaxSpeed { get; private set; } = 5;
        [field: SerializeField, Min(0)]
        public float RotationStrength { get; private set; } = 25f;

        [field: SerializeField, Min(0.000001f)]
        public float StopThreshold { get; private set; } = .001f;
        [field: SerializeField, Range(0, 1)]
        public float AlignSpeedThreshold { get; private set; } = .2f;
        [field: SerializeField]
        public MovementAxis Axis { get; private set; } = MovementAxis.Horizontal;

        [field: Header("Acceleration")]
        [field: SerializeField, Min(0)]
        public int AccelerationFrameCount { get; private set; } = 5;
        [field: SerializeField, Min(0)]
        public float AccelerationStrength { get; private set; } = 5;
        [field: SerializeField]
        public AnimationCurve AccelerationCurve { get; private set; }

        [field: Header("Deceleration")]
        [field: SerializeField, Min(0)]
        public int DecelerationFrameCount { get; private set; } = 5;
        [field: SerializeField, Min(0)]
        public float DecelerationStrength { get; private set; } = 5;

        [field: SerializeField]
        public AnimationCurve DecelerationCurve { get; private set; }

        [field: Header("Half turn")]
        [field: SerializeField]
        public bool EnableHalfTurn { get; private set; } = true;
        [field: SerializeField, Range(-1, 1)]
        public float HalfTurnThreshold { get; private set; } = 0;
        [field: SerializeField, Range(0, 1)]
        public float HalfTurnMinSpeed { get; private set; } = .35f;
        [field: SerializeField, Min(0)]
        public float HalfTurnStrength { get; private set; } = 20;

        [field: Header("Stop")]
        [field: SerializeField, Min(0)]
        public float StopStrength { get; private set; } = 20;

        [SerializeField, HideInInspector]
        private int stopStateNameHash;
        [SerializeField, HideInInspector]
        private int halfTurnStateNameHash;
        [SerializeField, HideInInspector]
        private int defaultStateNameHash;

        private void OnValidate()
        {
            defaultStateNameHash = string.IsNullOrWhiteSpace(DefaultStateName) ? 0 : Animator.StringToHash(DefaultStateName);
            stopStateNameHash = string.IsNullOrWhiteSpace(StopStateName) ? 0 : Animator.StringToHash(StopStateName);
            halfTurnStateNameHash = string.IsNullOrWhiteSpace(HalfTurnStateName) ? 0 : Animator.StringToHash(HalfTurnStateName);
        }

        public override T Initialize(in HumanoidCharacter character)
        {
            return new T()
            {
                PhaseFrames = 0,
                CurrentPhase = ControlledMovementPhase.Waiting,
                LastAnimatorState = 0,
            };
        }

        public override void Enter(in HumanoidCharacter character, ref T status)
        {
            status.LastAnimatorState = 0;
            status.PhaseFrames = 0;
            status.CurrentPhase = ControlledMovementPhase.Waiting;
        }

        public override void Exit(in HumanoidCharacter character, ref T status)
        {
            status.LastAnimatorState = 0;
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
                var halfTurnMinSpeed = HalfTurnMinSpeed * MaxSpeed;

                if (EnableHalfTurn && dot < HalfTurnThreshold &&
                    currentSqrSpeed > (status.CurrentPhase == ControlledMovementPhase.DoingHalfTurn ? .5f : halfTurnMinSpeed * halfTurnMinSpeed))
                    phase = ControlledMovementPhase.DoingHalfTurn;
                else if(Mathf.Abs(currentSqrSpeed -  MaxSpeed * MaxSpeed) <= .05f)
                    phase = ControlledMovementPhase.Moving;
                else if (currentSqrSpeed > MaxSpeed * MaxSpeed)
                    phase = ControlledMovementPhase.Decelerating;
                else
                    phase = ControlledMovementPhase.Accelerating;
            }

            Vector3 directionNormalized = GetInputDirection(in character, in status).normalized;
            Vector3 targetVelocity = directionNormalized * MaxSpeed;
            switch (phase)
            {
                case ControlledMovementPhase.Stopping:
                    newVelocity = Vector3.Slerp(affectedVelocity, Vector3.zero, StopStrength);
                    break;
                case ControlledMovementPhase.DoingHalfTurn:
                    newVelocity = Vector3.MoveTowards(affectedVelocity, -affectedVelocity * .2f, HalfTurnStrength);
                    break;
                case ControlledMovementPhase.Moving:
                    newVelocity = Vector3.RotateTowards(
                        affectedVelocity.normalized * MaxSpeed,
                        targetVelocity,
                        RotationStrength * Mathf.Deg2Rad * deltaTime,
                        0);
                    break;
                case ControlledMovementPhase.Accelerating or ControlledMovementPhase.Decelerating:
                    bool isAcceleration = phase == ControlledMovementPhase.Accelerating;
                    AnimationCurve curve = isAcceleration ? AccelerationCurve : DecelerationCurve;
                    float strength = isAcceleration ? AccelerationStrength : DecelerationStrength;
                    float frames = isAcceleration ? AccelerationFrameCount : DecelerationFrameCount;

                    float effectiveStrength = curve.Evaluate(status.PhaseFrames / frames) * strength;

                    if (currentSqrSpeed < AlignSpeedThreshold * MaxSpeed)
                        affectedVelocity = directionNormalized * Mathf.Sqrt(currentSqrSpeed);

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

            int hash = phase switch
            {
                ControlledMovementPhase.Stopping => stopStateNameHash,
                ControlledMovementPhase.DoingHalfTurn => halfTurnStateNameHash,
                ControlledMovementPhase.Accelerating or ControlledMovementPhase.Decelerating or ControlledMovementPhase.Moving => defaultStateNameHash,
                _ => 0
            };

            if (hash != 0 && status.LastAnimatorState != hash)
            {
                var state = character.Animator.GetAnimatorStateInfo();
                character.Animator.PlayState(hash, .15f, normalizedTimeOffset: state.normalizedTime);
            }
            status.LastAnimatorState = hash;

            Vector3 finalVelocity = Axis switch
            {
                MovementAxis.Horizontal => newVelocity + verticalVelocity,
                MovementAxis.Vertical => newVelocity + horizontalVelocity,
                MovementAxis.Vertical | MovementAxis.Horizontal => newVelocity,
                _ =>  Vector3.zero
            };

            return new MovementInfos()
            {
                velocity = finalVelocity,
                gravityMultiplier = GravityMultiplier(in character, in status),
            };
        }

        protected virtual Vector3 GetInputDirection(in HumanoidCharacter character, in T status)
        {
            switch (Axis)
            {
                case MovementAxis.Horizontal:
                    return character.GetHorizontalVector(character.InputDirection);
                case MovementAxis.Vertical:
                    return character.GetVerticalVector(character.InputDirection);

                case MovementAxis.Horizontal | MovementAxis.Vertical:
                    return character.InputDirection;
            }

            return character.InputDirection;
        }

        protected virtual float GravityMultiplier(in HumanoidCharacter character, in T status) => 1f;
    }
}