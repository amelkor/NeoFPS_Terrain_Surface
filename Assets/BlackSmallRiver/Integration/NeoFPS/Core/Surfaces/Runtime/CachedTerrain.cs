#if NEOFPS
using UnityEngine;

namespace BlackSmallRiver.Integration.NeoFPS.Core.Surfaces
{
    public readonly struct CachedTerrain
    {
        public readonly int gameObjectInstanceId;
        public readonly Terrain terrain;
        public readonly TerrainData terrainData;
        public readonly Transform transform;

        public bool isEmpty => gameObjectInstanceId == 0;
        public static CachedTerrain empty => new CachedTerrain();

        public CachedTerrain(Terrain terrain)
        {
            var gameObject = terrain.gameObject;

            this.terrain = terrain;
            terrainData = terrain.terrainData;
            transform = gameObject.transform;
            gameObjectInstanceId = gameObject.GetInstanceID();
        }

        /// <summary>
        /// Returns index of the dominating terrain layer based on the splat map texture.
        /// </summary>
        /// <param name="at">Position to probe at.</param>
        /// <returns>Terrain layer index.</returns>
        public int GetDominatingLayerIndex(Vector3 at)
        {
            var terrainPos = transform.position;

            var mapX = (int) ((at.x - terrainPos.x) / terrainData.size.x * terrainData.alphamapWidth);
            var mapZ = (int) ((at.z - terrainPos.z) / terrainData.size.z * terrainData.alphamapHeight);

            var splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);
            var cellMix = new float[splatmapData.GetUpperBound(2) + 1];

            for (var i = 0; i < cellMix.Length; i++)
                cellMix[i] = splatmapData[0, 0, i];

            float maxMix = 0;
            var index = 0;

            for (var i = 0; i < cellMix.Length; i++)
            {
                // ReSharper disable once InvertIf
                if (cellMix[i] > maxMix)
                {
                    index = i;
                    maxMix = cellMix[i];
                }
            }

            return index;
        }
    }
}
#endif