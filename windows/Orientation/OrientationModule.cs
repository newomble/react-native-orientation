using ReactNative.Bridge;
using ReactNative.Modules.Core;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Display;
using Windows.UI.Core;

namespace Orientation.Orientation
{
    /// <summary>
    /// A module that allows JS to share data.
    /// </summary>
    class OrientationModule : NativeModuleBase
    {
        public ReactContext RCTContext { get; private set; }

        /// <summary>
        /// Instantiates the <see cref="OrientationModule"/>.
        /// </summary>
        internal OrientationModule(ReactContext _context)
        {
            RCTContext = _context;
            DisplayInformation.GetForCurrentView().OrientationChanged += this.OnOrientationChange;

        }

        private void OnOrientationChange(DisplayInformation sentDisplay, object e)
        {
            String orientationValue = ParseOrientation(sentDisplay.CurrentOrientation);
            if (RCTContext.HasActiveReactInstance)
            {
                RCTContext.GetJavaScriptModule<RCTDeviceEventEmitter>()
                .emit("orientationDidChange", new Dictionary<string, string>() {
                    {"orientation", orientationValue}
                });
            }
        }

        /**
         * Does not use the provided parser so we can better controll
         * the output string and make sure it matches iOS/Android
         */ 
        private string ParseOrientation(DisplayOrientations val)
        {
            switch (val)
            {
                case DisplayOrientations.Landscape:
                case DisplayOrientations.LandscapeFlipped:
                    return "LANDSCAPE";
                case DisplayOrientations.Portrait:
                case DisplayOrientations.PortraitFlipped:
                    return "PORTRAIT";
                case DisplayOrientations.None:
                    return "null";
                default:
                    return "UNKNOWN";
            }
        }

        /// <summary>
        /// The name of the native module.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Orientation";
            }
        }

        #region ReactMethods
        [ReactMethod]
        public void getOrientation(ICallback callback) {
            callback.Invoke(
                DisplayInformation.GetForCurrentView().CurrentOrientation
            );
        }

        [ReactMethod]
        public void lockToPortrait() 
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
        }

        [ReactMethod]
        public void lockToLandscape() 
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
        }

        [ReactMethod]
        public void lockToLandscapeLeft() 
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.LandscapeFlipped;
        }

        [ReactMethod]
        public void lockToLandscapeRight()
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.PortraitFlipped;
        }
        [ReactMethod]
        public void unlockAllOrientations() 
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
        }
    }
#endregion ReactMethods
}
