using System.Collections.Generic;
using RocketGames.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace RocketGames.Editor.PackageWindowComponents
{
    [InitializeOnLoad]
    static class DebugWindow
    {
        private static RocketEditorSettings _rocketEditorSettings;

        static DebugWindow()
        {
            _rocketEditorSettings = RocketPackageManagerHelper.ReadEditorSettings();

            EditorApplication.delayCall += () =>
            {
                Menu.SetChecked("RocketGames/Settings/DebugMode", _rocketEditorSettings.DebugEnabled);
                Menu.SetChecked("RocketGames/Settings/LogMode", _rocketEditorSettings.LogEnabled);
                
                if (_rocketEditorSettings.DebugEnabled)
                {
                    RocketPackageManagerHelper.AddDefineSymbols(new[] { "ROC_DEBUG_MODE" });
                }
                else
                {
                    RocketPackageManagerHelper.RemoveDefineSymbols(new[] { "ROC_DEBUG_MODE" });
                }

                if (_rocketEditorSettings.LogEnabled)
                {
                    RocketPackageManagerHelper.AddDefineSymbols(new[] { "ROC_LOG_MODE" });
                }
                else
                {
                    RocketPackageManagerHelper.RemoveDefineSymbols(new[] { "ROC_LOG_MODE" });
                }
            };
        }
        
        [MenuItem("RocketGames/Settings/DebugMode")]
        static void ToggleDebugMode()
        {
            _rocketEditorSettings.DebugEnabled = !_rocketEditorSettings.DebugEnabled;
            _rocketEditorSettings.LogEnabled = _rocketEditorSettings.DebugEnabled;

            if (_rocketEditorSettings.DebugEnabled)
            {
                RocketPackageManagerHelper.AddDefineSymbols(new[] {"ROC_DEBUG_MODE", "ROC_LOG_MODE"});
            }
            else
            {
                RocketPackageManagerHelper.RemoveDefineSymbols(new[] {"ROC_DEBUG_MODE", "ROC_LOG_MODE"});
            }

            Menu.SetChecked("RocketGames/Settings/DebugMode", _rocketEditorSettings.DebugEnabled);
            Menu.SetChecked("RocketGames/Settings/LogMode", _rocketEditorSettings.LogEnabled);

            AssetDatabase.SaveAssets();
            RocketPackageManagerHelper.SaveEditorSettings(_rocketEditorSettings);
        }

        [MenuItem("RocketGames/Settings/LogMode")]
        static void ToggleLogMode()
        {
            _rocketEditorSettings.LogEnabled = !_rocketEditorSettings.LogEnabled;

            if (_rocketEditorSettings.LogEnabled)
            {
                RocketPackageManagerHelper.AddDefineSymbols(new[] {"ROC_LOG_MODE"});
            }
            else
            {
                RocketPackageManagerHelper.RemoveDefineSymbols(new[] {"ROC_LOG_MODE"});
            }

            Menu.SetChecked("RocketGames/Settings/LogMode", _rocketEditorSettings.LogEnabled);

            AssetDatabase.SaveAssets();
            RocketPackageManagerHelper.SaveEditorSettings(_rocketEditorSettings);
        }
    }
}
