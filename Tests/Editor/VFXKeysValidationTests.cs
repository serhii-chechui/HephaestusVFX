using NUnit.Framework;

namespace WTFGames.Hephaestus.VFX.Editor.Tests
{
    public class VFXKeysValidationTests
    {
        [Test]
        public void GetNewKeyError_ValidUniqueKey_ReturnsNull()
        {
            Assert.IsNull(VFXKeysValidation.GetNewKeyError("SMOKE", new[] { "EXPLOSION" }, 1));
        }

        [TestCase("")]
        [TestCase("1FX")]
        [TestCase("HIT-SPARKS")]
        public void GetNewKeyError_InvalidIdentifier_ReturnsError(string key)
        {
            StringAssert.StartsWith("Invalid key", VFXKeysValidation.GetNewKeyError(key, new string[0], 0));
        }

        [Test]
        public void GetNewKeyError_ExistingKey_ReturnsError()
        {
            StringAssert.Contains("already exists", VFXKeysValidation.GetNewKeyError("EXPLOSION", new[] { "EXPLOSION" }, 1));
        }

        [Test]
        public void GetNewKeyError_LastByteId_IsAllowed()
        {
            Assert.IsNull(VFXKeysValidation.GetNewKeyError("SMOKE", new string[0], VFXKeysValidation.MaxKeyId));
        }

        [Test]
        public void GetNewKeyError_NoIdsLeft_ReturnsError()
        {
            StringAssert.Contains("No free ids", VFXKeysValidation.GetNewKeyError("SMOKE", new string[0], VFXKeysValidation.MaxKeyId + 1));
        }

        [Test]
        public void GetKeysErrors_ValidKeys_ReturnsNoErrors()
        {
            CollectionAssert.IsEmpty(VFXKeysValidation.GetKeysErrors(new[] { "EXPLOSION", "SMOKE" }));
        }

        [Test]
        public void GetKeysErrors_EmptyList_ReturnsNoErrors()
        {
            CollectionAssert.IsEmpty(VFXKeysValidation.GetKeysErrors(new string[0]));
        }

        [Test]
        public void GetKeysErrors_ReportsEachInvalidKey()
        {
            var errors = VFXKeysValidation.GetKeysErrors(new[] { "EXPLOSION", "", "1FX" });

            Assert.AreEqual(2, errors.Count);
            StringAssert.Contains("\"\"", errors[0]);
            StringAssert.Contains("\"1FX\"", errors[1]);
        }

        [Test]
        public void GetKeysErrors_ReportsDuplicateOnce()
        {
            var errors = VFXKeysValidation.GetKeysErrors(new[] { "SMOKE", "SMOKE", "SMOKE" });

            Assert.AreEqual(1, errors.Count);
            StringAssert.Contains("Duplicate key SMOKE", errors[0]);
        }
    }
}
