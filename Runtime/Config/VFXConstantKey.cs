using System;

namespace WTFGames.Hephaestus.VFX
{
    [Serializable]
    public class VFXConstantKey
    {
        public string name;

        /// <summary>
        /// Stable value of the generated enum member. It never changes when keys are reordered or removed.
        /// </summary>
        public int id;
    }
}
