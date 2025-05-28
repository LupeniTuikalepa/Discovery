using System;
using Discovery.Game.CharacterControllers.Humanoid;
using Discovery.Game.CharacterControllers.Humanoid.States;
using Discovery.Game.Player;
using UnityEngine;

namespace Discovery.Game
{
    public class HumanoidAnimator : MonoBehaviour
    {
        private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
        private static readonly int IsSprinting = Animator.StringToHash("IsSprinting");
        private static readonly int XStateSpeed = Animator.StringToHash("XStateSpeed");
        private static readonly int YStateSpeed = Animator.StringToHash("YStateSpeed");
        private static readonly int ZStateSpeed = Animator.StringToHash("ZStateSpeed");
        private static readonly int StateSpeed = Animator.StringToHash("StateSpeed");
        private static readonly int GroundSpeed = Animator.StringToHash("GroundSpeed");
        private static readonly int XCurrentSpeed = Animator.StringToHash("XCurrentSpeed");
        private static readonly int YCurrentSpeed = Animator.StringToHash("YCurrentSpeed");
        private static readonly int ZCurrentSpeed = Animator.StringToHash("ZCurrentSpeed");
        private static readonly int CurrentSpeed = Animator.StringToHash("CurrentSpeed");

        [SerializeField]
        private HumanoidCharacter character;
        [SerializeField]
        private Animator animator;



        private void LateUpdate()
        {
            animator.SetBool(IsGrounded, character.IsGrounded);
            animator.SetBool(IsSprinting, character.IsState<SprintState>());

            Vector3 stateVelocity = transform.InverseTransformDirection(character.StateVelocity);

            var fade = 25f;
            Fade(XStateSpeed, stateVelocity.x, fade);
            Fade(YStateSpeed, stateVelocity.y, fade);
            Fade(ZStateSpeed, stateVelocity.z, fade);

            Fade(StateSpeed, stateVelocity.magnitude, fade * 50);
            Fade(GroundSpeed, character.GetHorizontalVector(stateVelocity).magnitude, fade * 50);

            Vector3 currentVelocity = transform.InverseTransformDirection(character.CurrentVelocity);
            Fade(XCurrentSpeed, currentVelocity.x, fade);
            Fade(YCurrentSpeed, currentVelocity.y, fade);
            Fade(ZCurrentSpeed, currentVelocity.z, fade);
            Fade(CurrentSpeed, currentVelocity.magnitude, fade * 50);

        }

        public void PlayState(int stateHash, float transition = 0, int layer = 0, float normalizedTimeOffset = 0) =>
            animator.CrossFade(stateHash, transition, layer, normalizedTimeOffset);


        public void Fade(int hash, float target, float t)
        {
            float current = animator.GetFloat(hash);
            float value = Mathf.MoveTowards(current, target, t * Time.deltaTime);

            animator.SetFloat(hash, Mathf.Approximately(value, target) ? target : value);
        }
    }
}