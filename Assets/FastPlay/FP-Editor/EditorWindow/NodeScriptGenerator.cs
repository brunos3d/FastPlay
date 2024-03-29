﻿#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FastPlay.Editor {
	public class NodeScriptGenerator : EditorWindow {

		private const BindingFlags METHOD_BIND_FLAGS = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

		public const string VARIABLE_FORMAT = "public InputValue<#PARAMETERTYPE#> #PARAMETERNAME#;";
		public const string REGISTER_PORT_FORMAT = @"#PARAMETERNAME# = this.RegisterInputValue<#PARAMETERTYPE#>(""#PARAMETERDISPLAYNAME#"");";

		public const string VALUE_NODE_FORMAT = @"
using UnityEngine;
using FastPlay.Runtime;

[Icon(""#NODEICON#"")]
[Title(""#NODETITLE#"")]
[Subtitle(""#NODESUBTITLE#"")]
[Path(""Generated/#NODETYPE#/#NODEDISPLAYNAME#"")]
public class #NODENAME# : ValueNode<#RETURNTYPE#>, IRegisterPorts {
	
	//#VARIABLES#

	public void OnRegisterPorts() {
		//#REGISTERPORTS#
	}

	public override #RETURNTYPE# OnGetValue() {
		return #METHODINVOKE#;
	}
}";

		public string type_name;
		public string method_name;

		public string code;
		public Dictionary<string, string> code_list = new Dictionary<string, string>();

		public Vector2 scroll;

		[MenuItem("Tools/FastPlay/NodeScriptGenerator")]
		public static NodeScriptGenerator Init() {
			return GetWindow<NodeScriptGenerator>();
		}

		void OnGUI() {
			type_name = EditorGUILayout.TextField("Type:", type_name);
			method_name = EditorGUILayout.TextField("Method Name:", method_name);

			if (GUILayout.Button("Generate")) {
				if (!type_name.IsNullOrEmpty()) {
					GenerateScript();
				}
			}
			if (GUILayout.Button("Generate All")) {
				int id = 0;
				foreach (string text in code_list.Values) {
					string path = Application.dataPath + "/CSGenerated/";
					using (StreamWriter writer = new StreamWriter(path + code_list.Keys.ToList()[id] + "NSG.cs", true)) {
						writer.Write(text);
						writer.Close();
					}
					id++;
				}
				AssetDatabase.Refresh();
			}

			scroll = EditorGUILayout.BeginScrollView(scroll);
			int index = 0;
			foreach (string text in code_list.Values) {
				if (GUILayout.Button("Save as NodeScript")) {
					string path = EditorUtility.SaveFilePanelInProject("Save as NodeScript", code_list.Keys.ToList()[index], "cs", "Export the reflected method to script (will make execution better).");
					StreamWriter writer = new StreamWriter(path, true);
					writer.Write(text);
					writer.Close();
					AssetDatabase.Refresh();
				}
				EditorGUILayout.TextArea(text);
				index++;
			}
			EditorGUILayout.EndScrollView();
		}

		void GenerateScript() {
			code_list = new Dictionary<string, string>();
			Type base_type = ReflectionUtils.GetTypeByName(type_name);

			foreach (MethodInfo method in base_type.GetMethods(METHOD_BIND_FLAGS)) {
				if (method_name.IsNullOrEmpty() || method.Name.ToLower().Contains(method_name.ToLower()) || method.Name.ToLower() == method_name.ToLower()) {
					bool is_value_node = method.ReturnType != typeof(void);
					string node_type = "NODETYPE";
					string node_name = "NODENAME";
					string icon = GUIReferrer.GetTypeIconName(base_type);
					string title = "NODETITLE";
					string subtitle = "NODESUBTITLE";
					string display_name = "NODESUBTITLE";
					string method_invoke = "//METHOD NOT IMPLEMENTED";

					string variables = "//#VARIABLES#";
					string ports = "//#REGISTERPORTS#";

					if (is_value_node) {
						code = VALUE_NODE_FORMAT;

						node_type = base_type.GetTypeName();
						if (base_type.GetGenericArguments().Length > 0) {
							node_name = string.Format("{0}{1}<{2}>", node_type, method.Name.NicifyPropertyName(), string.Join(", ", base_type.GetGenericArguments().Select(a => a.GetTypeName(true, true)).ToArray()));
						}
						else {
							node_name = string.Format("{0}{1}", node_type, method.Name.NicifyPropertyName());
						}
						code = code.Replace("#RETURNTYPE#", method.ReturnType.GetTypeName(true, true));

						ParameterInfo[] parameters = method.GetParameters();
						string[] param_names = new string[parameters.Length];
						if (parameters.Length > 0) {
							int id = 0;
							foreach (ParameterInfo parameter in parameters) {
								param_names[id++] = parameter.Name + ".value";
								string variable = VARIABLE_FORMAT.Replace("#PARAMETERNAME#", parameter.Name);
								variable = variable.Replace("#PARAMETERTYPE#", parameter.ParameterType.GetTypeName(true, true));
								variables += Environment.NewLine + "\t" + variable;

								string port = REGISTER_PORT_FORMAT.Replace("#PARAMETERNAME#", parameter.Name);
								port = port.Replace("#PARAMETERDISPLAYNAME#", parameter.Name.AddSpacesToSentence());
								port = port.Replace("#PARAMETERTYPE#", parameter.ParameterType.GetTypeName(true, true));
								ports += Environment.NewLine + "\t\t" + port;
							}
						}

						title = method.Name;
						subtitle = node_type;
						display_name = method.GetSignName();
						if (method.IsStatic) {
							if (method.IsSpecialName) {
								method_invoke = string.Format("{0}.{1}", method.ReflectedType.GetTypeName(true), method.Name);
								method_invoke = method_invoke.NicifyPropertyName();
							}
							else {
								method_invoke = string.Format("{0}.{1}({2})", method.ReflectedType.GetTypeName(true), method.Name, string.Join(",", param_names));
							}
						}
						else {
							string target = VARIABLE_FORMAT.Replace("#PARAMETERNAME#", "m_target");
							target = target.Replace("#PARAMETERTYPE#", base_type.GetTypeName(true, true));
							variables += Environment.NewLine + "\t" + target;

							string port = REGISTER_PORT_FORMAT.Replace("#PARAMETERNAME#", "this.m_target");
							port = port.Replace("#PARAMETERDISPLAYNAME#", "target");
							port = port.Replace("#PARAMETERTYPE#", base_type.GetTypeName(true, true));
							ports += Environment.NewLine + "\t\t" + port;

							if (method.IsSpecialName) {
								method_invoke = string.Format("{0}.{1}", "this.m_target.value", method.Name);
								method_invoke = method_invoke.Replace("get_", string.Empty);
							}
							else {
								method_invoke = string.Format("{0}.{1}({2})", "this.m_target.value", method.Name, string.Join(",", param_names));
							}
						}
					}

					code = code.Replace("#NODETYPE#", node_type);
					code = code.Replace("#NODENAME#", node_name + "NSG");
					code = code.Replace("#NODEDISPLAYNAME#", display_name);
					code = code.Replace("#NODEICON#", icon);
					code = code.Replace("#NODETITLE#", title);
					code = code.Replace("#NODESUBTITLE#", subtitle);

					code = code.Replace("//#VARIABLES#", variables);
					code = code.Replace("//#REGISTERPORTS#", ports);

					code = code.Replace("#METHODINVOKE#", method_invoke);

					code_list[node_name] = code;
				}
			}
		}
	}
}
#endif
