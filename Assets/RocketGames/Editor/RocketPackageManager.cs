using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using RocketGames.Editor.Models;
using RocketGames.Editor.PackageDefinitions;
using RocketGames.Editor.PackageDefinitions.Base;
using RocketGames.Editor.Utils;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace RocketGames.Editor
{
	[InitializeOnLoad]
	static class RocketPackageManager
	{
	    public static RocketPackageType CurrentProgressType
	    {
	        get { return (RocketPackageType) EditorPrefs.GetInt(_currentProgressTypeKey, (int) RocketPackageType.None); }
	        set { EditorPrefs.SetInt(_currentProgressTypeKey, (int) value);}
	    }        

	    private const string _currentProgressTypeKey = "currentProgressTypeKey";

		static RocketPackageManager()
		{
		    GetCurrentPackage();
		}

	    public static void AddPackage(RocketPackageType packageType, bool isCleanInstall)
	    {
	        CurrentProgressType = packageType;

	        RocketPackageBase currentPackage = GetCurrentPackage();
	        currentPackage.Add(isCleanInstall);
	    }

	    private static RocketPackageBase GetCurrentPackage()
	    {
	        switch (CurrentProgressType)
	        {
	            case RocketPackageType.RocketGM:
	                return new RocketPackageGM();
	            case RocketPackageType.RocketAd:
	                return new RocketPackageAd();
	            case RocketPackageType.RocketGMAdjust:
	                return new RocketPackageGMAdjust();
	            case RocketPackageType.RocketPush:
	                return new RocketPackagePush();
	            case RocketPackageType.RocketGMPurchase:
	                return new RocketPackageGMPurchase();
	            case RocketPackageType.RocketFacebook:
	                return new RocketPackageFacebook();
                case RocketPackageType.RocketRateUs:
                    return new RocketPackageRateUs();
                default:
                    return null;
            }
        }
        
	    public static void FinishAddingPackage(RocketPackageType packageType)
	    {
	        string packageName = packageType.ToString();

	        if (Directory.Exists(string.Format("{0}/RocketGames/{1}/Packages", Application.dataPath, packageName)))
	        {
	            Directory.Delete(string.Format("{0}/RocketGames/{1}/Packages", Application.dataPath, packageName), true);
	        }

	        if (File.Exists(string.Format("{0}/RocketGames/{1}/Packages.meta", Application.dataPath, packageName)))
	        {
	            File.Delete(string.Format("{0}/RocketGames/{1}/Packages.meta", Application.dataPath, packageName));
	        }

            CurrentProgressType = RocketPackageType.None;
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);	        

            Debug.Log(string.Format("{0} module installation completed!", packageType));
	    }
	}
}