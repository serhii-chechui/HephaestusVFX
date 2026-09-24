using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace WTFGames.Hephaestus.VFX.Tests
{
    public class VFXLibraryConstantsTests
    {
        private VFXLibraryConstants _constants;

        [SetUp]
        public void SetUp()
        {
            _constants = ScriptableObject.CreateInstance<VFXLibraryConstants>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_constants);
        }

        [Test]
        public void LegacyKeys_AreMigratedWithIndexIds()
        {
            // JsonUtility runs the ISerializationCallbackReceiver callbacks like asset loading does.
            JsonUtility.FromJsonOverwrite("{\"uiMapKeys\":[\"EXPLOSION\",\"HIT\",\"SMOKE\"]}", _constants);

            CollectionAssert.AreEqual(new[] { "EXPLOSION", "HIT", "SMOKE" }, _constants.keys.Select(key => key.name));
            CollectionAssert.AreEqual(new[] { 0, 1, 2 }, _constants.keys.Select(key => key.id));
            Assert.AreEqual(3, _constants.nextKeyId);
        }

        [Test]
        public void LegacyKeys_AreClearedAfterMigration()
        {
            JsonUtility.FromJsonOverwrite("{\"uiMapKeys\":[\"EXPLOSION\"]}", _constants);

            StringAssert.Contains("\"uiMapKeys\":[]", JsonUtility.ToJson(_constants));
        }

        [Test]
        public void LegacyKeys_AreIgnoredWhenKeysExist()
        {
            JsonUtility.FromJsonOverwrite("{\"keys\":[{\"name\":\"SPARK\",\"id\":7}],\"uiMapKeys\":[\"EXPLOSION\"]}", _constants);

            CollectionAssert.AreEqual(new[] { "SPARK" }, _constants.keys.Select(key => key.name));
            Assert.AreEqual(8, _constants.nextKeyId);
        }

        [Test]
        public void NextKeyId_IsNeverBelowHighestIdPlusOne()
        {
            JsonUtility.FromJsonOverwrite("{\"keys\":[{\"name\":\"SPARK\",\"id\":10}],\"nextKeyId\":3}", _constants);

            Assert.AreEqual(11, _constants.nextKeyId);
        }

        [Test]
        public void NextKeyId_KeepsHigherStoredValue()
        {
            // Ids of removed keys must not be handed out again.
            JsonUtility.FromJsonOverwrite("{\"keys\":[{\"name\":\"SPARK\",\"id\":1}],\"nextKeyId\":20}", _constants);

            Assert.AreEqual(20, _constants.nextKeyId);
        }
    }
}
