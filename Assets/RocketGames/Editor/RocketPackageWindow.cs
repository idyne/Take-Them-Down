using System;
using RocketGames.Editor.Models;
using RocketGames.Editor.PackageWindowComponents;
using RocketGames.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace RocketGames.Editor
{
	public class RocketPackageWindow : EditorWindow
	{
		private static bool _isCleanInstall;

		private const float _width = 300;
		private const float _height = 150;

		private static RocketPackageType _currentPackageType;

	    [MenuItem("RocketGames/Install/GM", false, -100)]
		private static void InstallGm()
	    {
		    _currentPackageType = RocketPackageType.RocketGM;

            if (!RocketPackageManagerHelper.PackageExists("com.rocket.rocketgm"))
            {
                RocketPackageManager.AddPackage(_currentPackageType, true);
            }
            else
            {
				CreateWindow("Rocket Games GM Installation");
            }
        }		

		[MenuItem("RocketGames/Install/Ad", false, -100)]
		private static void InstallAd()
		{
			_currentPackageType = RocketPackageType.RocketAd;

			if (!RocketPackageManagerHelper.PackageExists("com.rocket.rocketad"))
			{
			    RocketPackageManager.AddPackage(_currentPackageType, true);
            }
			else
			{
				CreateWindow("Rocket Games Ad Installation");
			}
		}

	    [MenuItem("RocketGames/Install/Push", false, -100)]
	    private static void InstallPush()
	    {
		    _currentPackageType = RocketPackageType.RocketPush;

			if (!RocketPackageManagerHelper.PackageExists("com.rocket.rocketpush"))
	        {
	            RocketPackageManager.AddPackage(_currentPackageType, true);
	        }
	        else
	        {
				CreateWindow("Rocket Games Push Installation");
			}
	    }

	    [MenuItem("RocketGames/Install/Facebook", false, -100)]
	    private static void InstallFacebook()
	    {
	        _currentPackageType = RocketPackageType.RocketFacebook;

	        if (!RocketPackageManagerHelper.PackageExists("com.rocket.rocketfacebook"))
	        {
	            RocketPackageManager.AddPackage(_currentPackageType, true);
	        }
	        else
	        {
	            CreateWindow("Rocket Games Facebook Installation");
	        }
	    }

	    [MenuItem("RocketGames/Install/RateUs", false, -100)]
	    private static void InstallRateUs()
	    {
	        _currentPackageType = RocketPackageType.RocketRateUs;

	        if (!RocketPackageManagerHelper.PackageExists("com.rocket.rocketrateus"))
	        {
	            RocketPackageManager.AddPackage(_currentPackageType, true);
	        }
	        else
	        {
	            CreateWindow("Rocket Games Rate Us Installation");
	        }
	    }

        [MenuItem("RocketGames/Install/GMAdjust", false, -40)]
	    private static void InstallGMAdjust()
	    {
		    _currentPackageType = RocketPackageType.RocketGMAdjust;

			if (!RocketPackageManagerHelper.PackageExists("com.rocket.rocketgmadjust"))
	        {
	            RocketPackageManager.AddPackage(_currentPackageType, true);
	        }
	        else
	        {
				CreateWindow("Rocket Games GMAdjust Installation");
			}
	    }

	    [MenuItem("RocketGames/Install/GMAdjust", true)]
	    private static bool GmAdjustEnabled()
	    {
	        return RocketPackageManagerHelper.PackageExists("com.rocket.rocketgm");
	    }

		[MenuItem("RocketGames/Install/GMPurchase", false, -40)]
		private static void InstallGMPurchase()
		{
			_currentPackageType = RocketPackageType.RocketGMPurchase;

			if (!RocketPackageManagerHelper.PackageExists("com.rocket.rocketgmpurchase"))
			{
				RocketPackageManager.AddPackage(_currentPackageType, true);
			}
			else
			{
				CreateWindow("Rocket Games GMPurchase Installation");
			}
		}

		[MenuItem("RocketGames/Install/GMPurchase", true)]
		private static bool GmPurchaseEnabled()
		{
			return RocketPackageManagerHelper.PackageExists("com.rocket.rocketgm");
		}

		private static void CreateWindow(string title)
		{
			// Get existing open window or if none, make a new one:
			RocketPackageWindow window = (RocketPackageWindow)GetWindow(typeof(RocketPackageWindow));
			window.titleContent = new GUIContent(title);
			window.position = new Rect(new Vector2((Screen.width - _width) / 2, (Screen.height - _height) / 2), new Vector2(_width, _height));
			window.ShowPopup();
			window.name = _currentPackageType.ToString();
		}

		private void OnGUI()
		{
            GUILayout.Space(10);

			float originalValue = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 200;

			_isCleanInstall = EditorGUILayout.Toggle("Overwrite RocketGames folder?", _isCleanInstall);

			EditorGUIUtility.labelWidth = originalValue;

			GUILayout.Label("This option is used for clean install", EditorStyles.boldLabel);

			GUILayout.Space(20);

			if (GUILayout.Button("INSTALL"))
			{
			    RocketPackageWindow window = (RocketPackageWindow)GetWindow(typeof(RocketPackageWindow));
                RocketPackageManager.AddPackage(_currentPackageType, _isCleanInstall);
                window.Close();
			}
		}
	}
}