using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using FastPlay.Runtime;

namespace FastPlay {
	public static class ReflectionUtils {

		private static IEnumerable<Assembly> cache_assemblies;

		private static Dictionary<string, ConverterFlagAtribute> cache_converter_flags = new Dictionary<string, ConverterFlagAtribute>();

		private static Dictionary<string, IValueConverter> cache_obj_converters = new Dictionary<string, IValueConverter>();

		private static Dictionary<string, Type> cache_types = new Dictionary<string, Type>();

		private static Dictionary<string, MethodInfo> cache_methods = new Dictionary<string, MethodInfo>();

		private static Dictionary<string, string> cache_type_names = new Dictionary<string, string>();

		public static Dictionary<MemberInfo, string> cached_summary = new Dictionary<MemberInfo, string>();

		private static readonly Dictionary<Type, string> keywords = new Dictionary<Type, string>() {
			{ typeof(int), "int" },
			{ typeof(bool), "bool" },
			{ typeof(float), "float" },
			{ typeof(string), "string" },
			{ typeof(object), "object" },
			{ typeof(byte), "byte" },
			{ typeof(char), "char" },
			{ typeof(decimal), "decimal" },
			{ typeof(double), "double" },
			{ typeof(long), "long" },
			{ typeof(sbyte), "sbyte" },
			{ typeof(short), "short" },
			{ typeof(uint), "uint" },
			{ typeof(ulong), "ulong" },
			{ typeof(ushort), "ushort" },
			{ typeof(void), "void" },
	};


		public static bool CanConvert<T>(Type from) {
			return CanConvert(from, typeof(T));
		}

		public static bool CanConvert(this Type from, Type to) {
			IValueConverter vc = GetConverter(from, to);
			if (vc != null) {
				return vc.CanConvert(from, to);
			}
			return false;
		}

		public static IValueConverter<T> GetConverter<T>(Type from) {
			return (IValueConverter<T>)GetConverter(from, typeof(T));
		}

		public static IValueConverter GetConverter(this Type from, Type to) {
			string key = from.FullName + to.FullName;
			IValueConverter vc;
			if (cache_obj_converters.TryGetValue(key, out vc)) {
				return vc;
			}
			ConverterFlagAtribute cf;
			if (cache_converter_flags.TryGetValue(key, out cf)) {
				return cache_obj_converters[key] = (IValueConverter)Activator.CreateInstance(cf.base_type);
			}
			foreach (ConverterFlagAtribute flag in YieldGetAllConverterFlags()) {
				if (flag.CanConvert(from, to)) {
					return cache_obj_converters[flag.from.FullName + flag.to.FullName] = (IValueConverter)Activator.CreateInstance(flag.base_type);
				}
			}
			return null;
		}

		public static IEnumerable<ConverterFlagAtribute> YieldGetAllConverterFlags() {
			foreach (Type type in YieldGetFullTypes()) {
				ConverterFlagAtribute flag = type.GetAttribute<ConverterFlagAtribute>(false);
				if (flag != null) {
					flag.base_type = type;
					yield return cache_converter_flags[flag.from.FullName + flag.to.FullName] = flag;
				}
			}
		}

		public static IEnumerable<ConverterFlagAtribute> GetAllConverterFlags() {
			foreach (Type type in GetFullTypes()) {
				ConverterFlagAtribute flag = type.GetAttribute<ConverterFlagAtribute>(false);
				if (flag != null) {
					flag.base_type = type;
					yield return cache_converter_flags[flag.from.FullName + flag.to.FullName] = flag;
				}
			}
		}

		/// <summary>
		/// Returns all types of all current assemblies
		/// </summary>
		public static IEnumerable<Type> YieldGetFullTypes() {
			foreach (Assembly assembly in YieldGetFullAssemblies()) {
				foreach (Type type in assembly.GetTypes()) {
					if (type != null && type.IsPublic) {
						yield return cache_types[type.GetTypeName(true)] = type;
					}
				}
			}
		}

		/// <summary>
		/// Returns all types of all current assemblies
		/// </summary>
		public static List<Type> GetFullTypes() {
			List<Type> types = new List<Type>();
			if (cache_types.IsNullOrEmpty()) {
				cache_types = new Dictionary<string, Type>();
				foreach (Assembly assembly in GetFullAssemblies()) {
					foreach (Type type in assembly.GetTypes()) {
						if (type != null && type.IsPublic) {
							cache_types[type.GetTypeName(true)] = type;
						}
					}
				}
				types = cache_types.Values.ToList();
				types.Sort((t1, t2) => string.Compare((t1.FullName ?? t1.Name), (t2.FullName ?? t2.Name)));
				return types;
			}
			else {
				types = cache_types.Values.ToList();
				return types;
			}
		}

