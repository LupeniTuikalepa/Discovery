using System;
using UnityEngine;

namespace Discovery.Game.Game.CharacterControllers.Core
{
    public static class MathExtension
    {
        public static Vector3 Project(this Vector3 vector, Vector3 normal)
        {
            float dist = Vector3.Dot(vector, normal.normalized);
            Vector3 projected = dist * normal;
            return projected;
        }

        public static Vector3 ProjectOnPlane(this Vector3 vector, Vector3 normal)
        {
            Vector3 projected = vector - vector.Project(normal);
            return projected;
        }

        public static float AngleFrom(this Vector3 vector1, Vector3 vector2)
        {
            return Mathf.Acos(Vector3.Dot(vector1.normalized, vector2.normalized));
        }
    }
}