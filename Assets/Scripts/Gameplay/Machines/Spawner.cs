using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Gameplay.Machines
{
    public class Spawner : Machine
    {
        private List<GameObject> players = new List<GameObject>();
        private void Start()
        {
            StartSpawn();
        }
        private void StartSpawn()
        {
            animator.SetBool("Working", true);
            StartCoroutine(SpawnEnum());
        }



        public IEnumerator SpawnEnum()
        {

            int currentUp = Mathf.FloorToInt(_spawnList.Count / maxOneLineValue);
            var currentLineValue = _spawnList.Count - (currentUp * maxOneLineValue);
            int currentWidth = currentLineValue / height;
            int currentHeight = currentLineValue % height;


            for (int p = currentUp; p < up; p++)
            {
                for (int i = currentWidth; i < width; i++)
                {
                    for (int j = currentHeight; j < height; j++)
                    {

                        Spawn(startPosition, spawnObjectName, _spawnList.Count);
                        yield return new WaitForSeconds(spawnRate);
                    }
                    currentHeight = 0;
                }
                currentWidth = 0;
            }
            animator.SetBool("Working", false);
            yield return new WaitUntil(() => _spawnList.Count < maxValue);
            StartSpawn();
        }
    }
}

