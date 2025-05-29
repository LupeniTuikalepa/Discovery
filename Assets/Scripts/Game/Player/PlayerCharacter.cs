using System;
using Discovery.Core;
using Discovery.Game.CharacterControllers.Humanoid;
using Discovery.Game.CharacterControllers.Humanoid.States;
using LTX.ChanneledProperties.Priorities;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Discovery.Game.Player
{
    [SelectionBase]
    public class PlayerCharacter : HumanoidCharacter
    {
        [Header("States")]
        [SerializeField]
        private IdleState idleState;
        [SerializeField]
        private JogState jogState;
        [SerializeField]
        private SprintState sprintState;
        [SerializeField]
        private FallingState fallingState;
        [SerializeField]
        private JumpingState jumpingState;


        protected virtual void OnEnable()
        {
            base.OnEnable();
            GameController.CursorLock.AddPriority(this, PriorityTags.Small, CursorLockMode.Locked);
            GameController.IsCursorVisible.AddPriority(this, PriorityTags.Small, false);
        }

        protected virtual void OnDisable()
        {
            base.OnDisable();
            GameController.CursorLock.RemovePriority(this);
            GameController.IsCursorVisible.RemovePriority(this);
        }

        private void Start()
        {
            RegisterMovementState(idleState);
            RegisterMovementState(jogState);
            RegisterMovementState(sprintState);
            RegisterMovementState(fallingState);
            RegisterMovementState(jumpingState);
        }

    }
}