using Binwell.Controls.FastCollectionView.FastCollection;
using Xamarin.Forms;

namespace FCViewSample.Cells
{
	public class SemiSizeCell : FastCollectionCell
	{
		protected override void InitializeCell()
		{
			View = new BoxView
			{
				Color = Color.YellowGreen,
			};
		}

		protected override void SetupCell(bool isRecycled)
		{

		}
	}
}
