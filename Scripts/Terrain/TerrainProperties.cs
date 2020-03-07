using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRGunners.Terrain
{
    [CreateAssetMenu()]
    public class TerrainProperties : ScriptableObject
    {
        public float TileSize = 1;
        public int Width = 100;
        public int Height = 250;
    }
}