using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using Gameplay.Machines;

namespace Gameplay.Player
{
    public class PlayerStackController : MonoBehaviour
    {
        [SerializeField] Transform stackPosition;
        [SerializeField] float damping;
        [SerializeField] float getSpeed;
        [SerializedDictionary("ObjectName", "UpSpred")]
        public SerializedDictionary<string, float> upValues = new SerializedDictionary<string, float>();
        private List<Coroutine> _getFromStack = new List<Coroutine>();
        public List<Coroutine> getFromStack { get { return _getFromStack; } }
        private float currentUpSpred;
        private Stack<GameObject> objectStack = new Stack<GameObject>();
        private List<GameObject> objectList = new List<GameObject>();
        public int stackCount { get { return objectStack.Count; } }
        private void Start()
        {
            EventManager.placedFromPlayer += PlacedFromPlayer;
        }
        private void OnDestroy()
        {
            EventManager.placedFromPlayer -= PlacedFromPlayer;
        }
        private void Update()
        {
            Follow();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("TransformerGive"))
            {
                Transformer transformer = other.transform.parent.GetComponent<Transformer>();
                transformer.GetFromPlayer(objectStack, gameObject);
            }
            if (other.CompareTag("SpawnMachineArea"))
            {
                Spawner spawner = other.GetComponent<Spawner>();
                getFromStack.Add(StartCoroutine(GetFromStack(spawner)));
            }
            if (other.CompareTag("TransformerArea"))
            {
                Transformer transformer = other.transform.parent.GetComponent<Transformer>();
                getFromStack.Add(StartCoroutine(GetFromStack(transformer)));
            }
            if (other.CompareTag("Trash"))
            {
                Trash(other.transform.parent.position);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("SpawnMachineArea") || other.CompareTag("TransformerArea"))
            {
                foreach (var item in getFromStack)
                {
                    StopCoroutine(item);

                }
                getFromStack.Clear();
            }

            if (other.CompareTag("TransformerGive"))
            {
                Transformer transformer = other.transform.parent.GetComponent<Transformer>();
                transformer.StopGetPlayer(gameObject);
            }
        }
        private void Trash(Vector3 pos)
        {
            objectList.Clear();
            foreach (var item in objectStack)
            {
                item.transform.DOMove(pos, 0.1f).SetEase(Ease.Linear).OnComplete(() => ObjectPooler.instance.Release(item, item.tag));
            }
            objectStack.Clear();
        }
        private void PlacedFromPlayer(GameObject obj)
        {
            objectList.Remove(obj);
        }
        IEnumerator GetFromStack(Machine machine)
        {
            GameObject obj;

            yield return new WaitUntil(() => machine._spawnList.Count != 0);
            obj = machine.GetFromQueue(objectStack);
            if (obj != null)
            {
                currentUpSpred = upValues[obj.tag];


                objectList.Add(obj);
                objectStack.Push(obj);
            }
            yield return new WaitForSeconds(getSpeed);

            getFromStack.Clear();

            getFromStack.Add(StartCoroutine(GetFromStack(machine)));

        }

        private void Follow()
        {
            if (objectList.Count != 0)
            {
                var obj = objectList[0];

                var pos = new Vector3(stackPosition.transform.position.x, stackPosition.transform.position.y, stackPosition.transform.position.z);
                obj.transform.position = Vector3.Lerp(obj.transform.position, pos, damping * Time.deltaTime);

                obj.transform.rotation = stackPosition.rotation;

                for (int i = 1; i < objectList.Count; i++)
                {

                    var currentObj = objectList[i];
                    var lookObj = objectList[i - 1];
                    var y = obj.transform.position.y + (currentUpSpred * i);
                    pos = new Vector3(lookObj.transform.position.x, y, lookObj.transform.position.z);
                    currentObj.transform.position = Vector3.Lerp(currentObj.transform.position, pos, damping * Time.deltaTime);


                    currentObj.transform.rotation = Quaternion.Slerp(currentObj.transform.rotation, stackPosition.rotation, damping * Time.deltaTime);

                }
            }
        }
    }
}
