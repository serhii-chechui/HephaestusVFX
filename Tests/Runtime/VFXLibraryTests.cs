using NUnit.Framework;
using UnityEngine;

namespace WTFGames.Hephaestus.VFX.Tests
{
    public class VFXLibraryTests
    {
        private VFXLibrary _library;
        private GameObject _spark;
        private GameObject _smoke;

        [SetUp]
        public void SetUp()
        {
            _spark = new GameObject("Spark");
            _smoke = new GameObject("Smoke");

            _library = ScriptableObject.CreateInstance<VFXLibrary>();

            // Out of id order on purpose: lookup must go by id, not by position.
            _library.vfxList.Add(new VFXNamePair { vfxType = (int)TestVFXType.SMOKE, vfxPrefab = _smoke });
            _library.vfxList.Add(new VFXNamePair { vfxType = (int)TestVFXType.SPARK, vfxPrefab = _spark });
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_spark);
            Object.DestroyImmediate(_smoke);
            Object.DestroyImmediate(_library);
        }

        [Test]
        public void GetPrefabByType_MappedKey_ReturnsItsPrefab()
        {
            Assert.AreSame(_spark, _library.GetPrefabByType(TestVFXType.SPARK));
            Assert.AreSame(_smoke, _library.GetPrefabByType(TestVFXType.SMOKE));
        }

        [Test]
        public void GetPrefabByType_UnmappedKey_ReturnsNull()
        {
            Assert.IsNull(_library.GetPrefabByType(TestVFXType.MISSING));
        }

        [Test]
        public void GetPrefabByType_EntryWithoutPrefab_ReturnsNull()
        {
            _library.vfxList.Add(new VFXNamePair { vfxType = (int)TestVFXType.MISSING });

            Assert.IsNull(_library.GetPrefabByType(TestVFXType.MISSING));
        }

        [Test]
        public void GetPrefabByType_DuplicateKeys_ReturnsFirstEntry()
        {
            _library.vfxList.Add(new VFXNamePair { vfxType = (int)TestVFXType.SPARK, vfxPrefab = _smoke });

            Assert.AreSame(_spark, _library.GetPrefabByType(TestVFXType.SPARK));
        }
    }
}
