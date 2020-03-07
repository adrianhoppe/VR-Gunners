using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRGunners.Terrain
{
    public class LevelTerrain : MonoBehaviour
    {
        public TerrainProperties TerrainProperties;

        [SerializeField, HideInInspector]
        MeshFilter meshFilter;

        //private void OnValidate()
        //{
        //    OnTerrainPropertiesChanged();
        //}

        public void Initialize()
        {
            if (meshFilter == null)
            {
                MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
                meshFilter = gameObject.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = new Mesh();
            }
        }

        void GenerateMesh()
        {
            int width = TerrainProperties.Width;
            int height = TerrainProperties.Height;

            Vector3[] vertices = new Vector3[width * height];
            int[] triangles = new int[(width - 1) * (height - 1) * 6];
            int triIndex = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int i = x + y * width;
                    Vector3 pos = TerrainProperties.TileSize * ((x - (width / 2)) * transform.right + (y - (height / 2)) * transform.forward);
                    vertices[i] = pos;

                    if (x != width - 1 && y != height - 1)
                    {
                        triangles[triIndex] = i;
                        triangles[triIndex + 1] = i + width;
                        triangles[triIndex + 2] = i + width + 1;

                        triangles[triIndex + 3] = i;
                        triangles[triIndex + 4] = i + width + 1;
                        triangles[triIndex + 5] = i + 1;
                        triIndex += 6;
                    }
                }
            }

            meshFilter.sharedMesh.Clear();
            meshFilter.sharedMesh.vertices = vertices;
            meshFilter.sharedMesh.triangles = triangles;
            meshFilter.sharedMesh.RecalculateNormals();
        }

        public void OnTerrainPropertiesChanged()
        {
            Initialize();
            GenerateMesh();
        }
    }
}