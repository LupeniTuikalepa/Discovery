using System;
using Discovery.Game.CharacterControllers.Infos;
using UnityEngine;
using UnityEngine.Serialization;

namespace Discovery.Game.CharacterControllers.Humanoid.States
{

    [CreateAssetMenu(menuName = "Character Controller/Jumping")]
    public class JumpingState : ControlledMovementState<JumpingStatus>
    {
        [Header("Jumping")]
        [SerializeField]
        private string jumpState;
        [SerializeField]
        private string landState;

        [SerializeField, HideInInspector]
        private int jumpHash;
        [SerializeField, HideInInspector]
        private int landHash;

        private void OnValidate()
        {
            jumpHash = Animator.StringToHash(jumpState);
            landHash = Animator.StringToHash(landState);
        }


        public override void Enter(in HumanoidCharacter character, ref JumpingStatus status)
        {
            character.Animator.PlayState(jumpHash);
            character.AddForce(Vector3.up * 10);

            base.Enter(in character, ref status);
        }

        public override void Exit(in HumanoidCharacter character, ref JumpingStatus status)
        {
            if(character.IsGrounded)
                character.Animator.PlayState(landHash);

            base.Exit(in character, ref status);
        }

        public override int GetStatePriority(in HumanoidCharacter character)
        {
            if (character.TryGetCurrentState(out JumpingState state, out JumpingStatus status))
            {
                if (state != this)
                    return -1;

                float dot = Vector3.Dot(character.CurrentVelocity, character.Gravity);
                return dot > 0 ? 5 : 0;
            }

            return character.WantsToJump ? 5 : -1;
        }

        public override MovementInfos GetStateVelocity(in float deltaTime, in HumanoidCharacter character, ref JumpingStatus status)
        {
            MovementInfos stateVelocity = base.GetStateVelocity(in deltaTime, in character, ref status);

            stateVelocity.velocity += Vector3.up;
            return stateVelocity;
        }
    }
    public struct JumpingStatus : IControlledMovementStatus
    {
        public int PhaseFrames { get; set; }
        public int JumpFrames { get; set; }
        public ControlledMovementPhase CurrentPhase { get; set; }
    }
}