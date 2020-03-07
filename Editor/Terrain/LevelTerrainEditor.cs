using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRGunners.Terrain
{
    [CustomEditor(typeof(LevelTerrain))]
    public class LevelTerrainEditor : Editor
    {
        bool foldoutTerrainProperties = true;
        public LevelTerrain Terrain;

        private void OnEnable()
        {
            Terrain = (LevelTerrain)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawPropertiesEditor(Terrain.TerrainProperties, Terrain.OnTerrainPropertiesChanged, ref foldoutTerrainProperties);
        }

        private void DrawPropertiesEditor(Object properties, System.Action OnChange, ref bool foldout)
        {
            if (properties == null) return;

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                foldout = EditorGUILayout.InspectorTitlebar(foldout, properties);
                Editor editor = CreateEditor(properties);
                editor.OnInspectorGUI();

                if(check.changed)
                {
                    OnChange?.Invoke();
                }
            }
        }
    }
}