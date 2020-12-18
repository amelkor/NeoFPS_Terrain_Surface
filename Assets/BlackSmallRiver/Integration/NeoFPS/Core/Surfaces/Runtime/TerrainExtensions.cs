#if NEOFPS
using UnityEngine;

namespace BlackSmallRiver.Integration.NeoFPS.Core.Surfaces
{
    public static class TerrainExtensions
    {
        /// <summary>
        /// Get terrain layer index in the TerrainData. Returns -1 if Terrain does not contain the provided layer.
        /// </summary>
        /// <param name="terrainData">TerrainData from a Unity Terrain component to find in.</param>
        /// <param name="layer">Terrain layer to find index of.</param>
        /// <returns>Terrain layer index in the TerrainData.</returns>
        public static int GetLayerIndex(this TerrainData terrainData, TerrainLayer layer)
        {
            var layers = terrainData.terrainLayers;

            for (var i = 0; i < layers.Length; i++)
            {
                if (layers[i] == layer)
                    return i;
            }

            return -1;
        }
    }
}
#endif