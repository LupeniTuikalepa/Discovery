using UnityEngine;

namespace Discovery.Game.CharacterControllers.States
{
    public struct MovementInfos
    {
        public Vector3 velocity;
        public bool snapToGround;
        public float gravityMultiplier;
    }
}