using Binwell.Controls.FastCollectionView.FastCollection;
using Xamarin.Forms.Xaml;

namespace FCViewSample.Core.Cells
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CategoryCell : FastCollectionCell
    {
        protected override void InitializeCell()
        {
            InitializeComponent();
        }

        protected override void SetupCell(bool isRecycled)
        {
        }
    }
}