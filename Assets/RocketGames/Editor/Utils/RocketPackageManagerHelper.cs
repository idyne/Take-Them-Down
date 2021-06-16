using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using RocketGames.Editor.Models;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace RocketGames.Editor.Utils
{
	public class RocketPackageManagerHelper
	{
        private static List<string> _dirtyDirectoryList = new List<string>{"Prefabs","Plugins"};

        private static Dictionary<ScopedRegistryType, List<string>> _scopedRegistryDictionary = new Dictionary<ScopedRegistryType, List<string>>
        {
            {ScopedRegistryType.Rocket,  new List<string>{
                                                                "\t\t{",
                                                                "\t\t\t\"name\": \"Rocket Games Package Registry\",",
                                                                "\t\t\t\"url\": \"https://registry.npmjs.org/\",",
                                                                "\t\t\t\"scopes\": [",
                                                                "\t\t\t\t\"com.rocket\"",
                                                                "\t\t\t]",
                                                                "\t\t}",
                                                                }
            },
            {ScopedRegistryType.Google, new List<string>{
                                                        "\t\t{",
                                                        "\t\t\t\"name\": \"Game Package Registry by Google\",",
                                                        "\t\t\t\"url\": \"https://unityregistry-pa.googleapis.com\",",
                                                        "\t\t\t\"scopes\": [",
                                                        "\t\t\t\t\"com.google\"",
                                                        "\t\t\t]",
                                                        "\t\t}",
                                                        }
            }
        };

        public static UnityVersion UnityVersion
	    {
	        get
	        {
	            if (Application.unityVersion.Contains("2018"))
	            {
	                return UnityVersion.UNITY_2018;
	            }

	            if (Application.unityVersion.Contains("2019"))
	            {
	                return UnityVersion.UNITY_2019;
	            }

	            if (Application.unityVersion.Contains("2020"))
	            {
	                return UnityVersion.UNITY_2020;
	            }

                return UnityVersion.NONE;
	        }
	    }

	    public static bool PackageExists(string packageId)
		{
			using (StreamReader reader = File.OpenText("./Packages/manifest.json"))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (line.Contains(packageId))
					{
						return true;
					}
				}
			}

			return false;
		}

	    public static string GetPackageVersion(string packageId)
	    {
	        using (StreamReader reader = File.OpenText("./Packages/manifest.json"))
	        {
	            string line;
	            while ((line = reader.ReadLine()) != null)
	            {
	                if (line.Contains(packageId))
	                {
	                    string[] splitLine = line.Split(':');
	                    return splitLine[1].Replace(",", "").Replace("\"", "").Trim();
                    }
	            }
	        }

	        return "0";
	    }

        public static void MoveDirectory(string sourceRootPath, string packageName, bool isCleanInstall)
		{
            if (!Directory.Exists(sourceRootPath))
                return;

            string destinationRootPath = Application.dataPath + "/RocketGames/";

            DirectoryInfo directoryInfo = new DirectoryInfo(sourceRootPath);
			try
			{
				foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
				{
				    string destinationDirectory = destinationRootPath + packageName + "/" + dir.Name + "/";

                    if (!isCleanInstall && Directory.Exists(destinationDirectory) && _dirtyDirectoryList.Contains(dir.Name))
				    {
                        Debug.Log(string.Format("Do not overwrite {0}", dir.Name));
                        continue;
				    }


				    foreach (FileInfo file in dir.GetFiles())
					{
						if (file.FullName.Contains(".meta"))
							continue;
					    
					    string destinationFilePath = destinationDirectory + file.Name;

					    if (File.Exists(destinationFilePath))
					    {
					        File.Delete(destinationFilePath);
                        }

						if (!Directory.Exists(destinationDirectory))
							Directory.CreateDirectory(destinationDirectory);

						File.Copy(file.FullName, destinationFilePath);
					}

					MoveDirectory(dir.FullName, packageName, isCleanInstall);
				}
			}
			catch (System.Exception exception)
			{
				Debug.LogError(exception.Message);
			}
		}

	    public static void DeleteDirectoryContents(string destinationRootPath, bool isCleanInstall)
	    {	        
            if(!Directory.Exists(destinationRootPath)) return;

	        DirectoryInfo directoryInfo = new DirectoryInfo(destinationRootPath);
	        try
	        {
	            foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
	            {
	                string destinationDirectory = destinationRootPath + "/" + dir.Name + "/";

	                if (!isCleanInstall && Directory.Exists(destinationDirectory) && _dirtyDirectoryList.Contains(dir.Name))
	                {
	                    continue;
	                }

                    foreach (FileInfo file in dir.GetFiles())
	                {
	                    if (file.FullName.Contains(".meta"))
	                        continue;

	                    string destinationFilePath = destinationDirectory + file.Name;

	                    if (File.Exists(destinationFilePath))
	                    {
	                        File.Delete(destinationFilePath);
	                    }
	                }

	                DeleteDirectoryContents(dir.FullName, isCleanInstall);
	            }
	        }
	        catch (System.Exception exception)
	        {
	            Debug.LogError(exception.Message);
	        }
	    }

	    public static bool DefineSymbolExists(BuildTargetGroup buildTargetGroup, string defineSymbol)
	    {
	        string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

	        return defineSymbols.Contains(defineSymbol);
	    }

	    public static void AddDefineSymbols(string[] defineSymbols)
		{
            AddDefineSymbols(defineSymbols, BuildTargetGroup.Android);
            AddDefineSymbols(defineSymbols, BuildTargetGroup.iOS);
            AddDefineSymbols(defineSymbols, BuildTargetGroup.Standalone);
		}

	    public static void AddDefineSymbols(string[] defineSymbols, BuildTargetGroup buildTargetGroup)
	    {
	        for (var index = 0; index < defineSymbols.Length; index++)
	        {
	            string defineSymbol = defineSymbols[index];

                if (DefineSymbolExists(buildTargetGroup, defineSymbol))
	            {
	                defineSymbols[index] = "";
	                continue;
	            }

                
	            if (!defineSymbol.Contains(";"))
	            {
	                defineSymbol = ";" + defineSymbol;
	            }

	            defineSymbols[index] = defineSymbol;
	        }

	        string additionalSymbols = string.Join("", defineSymbols);
            
	        string newDefineSymbols = FixDefineSymbolsString(PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup) + additionalSymbols);
	        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, newDefineSymbols);
	    }

        public static void RemoveDefineSymbols(string[] defineSymbols)
	    {
	        string defineSymbolsAndroid = FixDefineSymbolsString(PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android));
	        string defineSymbolsIOS = FixDefineSymbolsString(PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS));
	        string defineSymbolsStandalone = FixDefineSymbolsString(PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone));

            foreach (string defineSymbol in defineSymbols)
            {
                if (defineSymbolsAndroid.Contains(defineSymbol))
                    defineSymbolsAndroid = defineSymbolsAndroid.Replace(defineSymbol, "");
                if (defineSymbolsIOS.Contains(defineSymbol))
                    defineSymbolsIOS = defineSymbolsIOS.Replace(defineSymbol, "");
                if (defineSymbolsStandalone.Contains(defineSymbol))
                    defineSymbolsStandalone = defineSymbolsStandalone.Replace(defineSymbol, "");
            }

	        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, FixDefineSymbolsString(defineSymbolsAndroid));
	        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, FixDefineSymbolsString(defineSymbolsIOS));
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, FixDefineSymbolsString(defineSymbolsStandalone));
        }

	    public static void RemoveDefineSymbols(string[] defineSymbols, BuildTargetGroup buildTargetGroup)
	    {
	        string currentDefineSymbols = FixDefineSymbolsString(PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup));

	        foreach (string defineSymbol in defineSymbols)
	        {
	            if (currentDefineSymbols.Contains(defineSymbol))
	                currentDefineSymbols = currentDefineSymbols.Replace(defineSymbol, "");
	        }

	        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, FixDefineSymbolsString(currentDefineSymbols));
	    }


        public static string FixDefineSymbolsString(string symbols)
		{
			// Remove double ';'
			symbols = symbols.Replace(";;", ";");
			// Remove leading ';'.
			if (symbols.Length > 0 && symbols[0] == ';')
			{
				symbols = symbols.Substring(1);
			}

			return RemoveDuplicateSymbols(symbols);
		}

		public static string RemoveDuplicateSymbols(string symbolsStr)
		{
			HashSet<string> symbols = new HashSet<string>();

			foreach (string s in symbolsStr.Split(';'))
			{
				if (!string.IsNullOrEmpty(s))
				{
					symbols.Add(s);
				}
			}

			return string.Join(";", symbols.ToArray());
		}

	    public static RocketEditorSettings ReadEditorSettings()
	    {
	        string editorSettingsPath = Application.dataPath + "/RocketGames/Editor/RocketEditorSettings.txt";

	        RocketEditorSettings rocketEditorSettings;


            if (!File.Exists(editorSettingsPath))
	        {
                rocketEditorSettings = new RocketEditorSettings();
                SaveEditorSettings(rocketEditorSettings);	            
	        }
            else
            {
                using (StreamReader reader = new StreamReader(editorSettingsPath))
                {
                    string json = reader.ReadToEnd();
                    rocketEditorSettings = JsonUtility.FromJson<RocketEditorSettings>(json);
                }
            }

	        return rocketEditorSettings;
        }

	    public static void SaveEditorSettings(RocketEditorSettings rocketEditorSettings)
	    {
	        string editorSettingsPath = Application.dataPath + "/RocketGames/Editor/RocketEditorSettings.txt";
            
            string json = JsonUtility.ToJson(rocketEditorSettings);
	        File.WriteAllText(editorSettingsPath, json);
            
            AssetDatabase.Refresh();
        }

	    public static void AddScopedRegistry(ScopedRegistryType scopedRegistryType)
	    {
	        List<string> scopedRegistryList = _scopedRegistryDictionary[scopedRegistryType];

            List<string> lines = new List<string>();

	        using (StreamReader reader = File.OpenText("./Packages/manifest.json"))
	        {
	            string line;
	            while ((line = reader.ReadLine()) != null)
	            {
	                if (line.Contains("com." + scopedRegistryType.ToString().ToLower()))
	                {
	                    Debug.Log("Package manifest already contains NPM registry, skipping...");
	                    return;
	                }

	                lines.Add(line);
	            }
	        }

	        int scopedRegistryIndex = lines.FindIndex((line) => line.Contains("scopedRegistries"));
	        int newScopeIndex;


	        if (scopedRegistryIndex == -1)
	        {
	            newScopeIndex = lines.FindLastIndex(line => line.Contains("}"));

	            scopedRegistryList.Insert(0, "\t,\"scopedRegistries\": [");
	            scopedRegistryList.Insert(scopedRegistryList.Count, "\t]");
	        }
	        else
	        {
	            scopedRegistryList[scopedRegistryList.Count - 1] += ",";
	            newScopeIndex = scopedRegistryIndex + 1;
	        }

	        for (int index = 0; index < scopedRegistryList.Count; index++)
	        {
	            string row = scopedRegistryList[index];
	            lines.Insert(newScopeIndex + index, row);
	        }

	        // Write the new file over the old file.
	        using (StreamWriter writer = new StreamWriter("./Packages/manifest.json"))
	        {
	            foreach (string line in lines)
	            {
	                writer.Write(line + "\n");
	            }
	        }

	        Debug.Log("Npm Registry Added to package");
	    }

        private static void FixGradleFile(string name, string extension, string version)
		{
			if (File.Exists(string.Format("{0}/Plugins/Android/{1}.{2}.DISABLED", Application.dataPath, name, extension)))
			{
				File.Move(string.Format("{0}/Plugins/Android/{1}.{2}.DISABLED", Application.dataPath, name, extension),
							string.Format("{0}/Plugins/Android/{1}.{2}", Application.dataPath, name, extension));
			}

			if (!File.Exists(string.Format("{0}/Plugins/Android/{1}.{2}", Application.dataPath, name, extension)))
			{
				if (!Directory.Exists(Application.dataPath + "/Plugins/Android"))
				{
					Directory.CreateDirectory(Application.dataPath + "/Plugins/Android");
				}

				File.Move(string.Format("{0}/RocketGames/Templates/{1}_{2}.{3}", Application.dataPath, name, version, extension),
							string.Format("{0}/Plugins/Android/{1}.{2}", Application.dataPath, name, extension));
			}
		}

		public static void AddMultiDex()
		{
		    string gradleName = "";

		    switch (UnityVersion)
		    {
		        case UnityVersion.UNITY_2018:
		            gradleName = "mainTemplate";//
		            break;
		        case UnityVersion.UNITY_2019:
                case UnityVersion.UNITY_2020:
		            gradleName = "launcherTemplate";
		            break;
		        default:
		            Debug.LogError(string.Format("Current Unity Version {0} is NOT defined!", Application.unityVersion));
		            break;
		    }

            string gradlePath = string.Format("{0}/Plugins/Android/{1}.gradle", Application.dataPath, gradleName);
			string gradleText = File.ReadAllText(gradlePath);

			if (gradleText.Contains("multiDexEnabled false"))
			{
				gradleText = gradleText.Replace("multiDexEnabled false", "multiDexEnabled true");
			}
			else if (!gradleText.Contains("multiDexEnabled"))
			{
				gradleText = gradleText.Replace("defaultConfig {", "defaultConfig {\n\t\tmultiDexEnabled true");
			}

			File.WriteAllText(gradlePath, gradleText);
		}

	    public static void ResolveGradle()
	    {
	        Assembly jarResolverAssembly = (
	                from Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()
	                where (assembly.FullName.Contains("Google.JarResolver"))
	                select assembly
	            )
	            .FirstOrDefault();

	        if (jarResolverAssembly != null)
	        {
	            Type jarResolverType =
	                jarResolverAssembly.GetType(string.Format("{0}.{1}", "GooglePlayServices", "PlayServicesResolver"));
	            if (jarResolverType != null)
	            {
	                //Check if exists, instantiate if so.
	                Object jarResolverInstance = Activator.CreateInstance(jarResolverType);

	                if (jarResolverInstance != null)
	                {
	                    MethodInfo deleteMethod = jarResolverType.GetMethod("DeleteResolvedLibrariesSync");
	                    if (deleteMethod != null)
	                    {
	                        deleteMethod.Invoke(jarResolverInstance, null);
	                    }

	                    object[] parameters = { true };
	                    MethodInfo resolveMethod = jarResolverType.GetMethod("ResolveSync");
	                    if (resolveMethod != null)
	                    {
	                        resolveMethod.Invoke(jarResolverInstance, parameters);
	                    }
	                }
	            }
	        }
	        else
	        {
	            Debug.LogError("Google.JarResolver Assembly NOT found");
	        }
	    }

	    public static void EnableCustomGradle()
	    {
	        switch (UnityVersion)
	        {
	            case UnityVersion.UNITY_2018:
	                FixGradleFile("mainTemplate", "gradle", "2018");
	                break;
	            case UnityVersion.UNITY_2019:
	            case UnityVersion.UNITY_2020:
                    FixGradleFile("mainTemplate", "gradle", "2019");
	                FixGradleFile("baseProjectTemplate", "gradle", "2019");
	                FixGradleFile("launcherTemplate", "gradle", "2019");
	                FixGradleFile("gradleTemplate", "properties", "2019");
	                break;
                default:
	                Debug.LogError(string.Format("Current Unity Version {0} is NOT defined!", Application.unityVersion));
	                break;
	        }
	    }
    }
}
