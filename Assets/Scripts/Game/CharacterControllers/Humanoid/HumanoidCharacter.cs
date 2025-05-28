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
            Quaternion bodyRotation = Body.Rotation;

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

            Quaternion rot = Quaternion.LookRotation(forward, upwards);

            Debug.DrawRay(transform.position + Vector3.up, upwards);
            Debug.DrawRay(transform.position + Vector3.up, forward);
            Quaternion smoothed = Quaternion.RotateTowards(bodyRotation, rot, OrientationSpeed * Time.deltaTime);
            Body.Rotate(Quaternion.Inverse(bodyRotation) * smoothed);
        }

        protected override HumanoidCharacter GetCharacter() => this;


        public Vector3 GetHorizontalVector(Vector3 vector3) =>  vector3.ProjectOnPlane(GroundNormal);
        public Vector3 GetVerticalVector(Vector3 vector3) =>  vector3.Project(GroundNormal);
    }
}