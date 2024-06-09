using System.Text.RegularExpressions;

namespace Chipseky.MamkinInvestor.WebApi.Extensions;

public static class StringExtensions
{
    public static string ToSnakeCase(this string source) => 
        Regex.Replace(source, "([a-z])([A-Z])", "$1_$2").ToLower();
}