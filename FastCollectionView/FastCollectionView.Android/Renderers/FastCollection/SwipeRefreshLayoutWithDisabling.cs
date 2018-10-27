using Android.Content;
using Android.Support.V4.Widget;

namespace Binwell.Controls.FastCollectionView.Droid.Renderers.FastCollection
{
	public class SwipeRefreshLayoutWithDisabling : SwipeRefreshLayout
	{

		public SwipeRefreshLayoutWithDisabling(Context context) : base(context)
		{
		}

		public bool IsPullToRefreshEnabled { get; set; }

		public override bool CanChildScrollUp()
		{
			if (!IsPullToRefreshEnabled) return true;
			return base.CanChildScrollUp();
		}
	}
}
