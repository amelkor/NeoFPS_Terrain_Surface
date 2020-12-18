#if NEOFPS
using System.Collections.Generic;
using UnityEngine;

namespace BlackSmallRiver.Integration.NeoFPS.Core.Surfaces
{
    /// <summary>
    /// Stores terrain layers mapping to NeoFPS surface types.
    /// </summary>
    /// <note>
    /// Do not create this manually.
    /// </note>
    public class TerrainSurfaceSettings : ScriptableObject
    {
        [SerializeField, HideInInspector]
        internal List<TerrainLayerSurface> surfaces;
    }
}
#endif