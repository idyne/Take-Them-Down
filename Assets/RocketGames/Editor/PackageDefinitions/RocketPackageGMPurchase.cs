using RocketGames.Editor.Models;
using RocketGames.Editor.PackageDefinitions.Base;
using RocketGames.Editor.Utils;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace RocketGames.Editor.PackageDefinitions
{
    public sealed class RocketPackageGMPurchase : RocketPackageBase
    {
	    private AddRequest _addUnityPurchasingRequest;

	    private bool ImportingUnityPurchasing
	    {
		    set { EditorPrefs.SetBool(_importingUnityPurchasingKey, value); }
		    get { return EditorPrefs.GetBool(_importingUnityPurchasingKey, false); }
	    }

	    private string _importingUnityPurchasingKey = "importingUnityPurchasing";

		public RocketPackageGMPurchase() : base(RocketPackageType.RocketGMPurchase, "com.rocket.rocketgmpurchase")
		{
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
            AssetDatabase.importPackageFailed += OnImportPackageFailed;

	        EditorApplication.update += OnPackageUpdateForAddUnityPurchasing;

            if (ImportingUnityPurchasing)
	        {
		        Debug.Log("RocketPackageGMPurchase, IMPORTING UNITY PURCHASING!");
		        OnUnityPurchasingImportFinished();
	        }
		}

        protected override void DoAfterPackageInstallationCompleted()
        {
	        if (RocketPackageManagerHelper.PackageExists("com.unity.purchasing"))
	        {
				OnUnityPurchasingImportFinished();
	        }
	        else
	        {
				_addUnityPurchasingRequest = Client.Add("com.unity.purchasing");
			}
		}

        protected override void ClearRelatedPersistentData()
        {
            base.ClearRelatedPersistentData();

            EditorPrefs.DeleteKey(_importingUnityPurchasingKey);
        }

        private void OnUnityPurchasingImportFinished()
	    {
		    AssetDatabase.ImportPackage(Application.dataPath + "/RocketGames/RocketGMPurchase/Packages/unitypurchasing.unitypackage", false);
	    }

	    private void OnPackageUpdateForAddUnityPurchasing()
	    {
		    if (_addUnityPurchasingRequest != null && _addUnityPurchasingRequest.IsCompleted)
		    {
			    EditorApplication.update -= OnPackageUpdateForAddUnityPurchasing;

			    Debug.Log("OnPackageUpdateForAddUnityPurchasing");

			    ImportingUnityPurchasing = true;
		    }
	    }

		private void OnImportPackageCompleted(string packagename)
        {
            if (packagename == "unitypurchasing")
            {
                RocketPackageManagerHelper.AddDefineSymbols(new[] { ";ROC_GM_PURCHASE" });

                ImportingUnityPurchasing = false;

                RocketPackageManager.FinishAddingPackage(PackageType);
            }
        }

        private void OnImportPackageFailed(string packagename, string errormessage)
        {
            Debug.LogError(string.Format("Import package {0} failed. Error: {1}", packagename, errormessage));
        }
    }
}
