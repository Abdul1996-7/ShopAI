using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ShopAI.Helpers;

public static partial class SlugHelper
{
    public static string GenerateSlug(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Guid.NewGuid().ToString("N")[..8];
        }

        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);
            if (category != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(char.ToLowerInvariant(character));
            }
        }

        var cleaned = NonSlugCharactersRegex().Replace(builder.ToString(), "-");
        cleaned = DuplicateDashesRegex().Replace(cleaned, "-").Trim('-');

        return string.IsNullOrWhiteSpace(cleaned)
            ? Guid.NewGuid().ToString("N")[..8]
            : cleaned;
    }

    [GeneratedRegex("[^a-z0-9]+")]
    private static partial Regex NonSlugCharactersRegex();

    [GeneratedRegex("-{2,}")]
    private static partial Regex DuplicateDashesRegex();
}
