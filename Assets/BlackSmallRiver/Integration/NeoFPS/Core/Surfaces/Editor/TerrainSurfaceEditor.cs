#if NEOFPS
using System;
using UnityEditor;
using UnityEngine;

namespace BlackSmallRiver.Integration.NeoFPS.Core.Surfaces
{
    [CustomEditor(typeof(TerrainSurface))]
    public class TerrainSurfaceEditor : Editor
    {
        private const string SURFACES_PROP = "surfaces";
        private const string TERRAIN_LAYER_PROP = "terrainLayer";
        private const string DEFAULT_SURFACE_PROP = "defaultSurface";
        private const string FPS_SURFACE_MATERIAL_PROP = "fpsSurfaceMaterial";
        private const string TERRAIN_SURFACE_SETTINGS_PROP = "terrainSurfaceSettings";

        private readonly GUILayoutOption[] m_TexturePreviewOptions = new[] {GUILayout.Width(40), GUILayout.Height(40)};
        private readonly GUIContent m_SurfaceLabel = new GUIContent("Surface", "NeoFPS Surface type");
        private GUIStyle m_RightAlignedLabel;

        private SerializedProperty m_DefaultSurfaceProperty;
        private SerializedProperty m_LayerSurfaceSettingsAsset;
        private SerializedProperty m_SurfacesProperty;
        private SerializedObject m_LayerSurfaceSettingsAssetObject;
        private UnityEngine.Terrain m_Terrain;

        private void Awake()
        {
            m_Terrain = ((Behaviour) target).GetComponent<UnityEngine.Terrain>();
            m_DefaultSurfaceProperty = serializedObject.FindProperty(DEFAULT_SURFACE_PROP);
            m_LayerSurfaceSettingsAsset = serializedObject.FindProperty(TERRAIN_SURFACE_SETTINGS_PROP);

            if (m_LayerSurfaceSettingsAsset.objectReferenceValue)
            {
                m_LayerSurfaceSettingsAssetObject = new SerializedObject(m_LayerSurfaceSettingsAsset.objectReferenceValue);
                m_SurfacesProperty = m_LayerSurfaceSettingsAssetObject.FindProperty(SURFACES_PROP);
            }
        }

        public override void OnInspectorGUI()
        {
            // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment for compatibility with Unity 2020.1 and lower
            if (m_RightAlignedLabel == null)
                m_RightAlignedLabel = new GUIStyle(EditorStyles.label) {alignment = TextAnchor.MiddleRight};

            if (!m_LayerSurfaceSettingsAsset.objectReferenceValue)
            {
                if (!GUILayout.Button("Create terrain surface settings"))
                    return;

                var path = EditorUtility.SaveFilePanelInProject(
                    "Save terrain surface settings",
                    $"NeoFPS_{m_Terrain.name}_surface_{Guid.NewGuid().ToString()}.asset",
                    "asset",
                    "Choose where to save the terrain surface asset");

                if (string.IsNullOrEmpty(path))
                    return;

                var layerSurfaceMapping = CreateInstance<TerrainSurfaceSettings>();
                m_LayerSurfaceSettingsAsset.objectReferenceValue = layerSurfaceMapping;

                AssetDatabase.CreateAsset(layerSurfaceMapping, path);
                AssetDatabase.SaveAssets();

                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();

                m_LayerSurfaceSettingsAssetObject = new SerializedObject(m_LayerSurfaceSettingsAsset.objectReferenceValue);
                m_SurfacesProperty = m_LayerSurfaceSettingsAssetObject.FindProperty(SURFACES_PROP);
            }

            serializedObject.Update();

            var previousGUIState = GUI.enabled;
            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_LayerSurfaceSettingsAsset);
            GUI.enabled = previousGUIState;

            EditorGUILayout.PropertyField(m_DefaultSurfaceProperty);
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("After terrain layers changed, they need to be synchronized with the surface component.", MessageType.Info);
            if (GUILayout.Button("Sync terrain layers", EditorStyles.miniButton))
            {
                ((TerrainSurface) target).EditorSyncTerrainLayers();
                m_LayerSurfaceSettingsAssetObject.Update();
            }

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("Set the surface types accordingly to the terrain textures below. The component tries to guess the surface by the texture name the first time a layer added.", MessageType.Info);
            m_SurfacesProperty.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(m_SurfacesProperty.isExpanded, $"Surfaces [{m_SurfacesProperty.arraySize.ToString()}]");
            EditorGUILayout.Space();

            if (m_SurfacesProperty.isExpanded)
            {
                for (var i = 0; i < m_SurfacesProperty.arraySize; i++)
                {
                    var elementProperty = m_SurfacesProperty.GetArrayElementAtIndex(i);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    var terrainLayer = (TerrainLayer) elementProperty.FindPropertyRelative(TERRAIN_LAYER_PROP).objectReferenceValue;
                    var preview = terrainLayer.diffuseTexture;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(terrainLayer.name, EditorStyles.centeredGreyMiniLabel);
                    var rect = EditorGUILayout.GetControlRect(m_TexturePreviewOptions);
                    EditorGUI.DrawPreviewTexture(rect, preview);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.LabelField(terrainLayer.diffuseTexture.name, m_RightAlignedLabel);
                    EditorGUILayout.PropertyField(elementProperty.FindPropertyRelative(FPS_SURFACE_MATERIAL_PROP), m_SurfaceLabel);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            m_LayerSurfaceSettingsAssetObject?.ApplyModifiedProperties();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif