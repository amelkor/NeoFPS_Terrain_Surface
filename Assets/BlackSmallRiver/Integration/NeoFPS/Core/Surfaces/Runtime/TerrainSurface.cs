#if NEOFPS
using System;
using System.Collections.Generic;
using NeoFPS;
using NeoFPS.Constants;
using UnityEngine;

namespace BlackSmallRiver.Integration.NeoFPS.Core.Surfaces
{
    /// <summary>
    /// This map is used to set each terrain layer to the corresponding NeoFPS surface type
    /// </summary>
    [RequireComponent(typeof(Terrain))]
    public class TerrainSurface : BaseSurface
    {
        [SerializeField, HideInInspector]
        private TerrainSurfaceSettings terrainSurfaceSettings;

        [SerializeField, HideInInspector, Tooltip("Will be used when terrain contains no layers.")]
        private FpsSurfaceMaterial defaultSurface = FpsSurfaceMaterial.Dirt;

        [Space, Header("NeoFPS DEBUG"), Tooltip("Will throw on call to GetSurface() if true, otherwise will return the default surface.")]
        [SerializeField] private bool throwOnGetSurfaceNoPosition;

        private Dictionary<int, FpsSurfaceMaterial> m_SurfaceMap;
        private CachedTerrain m_CachedTerrain;
        private int m_TerrainLayersCount;

        /// <summary>
        /// The Unity Terrain which this surface is attached to.
        /// </summary>
        public Terrain terrain => m_CachedTerrain.terrain;

        /// <summary>
        /// The amount of registered surfaces.
        /// </summary>
        public int surfaceLayersCount => m_TerrainLayersCount;

        private void Awake()
        {
            if (!TryGetComponent<Terrain>(out var t))
            {
                Debug.LogError("Terrain surface is missing the Terrain component");
                return;
            }

            m_CachedTerrain = new CachedTerrain(t);
        }

        private FpsSurfaceMaterial GetFpsSurfaceMaterial(Vector3 point)
        {
            if (m_TerrainLayersCount == 0)
                return defaultSurface;

            var index = m_CachedTerrain.GetDominatingLayerIndex(point);

            return m_SurfaceMap.TryGetValue(index, out var surface)
                ? surface
                : defaultSurface;
        }

        #region BaseSurface implementation (NeoFPS)

        public override FpsSurfaceMaterial GetSurface() =>
            throwOnGetSurfaceNoPosition
                ? throw new NotSupportedException("Can't determine terrain surface without a position")
                : defaultSurface;

        public override FpsSurfaceMaterial GetSurface(RaycastHit hit) => GetFpsSurfaceMaterial(hit.point);

        public override FpsSurfaceMaterial GetSurface(ControllerColliderHit hit) => GetFpsSurfaceMaterial(hit.point);

        #endregion

        #region Terrain layers sync (Editor)

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!terrainSurfaceSettings)
            {
                m_TerrainLayersCount = 0;
                return;
            }

            SyncTerrainLayersInternal();
        }

        // ReSharper disable once CognitiveComplexity
        private void SyncTerrainLayersInternal()
        {
            m_TerrainLayersCount = terrainSurfaceSettings.surfaces?.Count ?? 0;

            if (!TryGetComponent<Terrain>(out var t))
            {
                Debug.LogError("Terrain surface is missing Terrain component");
                return;
            }

            var terrainData = t.terrainData;
            
            // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment for compatibility with Unity 2020.1 and lower
            if(terrainSurfaceSettings.surfaces == null)
                terrainSurfaceSettings.surfaces = new List<TerrainLayerSurface>(terrainData.terrainLayers.Length);
            
            var surfaces = terrainSurfaceSettings.surfaces;
            var difference = m_TerrainLayersCount - terrainData.terrainLayers.Length;

            // remove orphan layers which have been removed from the terrain
            if (difference > 0)
            {
                var indexes = new List<int>(difference);
                var set = new HashSet<TerrainLayer>();

                foreach (var element in terrainData.terrainLayers)
                    set.Add(element);
                for (var i = 0; i < surfaces.Count; i++)
                {
                    if (set.Add(surfaces[i].terrainLayer))
                        indexes.Add(i);
                }

                for (var i = 0; i < difference; i++)
                {
                    surfaces.RemoveAt(indexes[i]);
                }

                Debug.Log($"Removed {difference.ToString()} orphaned terrain layer surfaces");
            }
            // add new layers from terrain data
            else if (difference < 0)
            {
                var indexes = new List<int>(Mathf.Abs(difference));
                var set = new HashSet<TerrainLayer>();

                foreach (var element in surfaces)
                    set.Add(element.terrainLayer);
                for (var i = 0; i < terrainData.terrainLayers.Length; i++)
                {
                    if (set.Add(terrainData.terrainLayers[i]))
                        indexes.Add(i);
                }

                for (var i = 0; i < indexes.Count; i++)
                {
                    var layer = terrainData.terrainLayers[indexes[i]];
                    var surface = new TerrainLayerSurface(layer, TryGuessSurfaceByTextureName(layer.diffuseTexture.name));
                    surfaces.Add(surface);

                    Debug.Log($"New TerrainLayerSurface added: {surface.ToString()}");
                }
            }

            m_TerrainLayersCount = surfaces.Count;
            m_SurfaceMap = new Dictionary<int, FpsSurfaceMaterial>(m_TerrainLayersCount);

            foreach (var surface in surfaces)
            {
                var index = terrainData.GetLayerIndex(surface.terrainLayer);
                if (index == -1)
                {
                    Debug.LogError($"Can not find terrain layer {surface.terrainLayer.name} in the {t.name} terrain data");
                    continue;
                }

                m_SurfaceMap.Add(index, surface.fpsSurfaceMaterial);
            }
        }

        /// <summary>
        /// Simply guessing the texture type if it contains a FpsSurfaceMaterial name.
        /// </summary>
        /// <param name="texture">Texture name to guess</param>
        /// <returns></returns>
        private static FpsSurfaceMaterial TryGuessSurfaceByTextureName(string texture)
        {
            texture = texture.ToUpper();
            for (var i = 0; i < FpsSurfaceMaterial.names.Length; i++)
            {
                if (texture.Contains(FpsSurfaceMaterial.names[i].ToUpper()))
                    return new FpsSurfaceMaterial() {value = (byte) i};
            }

            return FpsSurfaceMaterial.Default;
        }

        /// <summary>
        /// Sync terrain layers with the surface component. Call this only in Unity Editor.
        /// </summary>
        public void EditorSyncTerrainLayers() => SyncTerrainLayersInternal();

#endif

        #endregion
    }
}
#endif