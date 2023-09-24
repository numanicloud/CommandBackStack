using System;
using System.Linq;

namespace Numani.CommandStack.Common;

public static class Helpers
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
}