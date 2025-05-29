using Discovery.Game.CharacterControllers.Interfaces;

namespace Discovery.Game.CharacterControllers.Humanoid.States
{
    public interface IControlledMovementStatus : IMovementStatus
    {
        int PhaseFrames { get; set; }
        int LastAnimatorState { get; set; }
        ControlledMovementPhase CurrentPhase { get; set; }
    }
}