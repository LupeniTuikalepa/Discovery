namespace Discovery.Game.CharacterControllers.States
{
    public struct ControlledMovementStatus : IControlledMovementStatus
    {
        public int PhaseFrames { get; set; }
        public ControlledMovementPhase CurrentPhase { get; set; }
    }
}