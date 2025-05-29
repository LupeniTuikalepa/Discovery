using System;
using Discovery.Game.CharacterControllers.Infos;
using LTX.ChanneledProperties.Priorities;
using UnityEngine;

namespace Discovery.Game.CharacterControllers.Bodies
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterBody : MonoBehaviour
    {
        private static readonly SlideCollision[] Buffer = new SlideCollision[64];
        private static readonly Collider[] Results = new Collider[32];

        public event Action OnFixedUpdate;

        [SerializeField]
        private Rigidbody rb;
        [SerializeField]
        private CapsuleCollider capsuleCollider;

        [SerializeField]
        private LayerMask collisionLayerMask;
        [SerializeField, Range(1, 10)]
        private int maxBounces = 5;
        [SerializeField, Range(1, 10)]
        private int maxDepenetrationSteps = 5;


        public Vector3 Position { get; protected set; }
        public Quaternion Rotation { get; protected set; }

        [Header("Default")]
        [field: SerializeField, Range(.001f, .01f)]
        public float SkinWidth { get; private set; } = .01f;


        public Priority<Vector2> CapsuleSize { get; private set; }

        private void Reset()
        {
            CollectDependencies();
        }

        private void Awake()
        {
            if(!HasComponents())
                CollectDependencies();

            Position = rb.position;
            Rotation = rb.rotation;
            CapsuleSize = new Priority<Vector2>(new Vector2(capsuleCollider.radius, capsuleCollider.height));
            UpdatePositionAndRotation();
        }


        protected void OnValidate()
        {
            if (!Application.isPlaying)
            {
                CollectDependencies();
            }
        }

        public bool HasComponents() => capsuleCollider != null && rb != null;

        protected void CollectDependencies()
        {
            if(rb == null)
                rb = GetComponent<Rigidbody>();

            if (capsuleCollider == null)
            {
                if(!TryGetComponent(out capsuleCollider))
                    capsuleCollider = GetComponentInChildren<CapsuleCollider>();
            }

        }


        protected virtual void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();

            Vector2 size = CapsuleSize;
            capsuleCollider.radius = size.x;
            capsuleCollider.height = size.y;

            ComputeDepenetration();
            ApplyChanges();
        }

        public MovementResult AddMovement(Vector3 velocity, float deltaTime)
            => AddMovement(velocity * deltaTime);

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
            };
        }

        private void ComputeDepenetration()
        {
            Vector3 capsulePosition = Position;
            Quaternion capsuleRotation = Rotation;

            for (int i = 0; i < maxDepenetrationSteps; i++)
            {
                Vector3 p1 = GetPoint1(capsulePosition);
                Vector3 p2 = GetPoint2(capsulePosition);

                int count = Physics.OverlapCapsuleNonAlloc(
                    p1,
                    p2,
                    capsuleCollider.radius + SkinWidth,
                    Results,
                    collisionLayerMask,
                    QueryTriggerInteraction.Ignore);

                for (int j = 0; j < count; j++)
                {
                    var other = Results[j];
                    bool hasRigidbody = other.attachedRigidbody;
                    if(other.attachedRigidbody == rb)
                        continue;

                    Vector3 otherPosition = hasRigidbody ? other.attachedRigidbody.position : other.transform.position;
                    Quaternion otherRotation = hasRigidbody ? other.attachedRigidbody.rotation : other.transform.rotation;

                    if (Physics.ComputePenetration(
                            other, otherPosition, otherRotation,
                            capsuleCollider, capsulePosition, capsuleRotation,
                            out Vector3 direction, out float distance))
                    {
                        Vector3 offset = direction * (distance + SkinWidth);
                        capsulePosition -= offset;
                    }
                }
            }

            Position = capsulePosition;
        }

        public int ComputeMovement(Vector3 translation, out Vector3 finalTranslation, SlideCollision[] buffer)
        {
            int count = 0;

            finalTranslation = CollideAndSlide(translation, Position, ref count, buffer);

            return count;
        }

        protected Vector3 CollideAndSlide(Vector3 delta, Vector3 from, ref int depth, SlideCollision[] collisions)
        {
            if(depth >= maxBounces)
                return delta;

            Vector3 normDirection = delta.normalized;
            var distance = delta.magnitude;

            if (Cast(from, normDirection, out RaycastHit hit, distance, collisionLayerMask.value))
            {
                float distanceBeforeContact = hit.distance - SkinWidth;
                float distanceAfterContact = distance - distanceBeforeContact;

                Vector3 projectedDelta = Vector3.ProjectOnPlane(normDirection, hit.normal) * distanceAfterContact;
                Vector3 newFrom = from + normDirection * distanceBeforeContact;

                collisions[depth] = new SlideCollision()
                {
                    point = hit.point,
                    normal = hit.normal,
                    collider = hit.collider,
                    collisionPosition = from + normDirection * distanceBeforeContact,
                    inVel = normDirection * distanceBeforeContact,
                    collisionVel = normDirection * distanceAfterContact,
                    projectedVel = projectedDelta,
                };

                depth++;
                Vector3 recursive = CollideAndSlide(projectedDelta, newFrom, ref depth, collisions);
                return normDirection.normalized * distanceBeforeContact + recursive;
            }

            return delta;
        }

        public bool Cast(Vector3 direction, out RaycastHit hit, float maxDistance, int mask)
            => Cast(Position, direction, out hit, maxDistance, mask);

        public bool Cast(Vector3 from, Vector3 direction, out RaycastHit hit,  float maxDistance, int mask)
        {
            Vector3 p1 = GetPoint1(from);
            Vector3 p2 = GetPoint2(from);

            return Physics.CapsuleCast(p1, p2, capsuleCollider.radius , direction, out hit, maxDistance + SkinWidth, mask, QueryTriggerInteraction.Ignore);
        }

        protected void ApplyChanges()
        {
            rb.MovePosition(Position);

            if(float.IsNaN(Rotation.x) || float.IsNaN(Rotation.y) || float.IsNaN(Rotation.z) || float.IsNaN(Rotation.w))
                Rotation = rb.rotation;

            Rotation.Normalize();
            rb.MoveRotation(Rotation);
        }

        public void UpdatePositionAndRotation()
        {
            /*
            Position = rb.position;
            Rotation = rb.rotation;
            */
        }


        protected Vector3 GetCapsuleAxis() => capsuleCollider.direction switch
        {
            0 => capsuleCollider.transform.right,
            1 => capsuleCollider.transform.up,
            2 => capsuleCollider.transform.forward,
            _ => transform.up
        };



        protected Vector3 GetCapsulePosition()
        {
            return Position + capsuleCollider.center;
        }

        protected Vector3 GetCenterPoint() => GetCenterPoint(GetCapsulePosition());
        protected Vector3 GetCenterPoint(Vector3 pos) => pos + capsuleCollider.center;
        protected Vector3 GetPoint1() => GetPoint1(GetCapsulePosition());
        protected Vector3 GetPoint1(Vector3 pos) => GetTop(pos) - GetCapsuleAxis() * capsuleCollider.radius;
        protected Vector3 GetPoint2() => GetPoint2(GetCapsulePosition());
        protected Vector3 GetPoint2(Vector3 pos) => GetBottom(pos) + GetCapsuleAxis() * capsuleCollider.radius;
        protected Vector3 GetTop() => GetTop(GetCapsulePosition());
        protected Vector3 GetTop(Vector3 pos) => GetCenterPoint(pos) + GetCapsuleAxis() * (capsuleCollider.height * .5f);
        protected Vector3 GetBottom() => GetBottom(GetCapsulePosition());
        protected Vector3 GetBottom(Vector3 pos) => GetCenterPoint(pos) - GetCapsuleAxis() * (capsuleCollider.height * .5f);


        public virtual void Teleport(Vector3 newPos)
        {
            Position = newPos;
        }

        public virtual void SetRotation(Quaternion quaternion)
        {
            Rotation = quaternion;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(GetPoint1(transform.position), capsuleCollider.radius);
            Gizmos.DrawWireSphere(GetPoint2(transform.position), capsuleCollider.radius);
        }
    }
}