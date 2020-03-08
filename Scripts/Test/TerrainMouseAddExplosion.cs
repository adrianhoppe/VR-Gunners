using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRGunners.Terrain;

namespace VRGunners.Test
{
    public class TerrainMouseAddExplosion : MonoBehaviour
    {
        LevelTerrain terrain;
        public float ExplosionForce = 10;
        public float ExplosionRadius = 5;

        // Start is called before the first frame update
        void Start()
        {
            terrain = FindObjectOfType<LevelTerrain>();
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                //left mouse pressed

                Vector3 mousePos = Input.mousePosition;
                Ray ray = Camera.main.ScreenPointToRay(mousePos);
                if(Physics.Raycast(ray, out RaycastHit hitInfo))
                {
                    if (hitInfo.collider.GetComponent<LevelTerrain>() == terrain)
                    {
                        //hit terrain
                        Debug.DrawLine(ray.origin, hitInfo.point, Color.green, 5);
                        terrain.AddExplosionForce(ExplosionForce, hitInfo.point, ray.direction, ExplosionRadius);
                    }
                }
            }
        }
    }
}
