#if NEOFPS
using System;
using NeoFPS.Constants;
using UnityEngine;

namespace BlackSmallRiver.Integration.NeoFPS.Core.Surfaces
{
    /// <summary>
    /// Used to map a Unity terrain layer to a NeoFPS surface material.
    /// </summary>
    [Serializable]
    public struct TerrainLayerSurface
    {
        [Tooltip("The surface material will be mapped to the terrain layer")]
        public TerrainLayer terrainLayer;

        [Tooltip("Surface which corresponds to the terrain layer")]
        public FpsSurfaceMaterial fpsSurfaceMaterial;

        public TerrainLayerSurface(TerrainLayer terrainLayer, FpsSurfaceMaterial fpsSurfaceMaterial)
        {
            this.terrainLayer = terrainLayer;
            this.fpsSurfaceMaterial = fpsSurfaceMaterial;
        }

        public override string ToString()
        {
            return new System.Text.StringBuilder()
                .Append("<b>Instance Id:</b> [").Append(terrainLayer.GetInstanceID())
                .Append("] Terrain layer: [").Append(terrainLayer.name)
                .Append("] Diffuse texture: [").Append(terrainLayer.diffuseTexture.name)
                .Append("] NeoFPS surface: [").Append(fpsSurfaceMaterial.ToString()).Append("]")
                .ToString();
        }
    }
}
#endif