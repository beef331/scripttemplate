using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace ScriptTemplate
{
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	[AttributeUsage (AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public class Templated : Attribute
	{
#if UNITY_EDITOR
		private static string scriptPattern = @"(,\s?(ScriptTemplate\.)?Templated)|(\[\s?(ScriptTemplate\.)?Templated\s?\])|((ScriptTemplate\.)?Templated\s?,)";
		private static string namePattern = @"[""]?\{0\}[""]?";
		private static string templatePath = "/Assets/ScriptTemplator/Templates/";
		private static string csharpPath = "/Assets/ScriptTemplator/csharp/";
		private static string asmdefPath = "/Assets/ScriptTemplator/ScriptTemplator.asmdef";
		private static string format =
			@"using UnityEditor;
using UnityEngine;
namespace ScriptTemplator
{
public class {0}Template{
	[MenuItem(""Assets/Create/C# Template/{0}"""",false,10)]
	public static void CreateScript()
	{
		string file = Application.dataPath +@""/ScriptTemplator/Templates/{0}.cs.txt"";
		ProjectWindowUtil.CreateScriptAssetFromTemplateFile(file,""""{0}"".cs"");
	}
	}
}	
	";

		private static string asmdef =
			@"{
    ""name"": ""ScriptTemplator"",
    ""references"": [],
    ""includePlatforms"": [],
    ""excludePlatforms"": [
        ""Android"",
        ""iOS"",
        ""LinuxStandalone64"",
        ""Lumin"",
        ""macOSStandalone"",
        ""PS4"",
        ""Stadia"",
        ""Switch"",
        ""tvOS"",
        ""WSA"",
        ""WebGL"",
        ""WindowsStandalone32"",
        ""WindowsStandalone64"",
        ""XboxOne""
    ],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}";

		static Templated ()
		{
			HashSet<String> foundTypes = new HashSet<string> ();
			string root = Path.GetDirectoryName (Application.dataPath);
			templatePath = root + templatePath;
			csharpPath = root + csharpPath;
			Directory.CreateDirectory (templatePath);
			Directory.CreateDirectory (csharpPath);
			if (!AssetDatabase.LoadAssetAtPath (asmdefPath, typeof (UnityEditor.Compilation.Assembly)))
			{
				using (StreamWriter sw = new StreamWriter (File.Open (Path.GetFullPath (Path.Combine (root, asmdefPath.Substring(1))), FileMode.Create)))
				{
					sw.Write (asmdef);
				}
			}

			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies ();
			foreach (Assembly ass in assemblies)
			{
				foreach (Type t in ass.GetTypes ())
				{
					Attribute at = t.GetCustomAttribute (typeof (Templated));
					if (at != null)
					{
						string[] assets = AssetDatabase.FindAssets ($"{t.ToString()}");
						string asset = default;
						foreach (string guid in assets)
						{
							string path = AssetDatabase.GUIDToAssetPath (guid);
							string[] split = path.Split ('/');
							if (split[split.Length - 1] == t.ToString () + ".cs")
							{
								asset = path;
								break;
							}
						}
						if (!string.IsNullOrEmpty (asset))
						{
							foundTypes.Add (t.ToString ());
							string file = File.ReadAllText (Path.GetFullPath (Path.Combine (root, asset.Substring(0))));
							file = Regex.Replace (file, scriptPattern, "");
							file = file.Replace (t.ToString (), "#SCRIPTNAME#");
							string template = Path.Combine (templatePath, t.ToString () + ".cs.txt");
							string csharp = Path.Combine (csharpPath, t.ToString () + "Template.cs");
							if (File.Exists (template) && File.ReadAllText (template).GetHashCode () != file.GetHashCode () || !File.Exists (template))
							{
								using (StreamWriter sw = new StreamWriter (File.Open (template, FileMode.Create)))
								{
									sw.Write (file);
								}
							}
							if (!File.Exists (csharp))
							{
								using (StreamWriter sw = new StreamWriter (File.Open (csharp, FileMode.Create)))
								{
									string data = format;
									data = Regex.Replace (data, namePattern, t.ToString ());
									sw.Write (data);
								}
							}

						}
					}
				}
			}

			foreach (string dir in Directory.GetFiles (csharpPath))
			{
				if (!foundTypes.Contains (Path.GetFileNameWithoutExtension (dir).Replace ("Template", "")) && Path.GetExtension (dir) == ".cs")
				{
					File.Delete (dir);
				}
			}
			foreach (string dir in Directory.GetFiles (templatePath))
			{
				if (!foundTypes.Contains (Path.GetFileNameWithoutExtension (dir).Replace (".cs", "")) && Path.GetExtension (dir) == ".txt")
				{
					File.Delete (dir);
				}
			}
		}
#endif
	}
}
