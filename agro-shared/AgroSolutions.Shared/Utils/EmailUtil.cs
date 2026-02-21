using System.Text.RegularExpressions;

namespace AgroSolutions.Shared.Utils;

public static class EmailUtil
{
    // Regex compilada para maior performance caso seja chamada muitas vezes
    private static readonly Regex _emailRegex = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$", 
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Valida se a string fornecida tem um formato de e-mail válido.
    /// </summary>
    /// <param name="email">O e-mail a ser validado.</param>
    /// <returns>True se for válido, False caso contrário.</returns>
    public static bool IsValid(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return _emailRegex.IsMatch(email);
    }
}