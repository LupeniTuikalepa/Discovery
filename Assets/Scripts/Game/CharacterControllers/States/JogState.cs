using UnityEngine;

namespace Discovery.Game.CharacterControllers.Humanoid.States
{
    [CreateAssetMenu(menuName = "Character Controller/Jog")]
    public class JogState : ControlledMovementState<ControlledMovementStatus>
    {
        public override int GetStatePriority(in HumanoidCharacter character) => character.IsGrounded && character.InputDirection.sqrMagnitude > .1f ? 2 : -1;

        protected override Vector3 GetInputDirection(in HumanoidCharacter character, in ControlledMovementStatus status)
        {
            Vector3 baseDirection = base.GetInputDirection(in character, in status);
            return Vector3.ProjectOnPlane(baseDirection, character.GroundNormal);
        }

    }
}