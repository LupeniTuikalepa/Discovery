using Discovery.Game.CharacterControllers.Infos;
using LTX.ChanneledProperties.Priorities;
using UnityEngine;

namespace Discovery.Game.CharacterControllers.Humanoid.States
{

    [CreateAssetMenu(menuName = "Character Controller/Falling")]
    public class FallingState : ControlledMovementState<ControlledMovementStatus>
    {
        [SerializeField]
        private float capsuleRadius = .5f;
        [SerializeField]
        private float capsuleHeight = 1;

        public override int GetStatePriority(in HumanoidCharacter character) => character.IsGrounded ? -1 : 1;


        public override void Enter(in HumanoidCharacter character, ref ControlledMovementStatus status)
        {
            base.Enter(in character, ref status);
            character.Body.CapsuleSize.AddPriority(this, PriorityTags.Default, new Vector2()
            {
                x = capsuleRadius,
                y = capsuleHeight
            });
        }

        public override void Exit(in HumanoidCharacter character, ref ControlledMovementStatus status)
        {
            base.Exit(in character, ref status);
            character.Body.CapsuleSize.RemovePriority(this);
        }

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