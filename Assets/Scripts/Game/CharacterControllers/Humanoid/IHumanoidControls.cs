using UnityEngine;

namespace Discovery.Game.CharacterControllers.States
{
    public interface IHumanoidControls
    {
        Vector3 Direction { get; }
        bool WantsToSprint { get; }
        bool WantsToJump { get; }
    }
}