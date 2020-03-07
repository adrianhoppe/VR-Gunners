using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRGunners.Weapons
{
    public class GunControl : MonoBehaviour
    {
        public Transform RotateJoint;
        public Transform TiltJoint;
        public GameObject BulletPrefab;
        public Transform BarrelEnd;

        public float RotateSpeed = 5;
        public float TiltSpeed = 2;
        public float GunPower = 20;

        List<Bullet> newBullets = new List<Bullet>();
        Collider[] gunColliders;

        // Start is called before the first frame update
        void Awake()
        {
            gunColliders = GetComponentsInChildren<Collider>();
        }

        // Update is called once per frame
        void Update()
        {
            KeyboardControls();
        }

        void FixedUpdate()
        {
            CheckNewBullets();
        }

        #region controls
        void KeyboardControls()
        {
            float rotate = 0;
            float tilt = 0;

            rotate += Input.GetKey(KeyCode.D) ? 1 : 0;
            rotate -= Input.GetKey(KeyCode.A) ? 1 : 0;
            tilt += Input.GetKey(KeyCode.W) ? 1 : 0;
            tilt -= Input.GetKey(KeyCode.S) ? 1 : 0;

            Rotate(rotate);
            Tilt(tilt);

            if (Input.GetKeyDown(KeyCode.Space))
                Shoot();
        }

        //positive is rotate right
        void Rotate(float amount)
        {
            if (amount == 0) return;

            float value = amount * RotateSpeed * Time.deltaTime;
            RotateJoint.Rotate(Vector3.up * value, Space.Self);
        }

        //positive is tilt up
        void Tilt(float amount)
        {
            if (amount == 0) return;

            float value = amount * TiltSpeed * Time.deltaTime;
            TiltJoint.Rotate(-Vector3.right * value, Space.Self);
        }
        #endregion controls

        #region bullets
        void Shoot()
        {
            //spawn bullet
            var bulletGo = GameObject.Instantiate(BulletPrefab);

            Bullet bullet = bulletGo.GetComponent<Bullet>();
            if (bullet == null)
                throw new System.ArgumentException("Bullet Prefab does not have a Bullet script attached!");
            //add force in FixedUpdate
            newBullets.Add(bullet);

            //set to spawn at barrel end
            bulletGo.transform.position = BarrelEnd.position;
            bulletGo.transform.rotation = BarrelEnd.rotation;

            //ignore collision between bullet and gun colliders
            var bulletColliders = bulletGo.GetComponentsInChildren<Collider>();
            foreach (var cb in bulletColliders)
            {
                foreach (var cg in gunColliders)
                {
                    //Debug.Log("Add Ignore = " + cb.gameObject.name + " - " + cg.gameObject.name);
                    Physics.IgnoreCollision(cb, cg);
                }
            }
        }

        private void CheckNewBullets()
        {
            foreach (Bullet b in newBullets)
            {
                var rb = b.GetComponent<Rigidbody>();
                rb.AddForce(b.transform.forward * GunPower, ForceMode.Impulse);
            }

            newBullets.Clear();
        }
        #endregion bullets
    }
}