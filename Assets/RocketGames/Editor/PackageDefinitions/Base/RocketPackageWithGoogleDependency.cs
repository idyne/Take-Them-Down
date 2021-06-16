using System;
using System.Linq;
using System.Reflection;
using RocketGames.Editor.Models;
using RocketGames.Editor.Utils;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace RocketGames.Editor.PackageDefinitions.Base
{
    public abstract class RocketPackageWithGoogleDependency : RocketPackageBase
    {
        private AddRequest _addExternalDependencyRequest;

        private bool ImportingResolver
        {
            set { EditorPrefs.SetBool(_importingResolverKey, value); }
            get { return EditorPrefs.GetBool(_importingResolverKey, false); }
        }

        private string _importingResolverKey = "importingResolver";

        protected RocketPackageWithGoogleDependency(RocketPackageType packageType, string packageId) : base(packageType, packageId)
        {
            EditorApplication.update += OnPackageUpdateForAddExternalDependency;

            if (ImportingResolver)
            {
                Debug.Log("Importing resolver");
                OnExternalDependencyImportFinished();
            }
        }

        protected override void DoAfterPackageInstallationCompleted()
        {
            DoExternalDependencyOperations();
        }

        protected override void ClearRelatedPersistentData()
        {
            base.ClearRelatedPersistentData();

            EditorPrefs.DeleteKey(_importingResolverKey);
        }


        private void OnPackageUpdateForAddExternalDependency()
        {
            if (_addExternalDependencyRequest != null && _addExternalDependencyRequest.IsCompleted)
            {
                EditorApplication.update -= OnPackageUpdateForAddExternalDependency;

                Debug.Log("OnPackageUpdateForAddExternalDependency");

                ImportingResolver = true;
            }
        }
        
        private void DoExternalDependencyOperations()
        {
            //Search for externaldependencymanager
            if (!RocketPackageManagerHelper.PackageExists("com.google.external-dependency-manager"))
                AddExternalDependencyManager();
            else
                OnExternalDependencyImportFinished();
        }

        private void AddExternalDependencyManager()
        {
            RocketPackageManagerHelper.AddScopedRegistry(ScopedRegistryType.Google);
            _addExternalDependencyRequest = Client.Add("com.google.external-dependency-manager");
        }        

        private void OnExternalDependencyImportFinished()
        {
            var jarResolverAssembly = (from Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()
                    where (assembly.FullName.Contains("Google.JarResolver"))
                    select assembly
                )
                .FirstOrDefault();

            if (jarResolverAssembly != null)
            {
                EditorApplication.update -= OnExternalDependencyImportFinished;

                EditorPrefs.DeleteKey(_importingResolverKey);

                DoAfterExternalDependencyInstallationCompleted();
            }
        }

        protected void FinalizeInstallation()
        {
            RocketPackageManagerHelper.EnableCustomGradle();
            RocketPackageManagerHelper.ResolveGradle();
            RocketPackageManagerHelper.AddMultiDex();

            if (PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel19)
            {
                PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel19;
            }

            AssetDatabase.SaveAssets();

            RocketPackageManager.FinishAddingPackage(PackageType);
        }

        protected abstract void DoAfterExternalDependencyInstallationCompleted();
    }
}
