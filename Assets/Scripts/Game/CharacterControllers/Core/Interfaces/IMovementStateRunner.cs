using Discovery.Game.CharacterControllers.Infos;

namespace Discovery.Game.CharacterControllers.Interfaces
{
    internal interface IMovementStateRunner
    {
        ICharacter Character { get; }
        IMovementStatus Status { get; }
        IMovementState State { get; }

        public int GetStatePriority();
        void Enter();
        MovementInfos Execute(float deltaTime);
        void Exit();
    }
}