using System;
using System.Linq;

namespace Numani.CommandStack
{
    public static class Helpers
    {
        public static string ParameterizedName(this Type type)
        {
            var arguments = type.GenericTypeArguments.Select(x => x.Name).ToArray();
            return arguments.Any()
                ? type.Name + $"[{string.Join(',', arguments)}]"
                : type.Name;
        }
    }
}