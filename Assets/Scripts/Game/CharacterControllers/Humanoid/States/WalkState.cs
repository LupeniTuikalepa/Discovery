using System.ComponentModel;
using UnityEngine;

namespace Discovery.Game.CharacterControllers.States
{
    [CreateAssetMenu(menuName = "Character Controller/Walk")]
    public class WalkState : ControlledMovementState<ControlledMovementStatus>
    {
        public override int GetStatePriority(in HumanoidCharacter character) => 1;

        protected override Vector3 GetInputDirection(in HumanoidCharacter character, in ControlledMovementStatus status)
        {
            Vector3 baseDirection = base.GetInputDirection(in character, in status);
            return Vector3.ProjectOnPlane(baseDirection, character.GroundNormal);
        }
    }
}