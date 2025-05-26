using UnityEngine;

namespace Discovery.Game.CharacterControllers.Infos
{
    public struct MovementResult
    {
        public Vector3 from;
        public Vector3 to;

        public int collisionCount;
        public SlideCollision[] collisions;
    }
}