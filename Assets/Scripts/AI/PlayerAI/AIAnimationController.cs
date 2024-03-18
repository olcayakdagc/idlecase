using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.PlayerAI
{
    public class AIAnimationController : MonoBehaviour
    {
        [SerializeField] Animator animator;

        private void Awake()
        {
            EventManager.onAIMove += SetMovement;
        }
        private void OnDestroy()
        {
            EventManager.onAIMove -= SetMovement;
        }
        private void SetMovement(bool move)
        {
            animator.SetBool("move", move);
        }
    }
}

