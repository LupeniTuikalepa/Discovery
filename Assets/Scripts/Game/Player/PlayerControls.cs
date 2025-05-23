using System;
using Discovery.Game.CharacterControllers.States;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Discovery.Game.CharacterControllers
{
    public class PlayerControls : MonoBehaviour, IHumanoidControls
    {
        public Vector3 Direction
        {
            get
            {
                Vector2 vector2 = moveInputAction.action.ReadValue<Vector2>();
                return new Vector3(vector2.x, 0, vector2.y);
            }
        }

        public bool WantsToSprint => sprintInputAction.action.IsPressed();

        public bool WantsToJump => jumpInputAction.action.WasPerformedThisFrame();

        [SerializeField]
        private InputActionReference moveInputAction;
        [SerializeField]
        private InputActionReference sprintInputAction;
        [SerializeField]
        private InputActionReference jumpInputAction;

    }
}