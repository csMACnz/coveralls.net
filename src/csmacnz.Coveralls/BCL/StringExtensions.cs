using System.Diagnostics.CodeAnalysis;

namespace csmacnz.Coveralls
{
    public static class StringExtensions
    {
        public static bool IsNotNullOrWhitespace([NotNullWhen(true)]this string? input)
        {
            return !string.IsNullOrWhiteSpace(input);
        }

        public static bool IsNullOrWhitespace([NotNullWhen(false)]this string? input)
        {
            return string.IsNullOrWhiteSpace(input);
        }
    }
}
