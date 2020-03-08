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
        public int Length = 250;
        public float HeightScale = 1;
        public float NoiseScale = 1;
    }
}