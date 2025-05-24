using Discovery.Game.Game.CharacterControllers.Core;
using UnityEngine;

namespace Discovery.Game.Game.CharacterControllers.Humanoid
{
    public class HumanoidCharacter : Character<HumanoidCharacter>
    {
        public Vector3 GravityDirection { get; private set; } = Vector3.down;
        public float GravityStrength { get; private set; } = 9.81f;

        public bool IsGrounded { get; private set; } = true;
        public Vector3 GroundNormal { get; private set; } = Vector3.up;
        public Vector3 GroundPosition { get; private set; }

        public bool WantsToStop => controls.Direction.sqrMagnitude <= .05f;
        public Vector3 InputDirection => controls.Direction;

        private IHumanoidControls controls;

        protected override void OnAwake()
        {
            controls = GetComponent<IHumanoidControls>();
            base.OnAwake();
        }

        protected override HumanoidCharacter GetCharacter() => this;


        public Vector3 GetHorizontalVector(Vector3 vector3) =>  vector3.ProjectOnPlane(GravityDirection);
        public Vector3 GetVerticalVector(Vector3 vector3) =>  vector3.Project(GravityDirection);
    }
}