using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRGunners.Terrain;

namespace VRGunners.Weapons
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour
    {
        public float Lifetime = 20;

        public float ExplosionForce = 10;
        public float ExplosionRadius = 5;
        public float RigidbodyExplosionForceScale = 10000;

        float starttime;
        Rigidbody rb;

        // Start is called before the first frame update
        void Awake()
        {
            starttime = Time.time;
            rb = GetComponent<Rigidbody>();
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
            Explode(collision);
        }

        const int maxOverlapCount = 250;
        
        Collider[] overlapResults = new Collider[maxOverlapCount];
        void Explode(Collision collision)
        {            
            //add explosion force to surrounding objects
            Vector3 explosionPos = transform.position;
            int count = Physics.OverlapSphereNonAlloc(explosionPos, ExplosionRadius, overlapResults);

            for (int i = 0; i < count; i++)
            {
                Rigidbody otherRb = overlapResults[i].GetComponent<Rigidbody>();
                if (otherRb != null)
                {
                    Debug.Log("Apply force to " + overlapResults[i].gameObject.name);
                    otherRb.AddExplosionForce(ExplosionForce * RigidbodyExplosionForceScale, explosionPos, ExplosionRadius, 0, ForceMode.Force);
                }

                LevelTerrain terrain = overlapResults[i].GetComponent<LevelTerrain>();
                if (terrain != null)
                {
                    Debug.Log("Apply force to " + overlapResults[i].gameObject.name);
                    terrain.AddExplosionForce(ExplosionForce, explosionPos, this.rb.velocity.normalized ,ExplosionRadius);
                }
            }

            //destroy
            Destroy(gameObject);
        }
    }
}