		/// <summary>
		/// Returns all current assemblies
		/// </summary>
		public static IEnumerable<Assembly> YieldGetFullAssemblies() {
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				if (cache_assemblies.IsNullOrEmpty() || !cache_assemblies.Contains(assembly)) {
					cache_assemblies = cache_assemblies.AddItem(assembly);
				}
				yield return assembly;
			}
		}

		/// <summary>
		/// Returns all current assemblies
		/// </summary>
		public static IEnumerable<Assembly> GetFullAssemblies() {
			if (cache_assemblies.IsNullOrEmpty()) {
				cache_assemblies = AppDomain.CurrentDomain.GetAssemblies();
			}
			return cache_assemblies;
		}

		/// <summary>
		/// Returns types in a namespace
		/// </summary>
		public static IEnumerable<Type> GetTypesWithNamespace(string @namespace) {
			return GetFullTypes().Where(t => t.Namespace == @namespace);
		}

		/// <summary>
		/// Find the method from your serializable name
		/// </summary>
		public static MethodInfo GetMethodInfoByKey(Type type, string key) {
			if (type == null || key.IsNullOrEmpty()) return null;
			MethodInfo m1;
			if (cache_methods.TryGetValue(key, out m1)) {
				return m1;
			}
			return type.GetMethods().FirstOrDefault(m2 => GetMethodInfoKey(m2) == key);
		}

		/// <summary>
		/// Returns a serializable name for the method
		/// </summary>
		public static string GetMethodInfoKey(MethodInfo method) {
			string key = method.ReturnType.GetTypeName(true);
			key += method.Name;
			string args = string.Join("/", method.GetGenericArguments().Select(m => m.GetTypeName(true)).ToArray());
			key += args;
			string parameters = string.Join(",", method.GetParameters().Select(p => p.ParameterType.GetTypeName(true)).ToArray());
			key += parameters;
			return key;
		}

		/// <summary>
		/// Search for a type from your name
		/// </summary>
		public static Type GetTypeByName(string type_name) {
			Type t1 = Type.GetType(type_name);
			if (t1 != null) {
				return t1;
			}
			Type t2;
			if (cache_types.TryGetValue(type_name, out t2)) {
				return t2;
			}
			foreach (Type type in YieldGetFullTypes()) {
				if (type.Name == type_name || type.FullName == type_name || type.GetTypeName(true) == type_name || type.GetTypeName(false) == type_name) {
					return type;
				}
				if (type.Name.Contains("+")) {
					string[] split = type.Name.Split('+');
					return GetTypeByName(split[0]).GetNestedType(split[1]);
				}
				if (type.FullName.Contains("+")) {
					string[] split = type.FullName.Split('+');
					return GetTypeByName(split[0]).GetNestedType(split[1]);
				}
				if (type.GetTypeName(true).Contains("+")) {
					string[] split = type.GetTypeName(true).Split('+');
					return GetTypeByName(split[0]).GetNestedType(split[1]);
				}
				if (type.GetTypeName(false).Contains("+")) {
					string[] split = type.GetTypeName(false).Split('+');
					return GetTypeByName(split[0]).GetNestedType(split[1]);
				}
			}
			return null;
		}

		// Extensions

		public static Type[] TryGetGenericParameterConstraints(this Type type) {
			try {
				if (type.IsGenericType) {
					return type.GetGenericParameterConstraints();
				}
			}
			catch {
				return null;
			}
			return null;
		}

		//T = string
		//from_type = float
		public static bool CanBeCastTo<T>(this Type from_type) {
			object obj = Activator.CreateInstance(from_type);
			try {
				UnityEngine.Debug.Log("pass 1");
				if (obj is T) {
					return true;
				}
				UnityEngine.Debug.Log("pass 2");
				return true;
			}
			catch {
				UnityEngine.Debug.Log("pass 3");
				return typeof(T).IsAssignableFrom(from_type);
			}
		}

		/// <summary>
		/// Extension to check if a type is static
		/// </summary>
		public static bool IsStatic(this Type type) {
			return type.IsAbstract && type.IsSealed;
		}

		/// <summary>
		/// Returns the first attribute of type "T" of the member or inherited if "inherit" is true
		/// </summary>
		public static T GetAttribute<T>(this MemberInfo member, bool inherit) where T : Attribute {
			object[] attributes = member.GetCustomAttributes(typeof(T), inherit);
			if (attributes.Length > 0) {
				return (T)attributes[0];
			}
			return null;
		}

		/// <summary>
		/// Checks whether there is an attribute on a member
		/// </summary>
		public static bool HasAttribute<T>(this MemberInfo member, bool inherit) where T : Attribute {
			return GetAttribute<T>(member, inherit) != null;
		}

