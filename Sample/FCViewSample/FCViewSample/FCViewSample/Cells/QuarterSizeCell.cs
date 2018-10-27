using Binwell.Controls.FastCollectionView.FastCollection;
using Xamarin.Forms;

namespace FCViewSample.Cells
{
	class QuarterSizeCell : FastCollectionCell
	{
		protected override void InitializeCell()
		{
			View = new BoxView
			{
				Color = Color.RoyalBlue,
			};
		}

		protected override void SetupCell(bool isRecycled)
		{

		}
	}
}
