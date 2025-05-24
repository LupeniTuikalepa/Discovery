using UnityEngine;

namespace Discovery.Game.CharacterControllers
{
    public struct SlideResult
    {
        public Vector3 from;
        public Vector3 to;
        public Vector3 inDelta;
        public Vector3 outDelta;

        public int collisionCount;
        public SlideCollision[] collisions;
    }
}