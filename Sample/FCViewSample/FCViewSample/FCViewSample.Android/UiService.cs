﻿using FCViewSample.Core;
using FCViewSample.Droid;
using Plugin.CurrentActivity;
using Xamarin.Forms;

[assembly: Dependency(typeof(UiService))]
namespace FCViewSample.Droid
{
    public class UiService : IUiService
    {
        public Size GetDisplaySize
        {
            get
            {
                var activity = CrossCurrentActivity.Current.Activity;
                var outSize = new Android.Graphics.Point();
                activity.WindowManager.DefaultDisplay.GetSize(outSize);
                var density = activity.Resources.DisplayMetrics.Density;
                var size = new Size(outSize.X / density, outSize.Y / density);
                return size;
            }
        }
    }
}