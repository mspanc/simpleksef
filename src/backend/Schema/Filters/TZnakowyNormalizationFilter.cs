using System.Collections;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Filters;
using SimpleKsef.Schema.Types;
namespace SimpleKsef.Schema.Filters;

public sealed class TZnakowyNormalizationFilter : IAsyncActionFilter
{
    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);

        foreach (var arg in context.ActionArguments.Values)
        {
            NormalizeObjectGraph(arg, visited);
        }

        return next();
    }

    private static void NormalizeObjectGraph(object? obj, HashSet<object> visited)
    {
        if (obj is null)
        {
            return;
        }

        var type = obj.GetType();

        // Treat string as terminal
        if (type == typeof(string))
        {
            return;
        }

        // Prevent cycles
        if (!type.IsValueType)
        {
            if (!visited.Add(obj))
            {
                return;
            }
        }

        // Collections
        if (obj is IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                NormalizeObjectGraph(item, visited);
            }

            return;
        }

        // Normalize properties annotated with TZnakowyBaseAttribute (or subclasses)
        foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!prop.CanRead || !prop.CanWrite)
            {
                continue;
            }

            if (prop.GetIndexParameters().Length != 0)
            {
                continue;
            }

            var tokenAttr = prop.GetCustomAttribute<TZnakowyBaseAttribute>(inherit: true);
            if (tokenAttr is not null)
            {
                var current = (string?)prop.GetValue(obj);

                // If null is allowed (min=0), leave as null; otherwise validation will fail later.
                if (current is null)
                {
                    continue;
                }

                var normalized = TZnakowyBaseAttribute.NormalizeToken(current);
                prop.SetValue(obj, normalized);
                continue;
            }

            // Recurse into nested complex types / collections
            var value = prop.GetValue(obj);
            NormalizeObjectGraph(value, visited);
        }
    }

    private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        public static ReferenceEqualityComparer Instance { get; } = new();

        public new bool Equals(object? x, object? y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }
    }
}