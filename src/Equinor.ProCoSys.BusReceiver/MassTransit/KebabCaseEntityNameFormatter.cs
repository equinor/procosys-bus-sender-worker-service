using System.Text.RegularExpressions;
using MassTransit;

namespace Equinor.ProCoSys.BusReceiver.MassTransit;


public class KebabCaseEntityNameFormatter : IEntityNameFormatter
{
    private readonly bool _includeNamespace;

    public KebabCaseEntityNameFormatter(bool includeNamespace) 
        => _includeNamespace = includeNamespace;

    public string FormatEntityName<T>()
    {
        var name = typeof(T).Name;
        return _includeNamespace ? name.ToKebabCase() : name[(name.LastIndexOf('.') + 1)..].ToKebabCase();
    }
}

public static class StringExtensions
{
    public static string ToKebabCase(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        return Regex.Replace(
            str,
            "([a-z0-9])([A-Z])",
            "$1-$2",
            RegexOptions.CultureInvariant,
            TimeSpan.FromMilliseconds(100)).ToLowerInvariant();
    }
}
