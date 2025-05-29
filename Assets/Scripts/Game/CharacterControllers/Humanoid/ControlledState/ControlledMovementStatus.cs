namespace Discovery.Game.CharacterControllers.Humanoid.States
{
    public struct ControlledMovementStatus : IControlledMovementStatus
    {
        public int PhaseFrames { get; set; }
        public int LastAnimatorState { get; set; }
        public ControlledMovementPhase CurrentPhase { get; set; }
    }
}