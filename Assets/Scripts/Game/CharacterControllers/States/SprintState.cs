using UnityEngine;

namespace Discovery.Game.CharacterControllers.Humanoid.States
{
    [CreateAssetMenu(menuName = "Character Controller/Sprint")]
    public class SprintState : WalkState
    {
        public override int GetStatePriority(in HumanoidCharacter character) => (character.WantsToSprint ? 3 : 0) * base.GetStatePriority(character);
    }
}