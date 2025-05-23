namespace Discovery.Game.CharacterControllers.States
{
    public interface IControlledMovementStatus : IMovementStatus
    {
        int PhaseFrames { get; set; }
        ControlledMovementPhase CurrentPhase { get; set; }
    }
}