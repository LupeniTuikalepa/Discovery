using Discovery.Game.Game.CharacterControllers.Core.Interfaces;

namespace Discovery.Game.Game.CharacterControllers.Humanoid.States
{
    public interface IControlledMovementStatus : IMovementStatus
    {
        int PhaseFrames { get; set; }
        ControlledMovementPhase CurrentPhase { get; set; }
    }
}