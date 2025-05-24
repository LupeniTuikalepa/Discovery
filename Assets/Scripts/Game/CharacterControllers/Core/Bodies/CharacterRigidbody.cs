using Discovery.Game.Game.CharacterControllers.Core.Infos;
using UnityEngine;

namespace Discovery.Game.Game.CharacterControllers.Core.Bodies
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterRigidbody : CharacterBody
    {
        private static Collider[] results = new Collider[32];

        [SerializeField]
        private Rigidbody rb;
        [SerializeField]
        private CapsuleCollider capsuleCollider;

        [SerializeField, Range(.001f, .01f)]
        private float skinWidth = .1f;
        [SerializeField]
        private LayerMask collisionLayerMask;
        [SerializeField, Range(1, 10)]
        private int maxBounces = 5;
        [SerializeField, Range(1, 10)]
        private int maxDepenetrationSteps = 5;


        private void Reset()
        {
            CollectDependencies();
        }

        private void OnValidate()
        {
            CollectDependencies();
        }
        private void Awake()
        {
            OnAwake();
            CollectDependencies();
        }

        private void FixedUpdate()
        {
            ComputeDepenetration();
        }


        protected virtual void OnAwake()
        {

        }

        private void ComputeDepenetration()
        {
            Vector3 capsulePosition = capsuleCollider.transform.position;
            Quaternion capsuleRotation = capsuleCollider.transform.rotation;

            for (int i = 0; i < maxDepenetrationSteps; i++)
            {
                int count = Physics.OverlapCapsuleNonAlloc(
                    GetPoint1(),
                    GetPoint2(),
                    capsuleCollider.radius + skinWidth,
                    results,
                    collisionLayerMask,
                    QueryTriggerInteraction.Ignore);

                for (int j = 0; j < count; j++)
                {
                    var other = results[j];
                    bool hasRigidbody = other.attachedRigidbody;
                    Vector3 otherPosition = hasRigidbody ? other.attachedRigidbody.position : other.transform.position;
                    Quaternion otherRotation = hasRigidbody ? other.attachedRigidbody.rotation : other.transform.rotation;


                    if (Physics.ComputePenetration(
                            other, otherPosition, otherRotation,
                            capsuleCollider, capsulePosition, capsuleRotation,
                            out Vector3 direction, out float distance))
                    {
                        Vector3 offset = direction * distance;
                        rb.position += offset;
                        capsulePosition += offset;
                    }
                }
            }
        }


        public virtual bool HasComponents() => capsuleCollider != null && rb != null;

        protected virtual void CollectDependencies()
        {
            if(rb == null)
                rb = GetComponent<Rigidbody>();

            if (capsuleCollider == null)
            {
                if(!TryGetComponent(out capsuleCollider))
                    capsuleCollider = GetComponentInChildren<CapsuleCollider>();
            }

        }

        public override TeleportationResult Teleport(Vector3 point, bool apply = true)
        {
            if(apply)
                rb.position = point;

            return new TeleportationResult();
        }

        public override SlideResult SlideAndCollide(Vector3 delta, bool apply = true)
        {
            SlideCollision[] buffer = new SlideCollision[maxBounces];
            int count = 0;

            Vector3 from = GetPosition();
            Vector3 finalDelta = CollideAndSlide(delta, from, ref count, buffer);

            Vector3 to = from + finalDelta;
            if (apply)
            {
                /*
                rb.linearVelocity = finalDelta;
                */
                rb.MovePosition(to);
            }

            return new SlideResult()
            {
                from = from,
                to = to,
                inDelta = delta,
                outDelta = finalDelta,
                collisions = buffer,
                collisionCount = count,
            };
        }

        protected Vector3 CollideAndSlide(Vector3 delta, Vector3 from, ref int depth, SlideCollision[] collisions)
        {
            if(depth >= maxBounces)
                return delta;

            Vector3 normDirection = delta.normalized;
            var distance = delta.magnitude;

            if (Cast(from, normDirection, out RaycastHit hit, distance, collisionLayerMask.value))
            {
                float distanceBeforeContact = hit.distance - skinWidth;
                float distanceAfterContact = distance - distanceBeforeContact;

                Vector3 projectedDelta = Vector3.ProjectOnPlane(normDirection, hit.normal) * distanceAfterContact;
                Vector3 newFrom = from + normDirection * distanceBeforeContact;
                Debug.DrawRay(newFrom, projectedDelta * 100);
                Debug.DrawRay(from, projectedDelta * 100, Color.coral);

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

        public override bool Cast(Vector3 from, Vector3 direction, out RaycastHit hit,  float maxDistance, int mask)
        {
            return Physics.CapsuleCast(GetPoint1(from), GetPoint2(from), capsuleCollider.radius , direction, out hit, maxDistance, mask);
        }

        public override Vector3 GetPosition() => rb.position;

        public override Quaternion GetRotation() => rb.rotation;


        protected Vector3 GetCapsuleAxis() => capsuleCollider.direction switch
        {
            0 => capsuleCollider.transform.right,
            1 => capsuleCollider.transform.up,
            2 => capsuleCollider.transform.forward,
            _ => transform.up
        };



        protected Vector3 GetCenterPoint() => GetCenterPoint(capsuleCollider.transform.position);
        protected Vector3 GetCenterPoint(Vector3 pos) => pos + capsuleCollider.center;
        protected Vector3 GetPoint1() => GetPoint1(capsuleCollider.transform.position);
        protected Vector3 GetPoint1(Vector3 pos) => GetTop(pos) - GetCapsuleAxis() * capsuleCollider.radius;
        protected Vector3 GetPoint2() => GetPoint2(capsuleCollider.transform.position);
        protected Vector3 GetPoint2(Vector3 pos) => GetBottom(pos) + GetCapsuleAxis() * capsuleCollider.radius;
        protected Vector3 GetTop() => GetTop(capsuleCollider.transform.position);
        protected Vector3 GetTop(Vector3 pos) => GetCenterPoint(pos) + GetCapsuleAxis() * (capsuleCollider.height * .5f);
        protected Vector3 GetBottom() => GetBottom(capsuleCollider.transform.position);
        protected Vector3 GetBottom(Vector3 pos) => GetCenterPoint(pos) - GetCapsuleAxis() * (capsuleCollider.height * .5f);

    }
}