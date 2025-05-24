using System;
using UnityEngine;

namespace Discovery.Game.CharacterControllers
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterRigidbody : CharacterBody
    {
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

        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            rb = GetComponent<Rigidbody>();
            capsuleCollider = GetComponent<CapsuleCollider>();
        }

        private void Awake()
        {
            if(rb == null)
                rb = GetComponent<Rigidbody>();
            if(capsuleCollider == null)
                capsuleCollider = GetComponent<CapsuleCollider>();
        }


        public override TeleportationResult Teleport(Vector3 point)
        {
            rb.position = point;

            return new TeleportationResult();
        }

        public override SlideResult SlideAndCollide(Vector3 delta, bool apply = true)
        {
            SlideCollision[] buffer = new SlideCollision[maxBounces];
            int count = 0;

            Vector3 from = GetPosition();
            Debug.DrawRay(from, delta * 10, Color.red);
            Vector3 finalDelta = CollideAndSlide(delta, from, ref count, buffer);
            Debug.DrawRay(from, finalDelta * 10, Color.magenta);

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