using System.ComponentModel;
using UnityEngine;

namespace Discovery.Game.CharacterControllers.States
{
    [CreateAssetMenu(menuName = "Character Controller/Walk")]
    public class WalkState : ControlledMovementState<ControlledMovementStatus>
    {
        public override int GetStatePriority(in HumanoidCharacter character) => 1;
    }
}