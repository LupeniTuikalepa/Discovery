using System;
using Discovery.Core;
using Discovery.Game.CharacterControllers.Humanoid;
using Discovery.Game.CharacterControllers.Humanoid.States;
using LTX.ChanneledProperties.Priorities;
using UnityEngine;

namespace Discovery.Game.Player
{
    [SelectionBase]
    public class PlayerCharacter : HumanoidCharacter
    {
        [SerializeField, Header("States")]
        private WalkState walkState;


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
        }

        private void Update()
        {
        }
    }
}