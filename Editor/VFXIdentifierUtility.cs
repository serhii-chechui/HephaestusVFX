using System.Text.RegularExpressions;

namespace WTFGames.Hephaestus.VFX.Editor
{
    /// <summary>
    /// Helpers that keep generated enum code compilable.
    /// </summary>
    internal static class VFXIdentifierUtility
    {
        private static readonly Regex IdentifierRegex = new Regex("^[A-Za-z_][A-Za-z0-9_]*$");

        private static readonly Regex InvalidCharactersRegex = new Regex("[^A-Za-z0-9_]");

        /// <summary>
        /// Converts user input to the key format: spaces become underscores, letters become upper case.
        /// </summary>
        public static string ToKey(string input)
        {
            return (input ?? string.Empty).Replace(' ', '_').ToUpperInvariant();
        }

        public static bool IsIdentifier(string value)
        {
            return !string.IsNullOrEmpty(value) && IdentifierRegex.IsMatch(value);
        }

        /// <summary>
        /// Strips characters that are not allowed in a namespace part, e.g. spaces in the product name.
        /// </summary>
        public static string ToNamespacePart(string input)
        {
            var part = InvalidCharactersRegex.Replace(input ?? string.Empty, string.Empty);
            return part.Length > 0 && char.IsDigit(part[0]) ? "_" + part : part;
        }
    }
}
