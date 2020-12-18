#if NEOFPS
using UnityEditor;

namespace BlackSmallRiver.Integration.NeoFPS.Core.Surfaces
{
    [CustomEditor(typeof(TerrainSurfaceSettings))]
    public class TerrainSurfaceSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This asset is controlled by TerrainSurface component for NeoFPS.\n Remove this if the corresponding TerrainSurface has been removed from the terrain.",
                MessageType.Info);
        }
    }
}
#endif