using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Machines
{

    public class Machine : MonoBehaviour
    {
        public Animator animator;

        [SerializeField] GameObject prefab;
        public Transform startPosition;
        public Transform spawnPos;
        public string spawnObjectName;
        public float xSpread = 0.2f;
        public float zSpread = 0.2f;
        public float ySpread = 1;
        public int width;
        public int height;
        public int up;
        public float spawnRate = 0.1f;
        public float moveSpeed = 0.1f;
        public int maxOneLineValue { get; set; }
        public int maxValue { get; set; }



        private Stack<GameObject> spawnList = new Stack<GameObject>();
        public Stack<GameObject> _spawnList { get { return spawnList; } }

        private void Awake()
        {
            maxValue = width * height * up;
            maxOneLineValue = width * height;
        }

        public GameObject GetFromQueue(Stack<GameObject> playerStack)
        {
            if (spawnList.Count == 0) return null;
            if (playerStack.Count == 0 || playerStack.Peek().tag.Equals(spawnList.Peek().tag))
            {
                return spawnList.Pop();

            }
            return null;
        }
        public virtual void Spawn(Transform startPos, string assetName, int count)
        {
            GameObject obj = ObjectPooler.instance.Spawn(assetName, spawnPos.position, Quaternion.identity, startPosition);
            Place(obj, count, startPos);
            _spawnList.Push(obj);
        }
        public void Place(GameObject obj, int count, Transform position)
        {
            int currentUp = Mathf.FloorToInt((count) / maxOneLineValue);
            var currentLineValue = (count) - (currentUp * maxOneLineValue);
            int currentWidth = currentLineValue / height;
            int currentHeight = currentLineValue % height;

            var x = position.transform.position.x + (currentWidth * xSpread);
            var z = position.transform.position.z + (currentHeight * zSpread);
            var y = position.transform.position.y + (currentUp * ySpread);
            Vector3 pos = new Vector3(x, y, z);
            obj.transform.DOJump(pos, 0.2f, 1, moveSpeed);
            obj.transform.DORotate(Vector3.zero, moveSpeed);
        }
    }

}
