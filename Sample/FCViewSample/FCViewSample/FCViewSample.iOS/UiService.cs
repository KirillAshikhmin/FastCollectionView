using FCViewSample.Core;
using FCViewSample.iOS;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(UiService))]
namespace FCViewSample.iOS
{
    public class UiService : IUiService
    {
        public Size GetDisplaySize
        {
            get
            {
                var size = new Size(UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
                return size;
            }
        }
    }
}