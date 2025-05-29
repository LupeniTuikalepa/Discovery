using System;
using Discovery.Game.CharacterControllers.Infos;
using Discovery.Game.CharacterControllers.Interfaces;
using UnityEngine;

namespace Discovery.Game.CharacterControllers.Humanoid.States
{
    public struct IdleStatus : IMovementStatus
    {

    }
    [CreateAssetMenu(fileName = "New Idle State", menuName = "Character Controller/Idle")]
    public class IdleState : MovementState<HumanoidCharacter, IdleStatus>
    {
        [SerializeField]
        private string animatorState;

        [SerializeField]
        private float stopForce = .3f;

        [SerializeField, HideInInspector]
        private int animatorStateHash;

        private void OnValidate()
        {
            animatorStateHash = Animator.StringToHash(animatorState);
        }

        public override int GetStatePriority(in HumanoidCharacter character) => character.IsGrounded ? 1 : -1;

        public override void Enter(in HumanoidCharacter character, ref IdleStatus status)
        {
            character.Animator.PlayState(animatorStateHash, .2f);
        }

        public override void Exit(in HumanoidCharacter character, ref IdleStatus status)
        {
        }

        public override MovementInfos GetStateVelocity(in float deltaTime, in HumanoidCharacter character, ref IdleStatus status)
        {
            return new MovementInfos()
            {
                velocity = Vector3.Lerp(character.StateVelocity, Vector3.zero, stopForce * deltaTime),
                gravityMultiplier = 1f
            };
        }

        public override IdleStatus Initialize(in HumanoidCharacter character)
        {
            return new IdleStatus();
        }
    }
}