		/// <summary>
		/// Returns the signature (friendly name) of the method that sets it apart from others
		/// </summary>
		public static string GetSignName(this MethodInfo method, bool full_path = false, bool full_name = false) {
			if (method == null) return null;
			string p_name;
			string[] parameters = method.GetParameters().Select(p => (p_name = p.ParameterType.GetTypeName(full_name)).Contains("&") ? string.Format("ref {0}", p_name.Replace("&", string.Empty)) : p_name).ToArray();
			string full_args = string.Empty;
			if (!parameters.IsNullOrEmpty()) {
				full_args = string.Join(", ", parameters);
			}
			if (full_path) {
				return string.Format("{0}.{1}({2}) : {3}", method.DeclaringType.GetTypeName(full_name), method.Name, full_args, method.ReturnType.GetTypeName(full_name));
			}
			else {
				return string.Format("{0}({1}) : {2}", method.Name, full_args, method.ReturnType.GetTypeName(full_name));
			}
		}

		/// <summary>
		/// Returns the path of the type as a directory
		/// </summary>
		public static string GetTypePath(this Type type, bool standard_types_prevail = false) {
			string namespace_path = type.Namespace;
			if (namespace_path.IsNullOrEmpty()) {
				namespace_path = "Global";
			}
			else {
				namespace_path = namespace_path.Replace(".", "/");
			}
			return string.Format("{0}/{1}", namespace_path, type.GetTypeName(false, standard_types_prevail));
		}

		/// <summary>
		/// Friendly name for types (ex: List'1 = List <T>)
		/// </summary>
		public static string GetTypeName(this Type type, bool full_name = false, bool standard_types_prevail = false) {
			if (type == null) return null;
			string s;
			if (keywords.TryGetValue(type, out s) && (!full_name || standard_types_prevail)) {
				return s.Replace("&", string.Empty);
			}
			if (cache_type_names.TryGetValue(string.Format("{0}, {1} & {2}", type, full_name, standard_types_prevail), out s)) {
				return s;
			}

			if (type.IsGenericType) {
				string value = type.Name;
				List<Type> generic_args = type.GetGenericArguments().ToList();

				if (value.IndexOf("`") > -1) {
					value = value.Substring(0, value.IndexOf("`"));
				}
				if (type.DeclaringType != null) {
					value = GetTypeName(type.DeclaringType) + "+" + value;
				}

				string arg_string = string.Empty;
				for (int i = 0; i < type.GetGenericArguments().Length && generic_args.Count > 0; i++) {
					if (i != 0) arg_string += ", ";

					arg_string += GetTypeName(generic_args[0]);
					generic_args.RemoveAt(0);
				}
				if (arg_string.Length > 0) {
					value += "<" + arg_string + ">";
				}
				return cache_type_names[string.Format("{0}, {1} & {2}", type, full_name, standard_types_prevail)] = value.Replace("&", string.Empty);
			}
			if ((full_name ? type.FullName : type.Name).IsNullOrEmpty()) {
				return string.Empty;
			}
			return cache_type_names[string.Format("{0}, {1} & {2}", type, full_name, standard_types_prevail)] = (full_name ? type.FullName : type.Name).Replace("&", string.Empty);
		}

		/// <summary>
		/// Make a generic instance based on "generic_type" using the "type_argument" argument
		/// </summary>
		public static object CreateGenericInstance(this Type generic_type, Type type_argument, params object[] constructor_args) {
			if (generic_type == null) return null;
			return Activator.CreateInstance(generic_type.MakeGenericType(type_argument), constructor_args);
		}

		/// <summary>
		/// Make a generic instance based on "generic_type" using the "type_args" arguments
		/// </summary>
		public static object CreateGenericInstance(this Type generic_type, Type[] type_args, params object[] constructor_args) {
			if (generic_type == null) return null;
			return Activator.CreateInstance(generic_type.MakeGenericType(type_args), constructor_args);
		}

		/// <summary>
		/// Return member description/summary
		/// </summary>
		public static string GetDescription(this MemberInfo member) {
			if (member == null) return string.Empty;
			string s;
			if (cached_summary.TryGetValue(member, out s)) {
				return s;
			}
			SummaryAttribute summary_flag = member.GetAttribute<SummaryAttribute>(false);
			if (summary_flag != null) {
				return cached_summary[member] = summary_flag.summary;
			}
			if (member is Type) {
				return cached_summary[member] = ((Type)member).GetTypeName(true);
			}
			if (member is MethodInfo) {
				return cached_summary[member] = ((MethodInfo)member).GetSignName(true);
			}
			return string.Empty;
		}
	}
}
