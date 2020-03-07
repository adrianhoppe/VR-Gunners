using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRGunners.Weapons
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour
    {
        public float Lifetime = 20;

        public float ExplosionForce = 10;
        public float ExplosionRadius = 5;

        float starttime;

        // Start is called before the first frame update
        void Awake()
        {
            starttime = Time.time;
        }

        // Update is called once per frame
        void Update()
        {
            CheckLifetime();
        }

        private void CheckLifetime()
        {
            float currentLifetime = Time.time - starttime;

            //too old
            if (currentLifetime > Lifetime)
                Destroy(gameObject);
        }

        void OnCollisionEnter(Collision collision)
        {
            //Explode on impact
            Explode();
        }

        const int maxOverlapCount = 250;
        Collider[] overlapResults = new Collider[maxOverlapCount];
        void Explode()
        {            
            //add explosion force to surrounding objects
            Vector3 explosionPos = transform.position;
            int count = Physics.OverlapSphereNonAlloc(explosionPos, ExplosionRadius, overlapResults);

            for (int i = 0; i < count; i++)
            {
                Rigidbody rb = overlapResults[i].GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Debug.Log("Apply force to " + overlapResults[i].gameObject.name);
                    rb.AddExplosionForce(ExplosionForce, explosionPos, ExplosionRadius, 0, ForceMode.Force);
                }
            }

            //destroy
            Destroy(gameObject);
        }
    }
}