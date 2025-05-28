using Discovery.Game.CharacterControllers.Infos;
using UnityEngine;

namespace Discovery.Game.CharacterControllers.Humanoid.States
{

    [CreateAssetMenu(menuName = "Character Controller/Falling")]
    public class FallingState : ControlledMovementState<ControlledMovementStatus>
    {
        public override int GetStatePriority(in HumanoidCharacter character) => character.IsGrounded ? -1 : 1;

        protected override Vector3 GetInputDirection(in HumanoidCharacter character, in ControlledMovementStatus status)
        {
            Vector3 baseDirection = base.GetInputDirection(in character, in status);
            return Vector3.ProjectOnPlane(baseDirection, character.Gravity.normalized);
        }

        public override MovementInfos GetStateVelocity(in float deltaTime, in HumanoidCharacter character, ref ControlledMovementStatus status)
        {
            MovementInfos movementInfos = base.GetStateVelocity(in deltaTime, in character, ref status);

            return movementInfos;
        }
    }
}