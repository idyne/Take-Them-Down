using RocketGames.Editor.Models;
using RocketGames.Editor.Utils;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace RocketGames.Editor.PackageDefinitions.Base
{
    public abstract class RocketPackageBase
    {
        public string PackageId { get; private set; }

        protected RocketPackageType PackageType;        

        protected bool IsCleanInstall
        {
            private set { EditorPrefs.SetBool(_isCleanInstallKey, value); }
            get { return EditorPrefs.GetBool(_isCleanInstallKey, false); }
        }

        protected bool IsRemoveInProgress
        {
            private set { EditorPrefs.SetBool(_isRemoveInProgressKey, value); }
            get { return EditorPrefs.GetBool(_isRemoveInProgressKey, false); }
        }

        protected bool IsAddInProgress
        {
            private set { EditorPrefs.SetBool(_isAddInProgressKey, value); }
            get { return EditorPrefs.GetBool(_isAddInProgressKey, false); }
        }

        protected string AddedPackageId
        {
            private set { EditorPrefs.SetString(_addedPackageIdKey, value); }
            get { return EditorPrefs.GetString(_addedPackageIdKey, ""); }
        }

        public string PackageName
        {
            get { return PackageType.ToString(); }
        }

        protected AddRequest AddRequest;
        protected RemoveRequest RemoveRequest;
        protected SearchRequest SearchRequest;

        private readonly string _isCleanInstallKey = "isCleanInstall";
        private readonly string _isRemoveInProgressKey = "isRemoveInProgress";
        private readonly string _isAddInProgressKey = "isAddInProgress";
        private readonly string _addedPackageIdKey = "addedPackageId";

        protected RocketPackageBase(RocketPackageType packageType, string packageId)
        {
            PackageType = packageType;
            PackageId = packageId;

            //When package update callback status comes successful, importing does not finish
            //Workaround for that situation
            if (IsRemoveInProgress)
            {
                IsRemoveInProgress = false;
                AddNewVersion();
            }

            //When package update callback status comes successful, importing does not finish
            //Workaround for that situation
            if (IsAddInProgress)
            {
                IsAddInProgress = false;
                FinalizeAddPackage();
            }
        }
        
        public virtual void Add(bool isCleanInstall)
        {
            IsCleanInstall = isCleanInstall;
           
            ClearRelatedPersistentData();

            if (!RocketPackageManagerHelper.PackageExists(PackageId))
            {
                AddNewVersion();
            }
            else
            {
                SearchPackage();
            }
        }

        private void SearchPackage()
        {
            EditorApplication.update += OnpackageUpdateForSearch;
            SearchRequest = Client.Search(PackageId);
        }

        private void OnpackageUpdateForSearch()
        {
            if (SearchRequest.IsCompleted)
            {
                EditorApplication.update -= OnpackageUpdateForSearch;

                if (SearchRequest.Result != null && SearchRequest.Result.Length > 0)
                {
                    var currentVersion = RocketPackageManagerHelper.GetPackageVersion(PackageId);
                    var newVersion = SearchRequest.Result[0].packageId.Split('@')[1];

                    if (currentVersion == newVersion)
                    {
                        Debug.Log("Already using newest version!");
                        RocketPackageManager.FinishAddingPackage(PackageType);
                    }
                    else
                    {
                        AddNewVersion();
                    }
                }

            }
        }

        protected virtual void RemoveOldVersion()
        {
            Debug.Log("Removing old version");

            EditorApplication.update += OnPackageUpdateForRemove;
            RemoveRequest = Client.Remove(PackageId);
        }

        protected virtual void ClearRelatedPersistentData()
        {
            EditorPrefs.DeleteKey(_isAddInProgressKey);
            EditorPrefs.DeleteKey(_isRemoveInProgressKey);
        }

        private void OnPackageUpdateForRemove()
        {
            if (RemoveRequest.IsCompleted)
            {
                EditorApplication.update -= OnPackageUpdateForRemove;

                IsRemoveInProgress = true;
            }
        }

        private void AddNewVersion()
        {
            Debug.Log("Adding new version");

            RocketPackageManagerHelper.DeleteDirectoryContents(Application.dataPath + "/RocketGames/" + PackageName, IsCleanInstall);
            RocketPackageManagerHelper.AddScopedRegistry(ScopedRegistryType.Rocket);
            
            EditorApplication.update += OnPackageUpdateForAdd;
            AddRequest = Client.Add(PackageId);
        }

        private void OnPackageUpdateForAdd()
        {
            if (AddRequest.IsCompleted)
            {
                EditorApplication.update -= OnPackageUpdateForAdd;

                IsAddInProgress = true;

                AddedPackageId = AddRequest.Result.packageId;
            }
        }

        private void FinalizeAddPackage()
        {
            RocketPackageManagerHelper.MoveDirectory("./Library/PackageCache/" + AddedPackageId + "/Runtime/RocketResources/", PackageName, IsCleanInstall);

            Debug.Log(string.Format("{0} installation completed!", AddedPackageId));

            DoAfterPackageInstallationCompleted();

            AddedPackageId = "";

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);            
        }

        protected abstract void DoAfterPackageInstallationCompleted();
    }
}
