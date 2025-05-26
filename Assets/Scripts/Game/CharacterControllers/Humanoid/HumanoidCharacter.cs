using UnityEngine;

namespace Discovery.Game.CharacterControllers.Humanoid
{
    public class HumanoidCharacter : Character<HumanoidCharacter>
    {
        public bool WantsToStop => controls.Direction.sqrMagnitude <= .05f;
        public Vector3 InputDirection => controls.Direction;

        private IHumanoidControls controls;

        protected override void OnAwake()
        {
            controls = GetComponent<IHumanoidControls>();
            base.OnAwake();
        }

        protected override HumanoidCharacter GetCharacter() => this;


        public Vector3 GetHorizontalVector(Vector3 vector3) =>  vector3.ProjectOnPlane(GroundNormal);
        public Vector3 GetVerticalVector(Vector3 vector3) =>  vector3.Project(GroundNormal);
    }
}