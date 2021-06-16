using System.IO;
using RocketGames.Editor.Models;
using RocketGames.Editor.PackageDefinitions.Base;
using RocketGames.Editor.Utils;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace RocketGames.Editor.PackageDefinitions
{
    public sealed class RocketPackageAd : RocketPackageWithGoogleDependency
    {            
        private bool IsRemoveUnityAdsInProgress
        {
            set { EditorPrefs.SetBool(_removingUnityAdsKey, value); }
            get { return EditorPrefs.GetBool(_removingUnityAdsKey, false); }
        }

        private string _removingUnityAdsKey = "removingUnityAds";

        public RocketPackageAd() : base(RocketPackageType.RocketAd, "com.rocket.rocketad")
        {
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
            AssetDatabase.importPackageFailed += OnImportPackageFailed;

            //When package update callback status comes successful, importing does not finish
            //Workaround for that situation
            if (IsRemoveUnityAdsInProgress)
            {
                IsRemoveUnityAdsInProgress = false;
                FinalizeInstallation();
            }
        }

        protected override void DoAfterExternalDependencyInstallationCompleted()
        {            
            RemoveMopub();
            RocketPackageManagerHelper.AddDefineSymbols(new[] { ";ROC_AD" });

            AssetDatabase.ImportPackage(Application.dataPath + "/RocketGames/RocketAd/Packages/mopub.unitypackage", false);
        }

        protected override void ClearRelatedPersistentData()
        {
            base.ClearRelatedPersistentData();

            EditorPrefs.DeleteKey(_removingUnityAdsKey);
        }

        private void RemoveMopub()
        {
            RocketPackageManagerHelper.RemoveDefineSymbols(new[] { ";mopub_manager" });

            if (Directory.Exists("./Assets/MoPub"))
                Directory.Delete("./Assets/MoPub", true);
            if (File.Exists("./Assets/MoPub.meta"))
                File.Delete("./Assets/MoPub.meta");
        }

        private void OnImportPackageCompleted(string packagename)
        {
            if (packagename.Contains("mopub"))
            {
                RocketPackageManagerHelper.AddDefineSymbols(new[] { ";mopub_manager" });

                if (IsCleanInstall)
                    AssetDatabase.ImportPackage(Application.dataPath + "/RocketGames/RocketAd/Packages/rocketad.unitypackage", false);
                else
                    RemoveUnityAds();
            }
            else if (packagename.Contains("rocketad"))
            {
                RemoveUnityAds();
            }
        }

        private void OnImportPackageFailed(string packagename, string errormessage)
        {
            Debug.LogError(string.Format("Import package {0} failed. Error: {1}", packagename, errormessage));
        }

        private void RemoveUnityAds()
        {
            if (RocketPackageManagerHelper.PackageExists("com.unity.ads"))
            {
                EditorApplication.update += OnUnityAdsPackageRemoved;
                RemoveRequest = Client.Remove("com.unity.ads");
            }
            else
            {
                FinalizeInstallation();
            }
        }

        private void OnUnityAdsPackageRemoved()
        {
            if (RemoveRequest.IsCompleted)
            {
                EditorApplication.update -= OnUnityAdsPackageRemoved;

                IsRemoveUnityAdsInProgress = true;
            }
        }                                
    }
}
