using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Player
{
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] CharacterController characterController;
        [SerializeField] Animator animator;

        private void Start()
        {
            EventManager.onJoystick += SetMovement;
        }
        private void OnDestroy()
        {
            EventManager.onJoystick -= SetMovement;
        }
        private void SetMovement(Vector2 movement)
        {
            if (characterController.velocity.sqrMagnitude > 0.1f)
            {
                animator.SetBool("move", true);

            }
            else
            {
                animator.SetBool("move", false);
            }
        }
    }
}

