using System.Reflection;

namespace Fuse8.BackendInternship.Domain;

public static class AssemblyHelpers
{
	private static readonly string _sysAssName = "System";
	/// <summary>
	/// Получает информацию о базовых типах классов из namespace "Fuse8.BackendInternship.Domain", у которых есть наследники.
	/// </summary>
	/// <remarks>
	///	Информация возвращается только по самым базовым классам.
	/// Информация о промежуточных базовых классах не возвращается
	/// </remarks>
	/// <returns>Список типов с количеством наследников</returns>
	public static (string BaseTypeName, int InheritorCount)[] GetTypesWithInheritors()
	{
		
		// Получаем все классы из текущей Assembly
		var assemblyClassTypes = Assembly.GetAssembly(typeof(AssemblyHelpers))
			!.DefinedTypes
			.Where(p => p.IsClass);
		var dic = new Dictionary<Type, HashSet<Type>>();
		foreach (var assemblyType in assemblyClassTypes)
		{
			var tempType = assemblyType.AsType();
			var root = GetBaseType(tempType);
			if(root != null)
			{
				if (root.AssemblyQualifiedName!.StartsWith(_sysAssName))
				{
					continue;
				}
				if (!dic.ContainsKey(root))
				{
					dic.Add(root, new());
				}
				dic[root].Add(tempType);
			}
		}
		foreach (var (baseClass, childClasses) in dic)
		{
			HashSet<Type> toRemove = new HashSet<Type>();
			foreach (var childClass in childClasses)
			{
				if (childClasses.Contains(childClass.BaseType!))
				{
					toRemove.Add(childClass.BaseType!);
				}
			}
			foreach (var type in toRemove)
			{
				dic[baseClass].Remove(type);
			}
		}
		return dic.Select(x=>(x.Key.Name, x.Value.Count)).ToArray();
		// ToDo: Добавить реализацию
	}

	/// <summary>
	/// Получает базовый тип для класса
	/// </summary>
	/// <param name="type">Тип, для которого необходимо получить базовый тип</param>
	/// <returns>
	/// Первый тип в цепочке наследований. Если наследования нет, возвращает null
	/// </returns>
	/// <example>
	/// Класс A, наследуется от B, B наследуется от C
	/// При вызове GetBaseType(typeof(A)) вернется C
	/// При вызове GetBaseType(typeof(B)) вернется C
	/// При вызове GetBaseType(typeof(C)) вернется C
	/// </example>
	private static Type? GetBaseType(Type type)
	{
		var baseType = type;

		while (baseType.BaseType is not null && baseType.BaseType != typeof(object))
		{
			baseType = baseType.BaseType;
		}

		return baseType == type
			? null
			: baseType;
	}
}