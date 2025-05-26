using System;
using Discovery.Game.CharacterControllers.Infos;
using UnityEngine;

namespace Discovery.Game.CharacterControllers.Bodies
{
    /// <summary>
    /// Class responsible for applying movement to a characters.
    /// Handles collisions etc...
    /// </summary>
    [DefaultExecutionOrder(5)]
    public abstract class CharacterBody : MonoBehaviour
    {
        private static readonly SlideCollision[] Buffer = new SlideCollision[64];

        public Vector3 Position { get; protected set; }
        public Quaternion Rotation { get; protected set; }


        [Header("Default")]
        [field: SerializeField, Range(.001f, .01f)]
        public float SkinWidth { get; private set; } = .01f;

        public MovementResult AddMovement(Vector3 velocity, float deltaTime) => AddMovement(velocity * deltaTime);

        public MovementResult AddMovement(Vector3 delta)
        {
            int count = ComputeMovement(delta, out Vector3 finalTranslation, Buffer);

            Vector3 newPosition = Position + finalTranslation;
            Vector3 lastPosition = Position;
            Position = newPosition;

            return new MovementResult()
            {
                from = lastPosition,
                to = newPosition,
                collisionCount = count,
                collisions = Buffer,
            };;
        }


        private void Reset()
        {
            CollectDependencies();
        }

        private void Awake()
        {
            if(!HasComponents())
                CollectDependencies();

            UpdatePositionAndRotation();
            OnAwake();
        }


        public virtual bool HasComponents() => true;

        protected void OnValidate()
        {
            if (!Application.isPlaying)
            {
                CollectDependencies();
            }
        }

        protected virtual void OnAwake()
        {

        }

        protected virtual void CollectDependencies()
        {

        }

        private void Update()
        {
            UpdatePositionAndRotation();
        }

        protected virtual void FixedUpdate()
        {
            ApplyChanges();
        }

        public abstract int ComputeMovement(Vector3 translation, out Vector3 finalTranslation, SlideCollision[] buffer);

        public abstract bool Cast(Vector3 from, Vector3 direction, out RaycastHit hit, float maxDistance, int mask);

        protected abstract void ApplyChanges();
        public abstract void UpdatePositionAndRotation();

        public bool Cast(Vector3 direction, out RaycastHit hit, float maxDistance, int mask) => Cast(Position, direction, out hit, maxDistance, mask);

        public virtual void Teleport(Vector3 newPos)
        {
            Position = newPos;
        }
    }
}