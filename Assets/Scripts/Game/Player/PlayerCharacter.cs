using System;
using LTX.ChanneledProperties.Priorities;
using Discovery.Core;
using Discovery.Game.CharacterControllers.States;
using UnityEngine;

namespace Discovery.Game.CharacterControllers
{
    public class PlayerCharacter : HumanoidCharacter
    {
        [SerializeField]
        private WalkState walkState;

        protected override void OnAwake()
        {
            base.OnAwake();
        }

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
    }
}