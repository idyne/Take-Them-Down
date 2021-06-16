using RocketGames.Editor.Models;
using RocketGames.Editor.PackageDefinitions.Base;
using RocketGames.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace RocketGames.Editor.PackageDefinitions
{
    public sealed class RocketPackageGMAdjust : RocketPackageBase
    {
        public RocketPackageGMAdjust() : base(RocketPackageType.RocketGMAdjust, "com.rocket.rocketgmadjust")
        {
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
            AssetDatabase.importPackageFailed += OnImportPackageFailed;
        }

        protected override void DoAfterPackageInstallationCompleted()
        {
            AssetDatabase.ImportPackage(Application.dataPath + "/RocketGames/RocketGMAdjust/Packages/adjust.unitypackage", false);
        }

        private void OnImportPackageCompleted(string packagename)
        {
            if (packagename == "adjust")
            {
                RocketPackageManagerHelper.AddDefineSymbols(new[] { ";ROC_GM_ADJUST" });

                if (IsCleanInstall)
                {
                    AssetDatabase.ImportPackage(Application.dataPath + "/RocketGames/RocketGMAdjust/Packages/rocketgmadjust.unitypackage", false);
                }
                else
                {
                    RocketPackageManager.FinishAddingPackage(PackageType);
                }
            }
            else if (packagename == "rocketgmadjust")
            {
                RocketPackageManager.FinishAddingPackage(PackageType);
            }
        }

        private void OnImportPackageFailed(string packagename, string errormessage)
        {
            Debug.LogError(string.Format("Import package {0} failed. Error: {1}", packagename, errormessage));
        }
    }
}
