using System.IO;
using RocketGames.Editor.Models;
using RocketGames.Editor.PackageDefinitions.Base;
using RocketGames.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace RocketGames.Editor.PackageDefinitions
{
    public sealed class RocketPackageFacebook : RocketPackageWithGoogleDependency
    {        
        public RocketPackageFacebook() : base(RocketPackageType.RocketFacebook, "com.rocket.rocketfacebook")
        {
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
            AssetDatabase.importPackageFailed += OnImportPackageFailed;          
        }

        protected override void DoAfterExternalDependencyInstallationCompleted()
        {
            RemoveFacebook();
            AssetDatabase.ImportPackage(Application.dataPath + "/RocketGames/RocketFacebook/Packages/facebook.unitypackage", false);
        }

        private void RemoveFacebook()
        {
            RocketPackageManagerHelper.RemoveDefineSymbols(new[] { ";ROC_FACEBOOK" });

            if (Directory.Exists("./Assets/FacebookSDK"))
                Directory.Delete("./Assets/FacebookSDK", true);
            if (File.Exists("./Assets/FacebookSDK.meta"))
                File.Delete("./Assets/FacebookSDK.meta");
        }

        private void OnImportPackageCompleted(string packagename)
        {
            if (packagename.Contains("facebook"))
            {
                RocketPackageManagerHelper.AddDefineSymbols(new[] { ";ROC_FACEBOOK" });

                FinalizeInstallation();
            }
        }        

        private void OnImportPackageFailed(string packagename, string errormessage)
        {
            Debug.LogError(string.Format("Import package {0} failed. Error: {1}", packagename, errormessage));
        }
    }
}
