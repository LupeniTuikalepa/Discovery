using Discovery.Core;
using Discovery.Game.Game.CharacterControllers.Humanoid;
using Discovery.Game.Game.CharacterControllers.Humanoid.States;
using LTX.ChanneledProperties.Priorities;
using UnityEngine;

namespace Discovery.Game.Game.Player
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