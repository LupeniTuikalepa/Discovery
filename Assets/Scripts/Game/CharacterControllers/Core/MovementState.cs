using Discovery.Game.Game.CharacterControllers.Core.Infos;
using Discovery.Game.Game.CharacterControllers.Core.Interfaces;
using UnityEngine;

namespace Discovery.Game.Game.CharacterControllers.Core
{
    public abstract class MovementState<TCharacter, TStatus> : ScriptableObject, IMovementState
        where TStatus : IMovementStatus
        where TCharacter : ICharacter
    {
        public abstract int GetStatePriority(in TCharacter character);
        public abstract void Enter(in TCharacter character, ref TStatus status);
        public abstract void Exit(in TCharacter character, ref TStatus status);
        public abstract MovementInfos GetStateVelocity(in float deltaTime,
            in TCharacter character,
            ref TStatus status);

        public abstract TStatus Initialize(in TCharacter character);
        public virtual void Release(in TCharacter character, ref TStatus status) { }
    }
}