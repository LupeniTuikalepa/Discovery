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

        [SerializeField, Range(.001f, 1)]
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

        public override SlideResult SlideAndCollide(Vector3 velocity, float deltaTime, bool apply = true)
        {
            SlideCollision[] buffer = new SlideCollision[maxBounces];
            int count = 0;

            Vector3 delta = velocity * deltaTime;
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
                inVelocity = velocity,
                outVelocity = finalDelta / deltaTime,
                collisions = buffer,
                collisionCount = count,
            };
        }

        protected Vector3 CollideAndSlide(Vector3 vel, Vector3 from, ref int depth, SlideCollision[] collisions)
        {
            if(depth >= maxBounces)
                return vel;

            Vector3 direction = vel.normalized;
            var distance = vel.magnitude;

            if (Cast(from, direction, out RaycastHit hit, distance, collisionLayerMask.value))
            {
                var distanceBeforeContact = (distance - hit.distance);
                collisions[depth] = new SlideCollision()
                {
                    point = hit.point,
                    normal = hit.normal,
                    collider = hit.collider,
                    collisionPosition = from + direction * distanceBeforeContact,
                    force = vel
                };
                depth++;

                float hitDistance = hit.distance;
                Vector3 bounceVelocity = Vector3.ProjectOnPlane(direction, hit.normal) * distanceBeforeContact;

                Debug.DrawRay(hit.point, bounceVelocity, Color.red);
                Vector3 recursive = CollideAndSlide(bounceVelocity, from + direction * hitDistance, ref depth, collisions);
                Debug.DrawRay(from + direction * hitDistance, bounceVelocity, Color.red);

                return direction * hitDistance + recursive;
            }

            return vel;
        }

        public override bool Cast(Vector3 from, Vector3 direction, out RaycastHit hit,  float maxDistance, int mask)
        {
            return Physics.CapsuleCast(GetPoint1(from), GetPoint2(from), Radius, direction, out hit, maxDistance, mask);
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


        public float Radius => capsuleCollider.radius + skinWidth;

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