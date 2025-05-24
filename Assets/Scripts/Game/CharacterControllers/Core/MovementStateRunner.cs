using Discovery.Game.Game.CharacterControllers.Core.Infos;
using Discovery.Game.Game.CharacterControllers.Core.Interfaces;

namespace Discovery.Game.Game.CharacterControllers.Core
{
    internal sealed class MovementStateRunner<TCharacter, TStatus> : IMovementStateRunner
        where TCharacter : ICharacter
        where TStatus : IMovementStatus
    {
        ICharacter IMovementStateRunner.Character => character;
        IMovementStatus IMovementStateRunner.Status => status;
        IMovementState IMovementStateRunner.State => state;

        public TStatus status;
        public TCharacter character;
        public MovementState<TCharacter, TStatus> state;

        public MovementStateRunner(
            TCharacter character,
            TStatus status,
            MovementState<TCharacter, TStatus> state)
        {
            this.character = character;
            this.status = status;
            this.state = state;
        }


        public int GetStatePriority() =>
            state.GetStatePriority(in character);
        public MovementInfos Execute(float deltaTime) =>
            state.GetStateVelocity(in deltaTime, in character, ref status);

        public void Enter() => state.Enter(in character, ref status);
        public void Exit() => state.Exit(in character, ref status);
        public void Dispose() => state.Release(in character, ref status);
    }
}