using Discovery.Game.CharacterControllers.Humanoid;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Discovery.Game.Player
{
    public class PlayerControls : MonoBehaviour, IHumanoidControls
    {
        public Vector3 Direction { get; private set; }

        public bool WantsToSprint => sprintInputAction.action.IsPressed();

        public bool WantsToJump => jumpInputAction.action.WasPerformedThisFrame();

        [SerializeField]
        private InputActionReference moveInputAction;
        [SerializeField]
        private InputActionReference sprintInputAction;
        [SerializeField]
        private InputActionReference jumpInputAction;

        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            GetInputDirection();
        }

        private void GetInputDirection()
        {

#if UNITY_EDITOR
            if(UnityEditor.EditorApplication.isPaused)
                return;
#endif
            Vector2 input = moveInputAction.action.ReadValue<Vector2>();

            Vector3 forward = Vector3.ProjectOnPlane(mainCamera.transform.forward, transform.up).normalized;
            Vector3 right = -Vector3.Cross(forward, transform.up).normalized;

            Direction = forward * input.y + right * input.x;
        }
    }
}