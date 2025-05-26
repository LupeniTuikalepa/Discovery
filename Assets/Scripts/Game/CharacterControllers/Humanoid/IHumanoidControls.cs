using UnityEngine;

namespace Discovery.Game.CharacterControllers.Humanoid
{
    public interface IHumanoidControls
    {
        Vector3 Direction { get; }
        bool WantsToSprint { get; }
        bool WantsToJump { get; }
    }
}