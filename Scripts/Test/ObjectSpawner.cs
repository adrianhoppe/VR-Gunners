using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRGunners.Test
{
    public class ObjectSpawner : MonoBehaviour
    {
        public GameObject ObjectPrefab;
        public int spawnWidth = 10;
        public int spawnLength = 10;
        public float ObjectDistance = 1;

        // Start is called before the first frame update
        void Start()
        {
            SpawnObjects();
        }

        // Update is called once per frame
        void SpawnObjects()
        {
            for (int y = 0; y < spawnLength; y++)
            {
                for (int x = 0; x < spawnWidth; x++)
                {
                    var go = GameObject.Instantiate(ObjectPrefab);
                    Vector3 pos = ((x - (spawnWidth / 2)) * transform.right
                       + (y - (spawnLength / 2)) * transform.forward);
                    go.transform.position = transform.position + pos * ObjectDistance;
                }
            }
        }
    }
}

