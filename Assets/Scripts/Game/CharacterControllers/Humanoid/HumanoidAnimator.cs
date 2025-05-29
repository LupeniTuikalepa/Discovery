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

        public Animator Animator => animator;

        private void LateUpdate()
        {
            animator.SetBool(IsGrounded, character.IsGrounded);
            animator.SetBool(IsSprinting, character.IsInState<SprintState>());

            Vector3 stateVelocity = transform.InverseTransformDirection(character.StateVelocity) * .2f;

            var fade = 25f;
            FadeAnimatorFloatValue(XStateSpeed, stateVelocity.x, fade);
            FadeAnimatorFloatValue(YStateSpeed, stateVelocity.y, fade);
            FadeAnimatorFloatValue(ZStateSpeed, stateVelocity.z, fade);

            FadeAnimatorFloatValue(StateSpeed, stateVelocity.magnitude, fade * 50);
            FadeAnimatorFloatValue(GroundSpeed, character.GetHorizontalVector(stateVelocity).magnitude, fade * 50);

            Vector3 currentVelocity = transform.InverseTransformDirection(character.CurrentVelocity)* .2f;
            FadeAnimatorFloatValue(XCurrentSpeed, currentVelocity.x, fade);
            FadeAnimatorFloatValue(YCurrentSpeed, currentVelocity.y, fade);
            FadeAnimatorFloatValue(ZCurrentSpeed, currentVelocity.z, fade);
            FadeAnimatorFloatValue(CurrentSpeed, currentVelocity.magnitude, fade * 50);

        }

        public AnimatorStateInfo GetAnimatorStateInfo(int layer = 0) => animator.IsInTransition(layer)
            ? animator.GetNextAnimatorStateInfo(layer)
            : animator.GetCurrentAnimatorStateInfo(layer);

        public int GetCurrentStateHash(int layer = 0)
        {
            return GetAnimatorStateInfo(layer).shortNameHash;
        }

        public bool IsInState(int stateHash, int layer = 0) =>
            animator.GetCurrentAnimatorStateInfo(layer).shortNameHash == stateHash ||
            (animator.IsInTransition(layer) && animator.GetNextAnimatorStateInfo(layer).shortNameHash == stateHash);

        public void PlayState(int stateHash, float transition = 0, int layer = 0, float normalizedTimeOffset = 0) =>
            animator.CrossFade(stateHash, transition, layer, normalizedTimeOffset);


        public void FadeAnimatorFloatValue(int hash, float target, float t)
        {
            float current = animator.GetFloat(hash);
            float value = Mathf.MoveTowards(current, target, t * Time.deltaTime);

            animator.SetFloat(hash, Mathf.Approximately(value, target) ? target : value);
        }
    }
}