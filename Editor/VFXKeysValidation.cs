using System.Collections.Generic;
using System.Linq;

namespace WTFGames.Hephaestus.VFX.Editor
{
    /// <summary>
    /// Checks VFX keys before they are added or exported to the enum.
    /// </summary>
    internal static class VFXKeysValidation
    {
        // The generated enum is backed by byte.
        public const int MaxKeyId = byte.MaxValue;

        private const string IdentifierRule = "it must start with a letter or underscore and contain only letters, digits and underscores";

        /// <summary>
        /// Returns why the key can't be added, or null when it can.
        /// </summary>
        public static string GetNewKeyError(string key, ICollection<string> existingKeys, int nextKeyId)
        {
            if (!VFXIdentifierUtility.IsIdentifier(key))
            {
                return $"Invalid key: {IdentifierRule}.";
            }

            if (existingKeys.Contains(key))
            {
                return $"The key {key} already exists.";
            }

            if (nextKeyId > MaxKeyId)
            {
                return $"No free ids left: the enum is backed by byte and supports ids up to {MaxKeyId}.";
            }

            return null;
        }

        /// <summary>
        /// Returns the problems that keep the keys from being exported to the enum.
        /// </summary>
        public static List<string> GetKeysErrors(IList<string> keys)
        {
            var errors = new List<string>();

            foreach (var key in keys.Where(key => !VFXIdentifierUtility.IsIdentifier(key)))
            {
                errors.Add($"Invalid key \"{key}\": {IdentifierRule}.");
            }

            foreach (var key in keys.GroupBy(key => key).Where(group => group.Count() > 1).Select(group => group.Key))
            {
                errors.Add($"Duplicate key {key}.");
            }

            return errors;
        }
    }
}
