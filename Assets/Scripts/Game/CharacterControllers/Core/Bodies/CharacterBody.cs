using System;
using System.Collections.Generic;
using Discovery.Game.CharacterControllers.States;

using UnityEngine;


namespace Discovery.Game.CharacterControllers
{
    /// <summary>
    /// Class responsible for applying movement to a characters.
    /// Handles collisions etc...
    /// </summary>
    public abstract class CharacterBody : MonoBehaviour
    {
        public struct SlideResult
        {
            public Vector3 from;
            public Vector3 to;
            public Vector3 inVelocity;
            public Vector3 outVelocity;

            public int collisionCount;
            public SlideCollision[] collisions;
        }

        public struct SlideCollision
        {
            public Vector3 point;
            public Vector3 normal;
            public Vector3 force;
            public Vector3 collisionPosition;
            public Collider collider;
        }

        public struct TeleportationResult
        {

        }

        public abstract TeleportationResult Teleport(Vector3 point);
        public abstract SlideResult SlideAndCollide(Vector3 delta, float deltaTime, bool apply = true);
        public abstract bool Cast(Vector3 from, Vector3 direction, out RaycastHit hit, float maxDistance, int mask);
        public abstract Vector3 GetPosition();
        public abstract Quaternion GetRotation();

        public bool Cast(Vector3 direction, out RaycastHit hit, float maxDistance, int mask) => Cast(GetPosition(), direction, out hit, maxDistance, mask);
    }
}