using NUnit.Framework;

namespace WTFGames.Hephaestus.VFX.Editor.Tests
{
    public class VFXIdentifierUtilityTests
    {
        [TestCase("hit sparks", "HIT_SPARKS")]
        [TestCase("Explosion", "EXPLOSION")]
        [TestCase("fx_1", "FX_1")]
        [TestCase("", "")]
        [TestCase(null, "")]
        public void ToKey_ConvertsInputToKeyFormat(string input, string expected)
        {
            Assert.AreEqual(expected, VFXIdentifierUtility.ToKey(input));
        }

        [Test]
        public void ToKey_IgnoresCurrentCulture()
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;

            try
            {
                // In the Turkish culture "i".ToUpper() is "İ", which is not a valid identifier character.
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("tr-TR");

                Assert.AreEqual("HIT", VFXIdentifierUtility.ToKey("hit"));
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            }
        }

        [TestCase("EXPLOSION", true)]
        [TestCase("_PRIVATE", true)]
        [TestCase("FX_1", true)]
        [TestCase("1FX", false)]
        [TestCase("HIT-SPARKS", false)]
        [TestCase("HIT SPARKS", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        public void IsIdentifier_AcceptsOnlyValidIdentifiers(string value, bool expected)
        {
            Assert.AreEqual(expected, VFXIdentifierUtility.IsIdentifier(value));
        }

        [TestCase("WTFGames", "WTFGames")]
        [TestCase("My Cool-Game 2", "MyCoolGame2")]
        [TestCase("3D Arena", "_3DArena")]
        [TestCase("!!!", "")]
        [TestCase(null, "")]
        public void ToNamespacePart_StripsInvalidCharacters(string input, string expected)
        {
            Assert.AreEqual(expected, VFXIdentifierUtility.ToNamespacePart(input));
        }
    }
}
