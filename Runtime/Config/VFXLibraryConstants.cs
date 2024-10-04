using System.Collections.Generic;
using UnityEngine;

namespace WTFGames.Hephaestus.VFX
{
    [CreateAssetMenu(fileName = "VFXLibraryConstants", menuName = "HephaestusMobile/Core/VFX/VFXLibraryConstants", order = 1)]
    public class VFXLibraryConstants : ScriptableObject
    {
        [HideInInspector]
        public string enumsPath;
        
        [HideInInspector]
        public List<string> uiMapKeys = new List<string>();
    }
}