using System.Text.RegularExpressions;

namespace AcademicPms.Api.Validation;

public static partial class ChannelRequestValidator
{
    public static bool ValidCode(string? code) =>
        !string.IsNullOrWhiteSpace(code)
        && code.Length <= 50
        && CodePattern().IsMatch(code);

    public static bool ValidName(string? name) =>
        !string.IsNullOrWhiteSpace(name)
        && name.Length <= 100;

    [GeneratedRegex("^[a-z0-9]+(?:-[a-z0-9]+)*$")]
    private static partial Regex CodePattern();
}
