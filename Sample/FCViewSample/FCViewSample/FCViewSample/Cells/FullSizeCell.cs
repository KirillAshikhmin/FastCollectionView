using Binwell.Controls.FastCollectionView.FastCollection;
using Xamarin.Forms;

namespace FCViewSample.Cells
{
	public class FullSizeCell : FastCollectionCell
	{
		protected override void InitializeCell()
		{
			View = new BoxView
			{
				Color = Color.Red,
			};
		}

		protected override void SetupCell(bool isRecycled)
		{

		}
	}
}
