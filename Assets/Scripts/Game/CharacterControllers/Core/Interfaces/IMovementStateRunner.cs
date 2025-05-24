using Discovery.Game.Game.CharacterControllers.Core.Infos;

namespace Discovery.Game.Game.CharacterControllers.Core.Interfaces
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