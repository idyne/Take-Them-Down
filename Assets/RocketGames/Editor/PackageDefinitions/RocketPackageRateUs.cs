using RocketGames.Editor.Models;
using RocketGames.Editor.PackageDefinitions.Base;
using RocketGames.Editor.Utils;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace RocketGames.Editor.PackageDefinitions
{
    public class RocketPackageRateUs : RocketPackageWithGoogleDependency
    {
        private AddRequest _googlePlayCommonAddRequest;
        private AddRequest _googlePlayReviewAddRequest;

        private bool ImportingGooglePlayCommon
        {
            set { EditorPrefs.SetBool(_importingGooglePlayCommonKey, value); }
            get { return EditorPrefs.GetBool(_importingGooglePlayCommonKey, false); }
        }

        private bool ImportingGooglePlayReview
        {
            set { EditorPrefs.SetBool(_importingGooglePlayReviewKey, value); }
            get { return EditorPrefs.GetBool(_importingGooglePlayReviewKey, false); }
        }

        private string _importingGooglePlayCommonKey = "importingGooglePlayCommon";
        private string _importingGooglePlayReviewKey = "importingGooglePlayReview";

        public RocketPackageRateUs() : base(RocketPackageType.RocketRateUs, "com.rocket.rocketrateus")
        {
            EditorApplication.update += OnPackageUpdateForAddGooglePlayCommon;
            EditorApplication.update += OnPackageUpdateForAddGooglePlayReview;

            if (ImportingGooglePlayCommon)
            {
                Debug.Log("RocketPackageRateUs, IMPORTING GooglePlayCommon");
                OnGooglePlayCommonImportFinished();
            }

            if (ImportingGooglePlayReview)
            {
                Debug.Log("RocketPackageRateUs, IMPORTING GooglePlayCommon");
                OnGooglePlayReviewImportFinished();
            }
        }

        protected override void DoAfterExternalDependencyInstallationCompleted()
        {
            if (RocketPackageManagerHelper.PackageExists("com.google.play.common"))
            {
                OnGooglePlayCommonImportFinished();
            }
            else
            {
                _googlePlayCommonAddRequest = Client.Add("com.google.play.common");
            }
        }        

        protected override void ClearRelatedPersistentData()
        {
            base.ClearRelatedPersistentData();

            EditorPrefs.DeleteKey(_importingGooglePlayCommonKey);
            EditorPrefs.DeleteKey(_importingGooglePlayReviewKey);
        }

        private void OnPackageUpdateForAddGooglePlayCommon()
        {
            if (_googlePlayCommonAddRequest != null && _googlePlayCommonAddRequest.IsCompleted)
            {
                EditorApplication.update -= OnPackageUpdateForAddGooglePlayCommon;

                Debug.Log("OnPackageUpdateForAddGooglePlayCommon");

                ImportingGooglePlayCommon = true;
            }
        }

        private void OnPackageUpdateForAddGooglePlayReview()
        {
            if (_googlePlayReviewAddRequest != null && _googlePlayReviewAddRequest.IsCompleted)
            {
                EditorApplication.update -= OnPackageUpdateForAddGooglePlayReview;

                Debug.Log("OnPackageUpdateForAddGooglePlayReview");

                ImportingGooglePlayReview = true;
            }
        }

        private void OnGooglePlayCommonImportFinished()
        {
            ImportingGooglePlayCommon = false;

            if (RocketPackageManagerHelper.PackageExists("com.google.play.review"))
            {
                OnGooglePlayReviewImportFinished();
            }
            else
            {
                _googlePlayReviewAddRequest = Client.Add("com.google.play.review");
            }
        }

        private void OnGooglePlayReviewImportFinished()
        {
            RocketPackageManagerHelper.AddDefineSymbols(new[] { ";ROC_RATEUS" });

            ImportingGooglePlayReview = false;

            RocketPackageManager.FinishAddingPackage(PackageType);
        }
    }
}
