using RocketGames.Editor.Models;
using RocketGames.Editor.PackageDefinitions.Base;
using RocketGames.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace RocketGames.Editor.PackageDefinitions
{
    public sealed class RocketPackagePush : RocketPackageWithGoogleDependency
    {
        public RocketPackagePush() : base(RocketPackageType.RocketPush, "com.rocket.rocketpush")
        {
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
            AssetDatabase.importPackageFailed += OnImportPackageFailed;
        }

        protected override void DoAfterExternalDependencyInstallationCompleted()
        {
            AssetDatabase.ImportPackage(Application.dataPath + "/RocketGames/RocketPush/Packages/onesignal.unitypackage", false);
        }

        private void OnImportPackageCompleted(string packagename)
        {
            if (packagename == "onesignal")
            {
                RocketPackageManagerHelper.AddDefineSymbols(new[] { ";ROC_ONESIGNAL" });

                if (IsCleanInstall)
                {
                    AssetDatabase.ImportPackage(Application.dataPath + "/RocketGames/RocketPush/Packages/onesignalconfig.unitypackage", false);
                }
                else
                {
                    FinalizeInstallation();
                }
            }
            else if (packagename == "onesignalconfig")
            {
                FinalizeInstallation();
            }          
        }

        private void OnImportPackageFailed(string packagename, string errormessage)
        {
            Debug.LogError(string.Format("Import package {0} failed. Error: {1}", packagename, errormessage));
        }
    }
}
