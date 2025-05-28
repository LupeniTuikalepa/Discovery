using System;
using System.Collections.Generic;
using Discovery.Game.CharacterControllers.Bodies;
using Discovery.Game.CharacterControllers.Gravity;
using Discovery.Game.CharacterControllers.Infos;
using Discovery.Game.CharacterControllers.Interfaces;
using LTX.ChanneledProperties.Priorities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Discovery.Game.CharacterControllers
{
    [RequireComponent(typeof(CharacterBody)), DefaultExecutionOrder(2)]
    public abstract class Character<TCharacter> : MonoBehaviour, ICharacter where TCharacter : Character<TCharacter>
    {
        public Vector3 CurrentVelocity => StateVelocity + ExternalVelocity;

        public Vector3 StateVelocity { get; private set; }
        public Vector3 ExternalVelocity { get; private set; }
        public Vector3 GroundNormal { get; private set; }
        public Vector3 GroundPoint { get; private set; }
        public bool IsGrounded { get; private set; }

        [field: Header("Dependencies")]
        [field: SerializeField]
        public CharacterBody Body { get; private set; }

        [Header("Ground")]
        [SerializeField]
        private float groundDetectionRange;
        [SerializeField, Range(0, 1)]
        private float minGroundSteepness = .5f;
        [SerializeField]
        private bool snapToGround;
        [SerializeField]
        private LayerMask groundLayerMast = Physics.AllLayers;

        [field: Header("Gravity")]
        [field: SerializeField]
        public Priority<IGravityField> GravityField { get; private set; }

        [SerializeField, Range(0, 10)]
        private float gravityScale = 1;

        public Vector3 Gravity
        {
            get
            {
                IGravityField gravityFieldValue = GravityField.Value;
                Vector3 baseGravity = gravityFieldValue?.GetGravityDirection(Body.Position) ?? Physics.gravity;

                return baseGravity * gravityScale;
            }
        }


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
            HandleRotation();

            float deltaTime = Time.fixedDeltaTime;
            HandleStates();

            MovementInfos movementInfos = HandleMovement(deltaTime);
            HandleGround();
        }

        protected virtual void HandleRotation()
        {

        }

        private void HandleStates()
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
        }

        protected virtual void HandleGround()
        {
            Vector3 gravityDirection = Gravity.normalized;
            if (Body.Cast(gravityDirection, out RaycastHit hit, groundDetectionRange, groundLayerMast))
            {
                float dot = Vector3.Dot(-gravityDirection, hit.normal);

                if (dot >= minGroundSteepness)
                {
                    GroundPoint = hit.point;
                    GroundNormal = hit.normal;
                    IsGrounded = true;
                    return;
                }
            }

            GroundPoint = Body.Position;
            GroundNormal = -gravityDirection;
            IsGrounded = false;
        }

        private MovementInfos HandleMovement(float deltaTime)
        {
            MovementInfos movementInfos = ComputeMovementForCurrentState(deltaTime);
            Vector3 gravity = Gravity * movementInfos.gravityMultiplier;


            Vector3 deltaVelocity = movementInfos.velocity * deltaTime;

            MovementResult result = Body.AddMovement(deltaVelocity);
            StateVelocity =  (result.to - result.from) / deltaTime;


            Vector3 delta = Vector3.zero;
            //Adding gravity as an external force
            forcesBuffer.Add(gravity * deltaTime);

            foreach (var force in forcesBuffer)
            {
                result = Body.AddMovement(force, deltaTime);
                delta += (result.to - result.from) / deltaTime;
            }

            ExternalVelocity = delta;

            forcesBuffer.Clear();

            return movementInfos;
        }


        public void AddForce(Vector3 force)
        {
            forcesBuffer.Add(force);
        }

        public bool TryGetCurrentState<TState>(out TState state)
        {
            if (components.TryGetValue(currentStateID, out IMovementStateRunner runner) &&
                runner.State is TState movementState)
            {
                state = movementState;
                return true;
            }
            state = default;
            return false;
        }

        public bool IsState<TState>() =>
            components.TryGetValue(currentStateID, out IMovementStateRunner runner) &&
            runner.State is TState;

        public bool TryGetCurrentState<TState, TStatus>(out TState state, out TStatus status)
            where TState : MovementState<TCharacter, TStatus>
            where TStatus : IMovementStatus
        {
            if (components.TryGetValue(currentStateID, out IMovementStateRunner runner) &&
                runner.State is TState movementState && runner.Status is TStatus movementStatus)
            {
                state = movementState;
                status = movementStatus;
                return true;
            }

            state = null;
            status = default;
            return false;
        }

        public bool TryGetCurrentState(out IMovementState state, out IMovementStatus status)
        {
            state = null;
            status = null;
            if (components.TryGetValue(currentStateID, out IMovementStateRunner runner))
            {
                state = runner.State;
                status = runner.Status;
                return true;
            }

            return false;
        }

        public IMovementState GetCurrentState() => components.TryGetValue(currentStateID, out IMovementStateRunner runner) ? runner.State : null;

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

                //Debug.Log($"{newPriority} for {movementStateRunner.State.GetType().Name}");
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
            };
        }

        protected abstract TCharacter GetCharacter();

    }
}