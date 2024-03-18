using Gameplay.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI.PlayerAI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AIController : MonoBehaviour
    {
        [SerializeField] List<Transform> destinations;
        [SerializeField] float speed;
        [SerializeField] float waitForGetTime;

        [SerializeField] int stackCount;
        [SerializeField] PlayerStackController stackController;

        private NavMeshAgent agent;
        private int destination;
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = speed;
            StartWondering();
        }
       
        private void StartWondering()
        {

            StartCoroutine(DestinationEnum());
        }
        IEnumerator DestinationEnum()
        {
            GoToDestination();
            yield return new WaitUntil(() => stackController.stackCount == stackCount);
            GoToDestination();
            yield return new WaitUntil(() => stackController.stackCount == 0);
            GoToDestination();
            yield return new WaitUntil(() => !agent.pathPending);
            yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);
            yield return new WaitUntil(() => !agent.hasPath || agent.velocity.sqrMagnitude == 0f);
            yield return new WaitForSeconds(waitForGetTime);

            GoToDestination();
            yield return new WaitUntil(() => stackController.stackCount == 0);
            StartWondering();
        }
        private void GoToDestination()
        {
            if (destination == destinations.Count)
            {
                destination = 0;
            }
            agent.destination = destinations[destination].position;
            EventManager.onAIMove?.Invoke(true);
            StartCoroutine(WaitDestination());
            destination++;
        }
        IEnumerator WaitDestination()
        {
            yield return new WaitUntil(() => !agent.pathPending);
            yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);
            yield return new WaitUntil(() => !agent.hasPath || agent.velocity.sqrMagnitude == 0f);
            EventManager.onAIMove?.Invoke(false);


        }

    }
}
