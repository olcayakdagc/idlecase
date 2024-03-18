using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace Gameplay.Machines
{
    public class Transformer : Machine
    {
        [SerializeField] float getRate;
        [SerializeField] Transform getPosition;
        [SerializeField] int makeLimitation;
        [SerializeField] string getAssetName;
        [SerializeField] float makeRate;
        [SerializeField] int storageCount;

        private Stack<GameObject> spawnableObjects = new Stack<GameObject>();
        Dictionary<GameObject, Coroutine> coroutines = new Dictionary<GameObject, Coroutine>();
        Coroutine makeCoroutine = null;

        public void StopGetPlayer(GameObject player)
        {

            if (coroutines[player] != null)
                StopCoroutine(coroutines[player]);
        }
        public void GetFromPlayer(Stack<GameObject> stack, GameObject player)
        {
            if (stack.Count == 0) return;
            coroutines[player] = StartCoroutine(GetEnum(stack));
        }
        private IEnumerator GetEnum(Stack<GameObject> stack)
        {
            int count = stack.Count;
            if (stack.TryPeek(out GameObject obj) && obj.tag.Equals(getAssetName))
            {
                for (int i = 0; i < count; i++)
                {
                    yield return new WaitUntil(() => spawnableObjects.Count <= storageCount);
                    obj = stack.Pop();

                    EventManager.placedFromPlayer?.Invoke(obj);

                    spawnableObjects.Push(obj);
                    Place(obj, spawnableObjects.Count - 1, getPosition);
                    yield return new WaitForSeconds(getRate);
                    SpawnAfter();

                }

            }

        }


        private void SpawnAfter()
        {
            if (makeCoroutine == null)
                makeCoroutine = StartCoroutine(SpawnAfterEnum());
        }
        IEnumerator SpawnAfterEnum()
        {
            yield return new WaitForSeconds(makeRate);

            Debug.Log("makeEnum");

            while (spawnableObjects.Count != 0)
            {
                if (_spawnList.Count > makeLimitation)
                {
                    animator.SetBool("Working", false);

                    yield return new WaitUntil(() => _spawnList.Count <= makeLimitation);
                    animator.SetBool("Working", true);

                }

                animator.SetBool("Working", true);

                var obj = spawnableObjects.Pop();
                obj.transform.DOJump(spawnPos.position, 0.2f, 1, spawnRate).OnComplete(() => ObjectPooler.instance.Release(obj, getAssetName));
                yield return new WaitForSeconds(makeRate);

                Spawn(startPosition, spawnObjectName, _spawnList.Count);
            }
            makeCoroutine = null;
            animator.SetBool("Working", false);
        }
    }
}

