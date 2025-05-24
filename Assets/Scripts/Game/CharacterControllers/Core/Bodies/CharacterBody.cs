using Discovery.Game.Game.CharacterControllers.Core.Infos;
using UnityEngine;

namespace Discovery.Game.Game.CharacterControllers.Core.Bodies
{
    /// <summary>
    /// Class responsible for applying movement to a characters.
    /// Handles collisions etc...
    /// </summary>
    public abstract class CharacterBody : MonoBehaviour
    {


        public SlideResult SlideAndCollide(Vector3 velocity, float deltaTime, bool apply = true) =>
            SlideAndCollide(velocity * deltaTime, apply);

        public abstract TeleportationResult Teleport(Vector3 point, bool apply = true);
        public abstract SlideResult SlideAndCollide(Vector3 delta, bool apply = true);

        public abstract bool Cast(Vector3 from, Vector3 direction, out RaycastHit hit, float maxDistance, int mask);
        public abstract Vector3 GetPosition();
        public abstract Quaternion GetRotation();

        public bool Cast(Vector3 direction, out RaycastHit hit, float maxDistance, int mask) => Cast(GetPosition(), direction, out hit, maxDistance, mask);
    }
}