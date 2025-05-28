using System;
using Discovery.Game.CharacterControllers.Bodies;
using UnityEngine;

namespace Discovery.Game
{
    public class OutOfBounds : MonoBehaviour
    {
        [SerializeField]
        private Transform respawnPoint;

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out CharacterBody characterBody))
            {
                characterBody.Teleport(respawnPoint.position);
                Debug.Break();
            }
        }
    }
}