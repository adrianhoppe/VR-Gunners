using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRGunners.Terrain
{
    public class LevelTerrain : MonoBehaviour
    {
        public TerrainProperties TerrainProperties;
        public Material Material;

        [SerializeField, HideInInspector]
        MeshFilter meshFilter;
        [SerializeField, HideInInspector]
        MeshCollider meshCollider;
        MeshRenderer meshRenderer;

        public void OnTerrainPropertiesChanged()
        {
            Initialize();
            GenerateMesh();
        }

        public void Initialize()
        {
            if (meshRenderer == null)
            {
                //get or add
                meshRenderer = gameObject.GetComponent<MeshRenderer>();
                if (meshRenderer == null)
                    meshRenderer = gameObject.AddComponent<MeshRenderer>();
            }

            if (Material == null)
            {
                Material = new Material(Shader.Find("Standard"));
            }

            meshRenderer.sharedMaterial = Material;

            if (meshFilter == null)
            {
                //get or add
                meshFilter = gameObject.GetComponent<MeshFilter>();
                if (meshFilter == null)
                    meshFilter = gameObject.AddComponent<MeshFilter>();

            }

            if (meshFilter.sharedMesh == null)
            {
                meshFilter.sharedMesh = new Mesh();
            }

            if (meshCollider == null)
            {
                //get or add
                meshCollider = gameObject.GetComponent<MeshCollider>();
                if (meshCollider == null)
                    meshCollider = gameObject.AddComponent<MeshCollider>();
            }

            meshCollider.sharedMesh = meshFilter.sharedMesh;
            meshCollider.convex = false;
            meshCollider.cookingOptions = MeshColliderCookingOptions.None;
        }

        void GenerateMesh()
        {
            int width = TerrainProperties.Width;
            int length = TerrainProperties.Length;

            Vector3[] vertices = new Vector3[width * length];
            Vector2[] uvs = new Vector2[width * length];
            int[] triangles = new int[(width - 1) * (length - 1) * 6];

            float minHeight = float.MaxValue;
            for (int y = 0; y < length; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int i = x + y * width;

                    //calculate position
                    float z = Mathf.PerlinNoise(TerrainProperties.NoiseScale * x, TerrainProperties.NoiseScale * y);
                    Vector3 pos = ((x - (width / 2)) * transform.right
                       + (y - (length / 2)) * transform.forward)
                       + z * transform.up;

                    //scale position
                    pos.x *= TerrainProperties.TileSize; //right
                    pos.z *= TerrainProperties.TileSize; //forward
                    pos.y *= TerrainProperties.HeightScale; //up

                    //remember lowest height, to ground terrain
                    if (minHeight > pos.y)
                        minHeight = pos.y;

                    vertices[i] = pos;

                    //calculate triangle indices
                    if (x != width - 1 && y != length - 1)
                    {
                        int triIndex = (x + y * (width - 1)) * 6;
                        triangles[triIndex] = i;
                        triangles[triIndex + 1] = i + width;
                        triangles[triIndex + 2] = i + width + 1;

                        triangles[triIndex + 3] = i;
                        triangles[triIndex + 4] = i + width + 1;
                        triangles[triIndex + 5] = i + 1;
                    }

                    //calculate uvs
                    Vector2 percent = new Vector2(x / (float)(width - 1), y / (float)(length - 1));
                    uvs[i] = percent;
                }
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].y -= minHeight;
            }

            UpdateMesh(true, vertices, uvs, triangles);
        }

        void UpdateMesh(in Vector3[] vertices) => UpdateMesh(false, vertices, null, null);
        void UpdateMesh(bool clearOldMeshData, in Vector3[] vertices, in Vector2[] uvs, in int[] triangles)
        {
            //update mesh filter
            if (clearOldMeshData)
                meshFilter.sharedMesh.Clear();
            meshFilter.sharedMesh.vertices = vertices;
            if (uvs != null)
                meshFilter.sharedMesh.SetUVs(0, uvs);
            if (triangles != null)
                meshFilter.sharedMesh.triangles = triangles;
            meshFilter.sharedMesh.RecalculateNormals();

            //update mesh collider
            meshCollider.sharedMesh = meshFilter.sharedMesh;
        }

        public void AddExplosionForce(float explosionForce, Vector3 explosionPos, Vector3 explosionDir, float explosionRadius)
        {
            //get copy of vertices
            Vector3[] vertices = meshFilter.sharedMesh.vertices;

            int width = TerrainProperties.Width;
            int length = TerrainProperties.Length;
            float sqrExplosionRadius = explosionRadius * explosionRadius;


            for (int y = 0; y < length; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int i = x + y * width;

                    Vector3 dir = vertices[i] - (explosionPos - explosionDir.normalized);

                    float sqrDistance = Vector3.SqrMagnitude(dir);
                    if (sqrDistance > sqrExplosionRadius)
                    {
                        //is not in radius
                        continue;
                    }

                    float normalizedDist = sqrDistance / sqrExplosionRadius;
                    const float distanceFactor = 2;
                    float strength = Mathf.Pow(1 - normalizedDist, distanceFactor) * explosionForce;

                    vertices[i].y += (strength * dir.normalized).y;
                }
            }

            UpdateMesh(vertices);

            SmoothTerain();
        }

        public float pullCloserStrength = 0.5f;
        void SmoothTerain()
        {
            //get copy of vertices
            Vector3[] vertices = meshFilter.sharedMesh.vertices;

            int width = TerrainProperties.Width;
            int length = TerrainProperties.Length;

            (int, int)[] offsets = new (int, int)[] { (-1, -1), (1, 1), (-1, 1), (1, -1) };

            for (int y = 1; y < length - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                                                            
                    int i = x + y * width;

                    for (int j = 0; j < offsets.Length; j++)
                    {
                        int i2 = (x + offsets[j].Item1) + (y + offsets[j].Item2) * width;

                        Vector3 v = vertices[i2] - vertices[i];
                        float dist = v.y;

                        if (dist < TerrainProperties.TileSize * 2)
                            continue;

                        //move closer together
                        //vertices[i2] += Vector3.up * 0.2f;

                        vertices[i].y += dist * pullCloserStrength;
                        vertices[i2].y -= dist * pullCloserStrength;
                    }                    
                }
            }

            UpdateMesh(vertices);
        }
    }
}