using System.Reflection;

namespace SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers.Extensions;

public static class ReflectionExtensions
{
    /// <summary>
    /// Get all classes in the assemblies that have a method with a parameter with the specified attribute
    /// </summary>
    public static IEnumerable<Type> GetClassesWithMethodParameterAttribute<TAttribute>(this IEnumerable<Assembly> assemblies)
    {
        return assemblies.SelectMany(assembly => assembly.GetTypes())
            .Where(type => type
                .GetMethods()
                .Any(method => method.GetParameters()
                .Any(parameter => parameter.GetCustomAttributes(typeof(TAttribute), false)
                .Any())));
    }

    public static MethodInfo GetMethodWithParameterAttribute<TAttribute>(this Type type)
    {
        return type.GetMethods()
            .FirstOrDefault(method => method.GetParameters()
            .Any(parameter => parameter.GetCustomAttributes(typeof(TAttribute), false)
            .Any()))!;
    }
}
