using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;

namespace FCViewSample.Droid
{
    [Activity(Label = "FCViewSample", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);

	        var outSize = new Android.Graphics.Point();
	        WindowManager.DefaultDisplay.GetSize(outSize);
	        var density = Resources.DisplayMetrics.Density;
	        var size = new Size(outSize.X / density, outSize.Y / density);
	        LoadApplication(new App(size));
        }
    }
}