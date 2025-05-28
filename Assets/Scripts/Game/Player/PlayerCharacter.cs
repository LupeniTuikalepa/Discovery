using System;
using Discovery.Core;
using Discovery.Game.CharacterControllers.Humanoid;
using Discovery.Game.CharacterControllers.Humanoid.States;
using LTX.ChanneledProperties.Priorities;
using UnityEngine;
using UnityEngine.Rendering;

namespace Discovery.Game.Player
{
    [SelectionBase]
    public class PlayerCharacter : HumanoidCharacter
    {
        [SerializeField, Header("States")]
        private WalkState walkState;
        [SerializeField]
        private SprintState sprintState;
        [SerializeField]
        private FallingState fallingState;
        [SerializeField]
        private JumpingState jumpingState;


        private void OnEnable()
        {
            GameController.CursorLock.AddPriority(this, PriorityTags.Small, CursorLockMode.Locked);
            GameController.IsCursorVisible.AddPriority(this, PriorityTags.Small, false);
        }

        private void OnDisable()
        {
            GameController.CursorLock.RemovePriority(this);
            GameController.IsCursorVisible.RemovePriority(this);
        }

        private void Start()
        {
            RegisterMovementState(walkState);
            RegisterMovementState(sprintState);
            RegisterMovementState(fallingState);
            RegisterMovementState(jumpingState);
        }

    }
}