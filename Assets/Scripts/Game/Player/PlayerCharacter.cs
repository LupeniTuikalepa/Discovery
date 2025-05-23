using System;
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

        private void Start()
        {
            RegisterMovementState(walkState);
        }
    }
}