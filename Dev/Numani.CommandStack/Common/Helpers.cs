using System;
using System.Linq;

namespace Numani.CommandStack.Common;

internal static class Helpers
{
    public static string ParameterizedName(this Type type)
    {
        var arguments1 = type.GenericTypeArguments
            .Select(ParameterizedName)
            .ToArray();

        return type.Name + (arguments1.Any()
            ? $"[{string.Join(',', arguments1)}]"
            : "");
    }
    
    public static string Indent(this string lines, int level)
    {
        var map = lines.Split(Environment.NewLine)
            .Select(x => string.Join("", Enumerable.Repeat("\t", level)) + x);
        return string.Join(Environment.NewLine, map);
    }
}