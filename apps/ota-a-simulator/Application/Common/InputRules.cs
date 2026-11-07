namespace OtaASimulator.Application.Common;

public static class InputRules
{
    public static void Unique<T, TKey>(IReadOnlyList<T> items, Func<T, TKey> key, string field)
    {
        if (items.Any(x => x is null)) throw ApiException.Invalid("La colección no admite elementos null.");
        if (items.Select(key).Distinct().Count() != items.Count)
            throw ApiException.Invalid($"La colección contiene valores duplicados de {field}.");
    }
}
