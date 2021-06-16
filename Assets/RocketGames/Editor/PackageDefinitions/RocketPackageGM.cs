using RocketGames.Editor.Models;
using RocketGames.Editor.PackageDefinitions.Base;
using RocketGames.Editor.Utils;

namespace RocketGames.Editor.PackageDefinitions
{
    public sealed class RocketPackageGM : RocketPackageBase
    {
        public RocketPackageGM() : base(RocketPackageType.RocketGM, "com.rocket.rocketgm")
        {
        }

        protected override void DoAfterPackageInstallationCompleted()
        {
            RocketPackageManagerHelper.AddDefineSymbols(new[] { ";ROC_GM_REPORTING" });

            RocketPackageManager.FinishAddingPackage(PackageType);
        }
    }
}