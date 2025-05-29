using System;
using UnityEngine;

namespace Discovery.Game.CharacterControllers.Humanoid
{
    public class HumanoidCharacter : Character<HumanoidCharacter>
    {
        public enum OrientationType
        {
            None,
            StateVelocity,
            CurrentVelocity,
            Input,
        }
        public bool WantsToStop => controls.Direction.sqrMagnitude <= .05f;
        public Vector3 InputDirection => controls.Direction;
        public bool WantsToSprint => controls.WantsToSprint;
        public bool WantsToJump => controls.WantsToJump && IsGrounded;


        private IHumanoidControls controls;

        [field: SerializeField, Header("Components")]
        public HumanoidAnimator Animator { get; private set; }
        [field: SerializeField, Header("Orientation")]
        public OrientationType Orientation { get; private set; }
        [field: SerializeField]
        public float OrientationSpeed { get; private set; }


        protected override void OnAwake()
        {
            controls = GetComponent<IHumanoidControls>();
            base.OnAwake();
        }

        protected override void HandleRotation()
        {
            Vector3 upwards = -Gravity.normalized;
            Quaternion bodyRotation = Body.Rotation.normalized;

            Vector3 direction = Orientation switch
            {
                OrientationType.StateVelocity => StateVelocity,
                OrientationType.CurrentVelocity => CurrentVelocity,
                OrientationType.Input => InputDirection,
                _ => transform.forward
            };

            Vector3 forward = Vector3.ProjectOnPlane(direction, upwards).normalized;
            if (forward.sqrMagnitude <= 0.01f)
                return;

            if(forward == upwards)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(forward, upwards).normalized;
            Quaternion smoothed = Quaternion.RotateTowards(bodyRotation, targetRotation, OrientationSpeed * Time.deltaTime);

            Body.SetRotation(smoothed);
        }

        protected override HumanoidCharacter GetCharacter() => this;


        public Vector3 GetHorizontalVector(Vector3 vector3) =>  vector3.ProjectOnPlane(GroundNormal);
        public Vector3 GetVerticalVector(Vector3 vector3) =>  vector3.Project(GroundNormal);
    }
}