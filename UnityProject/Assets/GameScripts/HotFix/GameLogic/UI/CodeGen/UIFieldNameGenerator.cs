using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.UI.CodeGen
{
    public static class UIFieldNameGenerator
    {
        private const int DuplicateIndexStart = 1;
        private const char Underscore = '_';

        public static string GenerateFieldName(string objectName, string componentTypeName, ISet<string> existing)
        {
            if (existing == null)
            {
                throw new ArgumentNullException(nameof(existing));
            }

            if (string.IsNullOrWhiteSpace(componentTypeName))
            {
                throw new ArgumentException("Component type name is required.", nameof(componentTypeName));
            }

            var baseName = string.IsNullOrWhiteSpace(objectName) ? componentTypeName : objectName;
            var sanitized = SanitizeIdentifier(baseName);
            if (string.IsNullOrWhiteSpace(sanitized))
            {
                sanitized = SanitizeIdentifier(componentTypeName);
            }

            var camelCase = ToCamelCase(sanitized);
            var candidate = EnsureValidStart(camelCase);
            candidate = EnsureUnique(candidate, existing);
            existing.Add(candidate);
            return candidate;
        }

        private static string SanitizeIdentifier(string value)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (char.IsLetterOrDigit(c) || c == Underscore)
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }

        private static string ToCamelCase(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            if (value.Length == 1)
            {
                return value.ToLowerInvariant();
            }

            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }

        private static string EnsureValidStart(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            if (char.IsDigit(value[0]))
            {
                return Underscore + value;
            }

            return value;
        }

        private static string EnsureUnique(string candidate, ISet<string> existing)
        {
            if (!existing.Contains(candidate))
            {
                return candidate;
            }

            var index = DuplicateIndexStart;
            var baseName = candidate;
            var current = baseName + index;
            while (existing.Contains(current))
            {
                index++;
                current = baseName + index;
            }

            return current;
        }
    }
}
