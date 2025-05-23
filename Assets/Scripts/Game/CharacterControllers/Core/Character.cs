using System.Collections.Generic;
using Discovery.Game.CharacterControllers;
using Discovery.Game.CharacterControllers.States;
using UnityEngine;

namespace Discovery.Game.Game.CharacterControllers.Core
{
    [RequireComponent(typeof(CharacterBody))]
    public abstract class Character<TCharacter> : MonoBehaviour, ICharacter where TCharacter : Character<TCharacter>
    {
        public Vector3 CurrentVelocity => StateVelocity + ReferentialVelocity;

        public Vector3 StateVelocity { get; private set; }
        public Vector3 ReferentialVelocity { get; private set; }

        [field: SerializeField]
        public CharacterBody Body { get; private set; }

        private Dictionary<int, IMovementStateRunner> components;
        private int currentStateID;

        private List<Vector3> forcesBuffer = new();

        private void Awake()
        {
            components = new Dictionary<int, IMovementStateRunner>();
            if(Body == null)
                Body = GetComponent<CharacterBody>();

            OnAwake();
        }

        protected virtual void OnAwake() { }

        private void FixedUpdate()
        {
            int nextState = GetNextStateID();
            if (nextState != currentStateID)
            {
                IMovementStateRunner runner;

                if (components.TryGetValue(currentStateID, out runner))
                    runner.Exit();
                if (components.TryGetValue(nextState, out runner))
                    runner.Enter();
            }

            currentStateID = nextState;
            float deltaTime = Time.fixedDeltaTime;

            MovementInfos movementInfos = ComputeMovementForCurrentState(deltaTime);
            CharacterBody.SlideResult result = Body.SlideAndCollide(movementInfos.velocity, deltaTime);

            StateVelocity = result.outVelocity;
        }


        public bool TryGetCurrentState<TState, TComponent>(out TState state)
            where TState : MovementState<TCharacter, TComponent>
            where TComponent : IMovementStatus
        {
            if (components.TryGetValue(currentStateID, out IMovementStateRunner runner) &&
                runner.State is TState movementState)
            {
                state = movementState;
                return true;
            }

            state = null;
            return false;
        }

        public bool RegisterMovementState<T>(MovementState<TCharacter, T> movementState) where T : IMovementStatus
        {
            var id = movementState.GetInstanceID();
            if(components.TryAdd(id, null))
            {
                TCharacter character = GetCharacter();
                components[id] = new MovementStateRunner<TCharacter, T>(
                    character,
                    movementState.Initialize(character),
                    movementState);

                return true;
            }

            return false;
        }

        public bool UnregisterMovementState<T>(MovementState<TCharacter, T> movementState) where T : IMovementStatus
        {
            if (components.Remove(movementState.GetInstanceID(), out IMovementStateRunner runner))
            {
                if (runner is MovementStateRunner<TCharacter, T> movementStateRunner)
                    movementState.Release(in movementStateRunner.character, ref movementStateRunner.status);

                return true;
            }

            return false;
        }

        protected int GetNextStateID()
        {
            int nextStateID = 0;
            int priority = -1;

            foreach ((var id, IMovementStateRunner movementStateRunner) in components)
            {
                int newPriority = movementStateRunner.GetStatePriority();
                if (newPriority > priority)
                {
                    priority = newPriority;
                    nextStateID = id;
                }
            }

            return nextStateID;
        }
        protected virtual MovementInfos ComputeMovementForCurrentState(float deltaTime)
        {
            if (components.TryGetValue(currentStateID, out IMovementStateRunner runner))
                return runner.Execute(deltaTime);

            return new MovementInfos()
            {
                velocity = Vector3.zero,
                snapToGround = false,
            };
        }

        protected abstract TCharacter GetCharacter();
    }
}