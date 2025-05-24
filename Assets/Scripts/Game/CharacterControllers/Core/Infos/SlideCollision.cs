using UnityEngine;

namespace Discovery.Game.CharacterControllers
{
    public struct SlideCollision
    {
        public Vector3 point;
        public Vector3 normal;
        public Vector3 inVel;
        public Vector3 projectedVel;
        public Vector3 collisionPosition;
        public Collider collider;
        public Vector3 collisionVel;
    }
}