using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        private CharacterController characterController;
        [SerializeField] float speed = 1;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            EventManager.onJoystick += Move;
        }
        private void OnDestroy()
        {
            EventManager.onJoystick -= Move;
        }

        private void Move(Vector2 movement)
        {
            Vector3 move = new Vector3(movement.x, 0, movement.y) * speed;
            characterController.Move(move);
        }
    }
}
