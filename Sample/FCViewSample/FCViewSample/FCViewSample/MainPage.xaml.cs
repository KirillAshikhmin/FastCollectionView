using Binwell.Controls.FastCollectionView.FastCollection;
using FCViewSample.Core.Cells;
using FCViewSample.Core.DataObjects;
using Xamarin.Forms;

namespace FCViewSample.Core
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            var size = DependencyService.Get<IUiService>().GetDisplaySize;

            collectionView.ItemTemplateSelector = new FastCollectionTemplateSelector().Add(
                new FastCollectionDataTemplate(typeof(CategoryObject).Name, typeof(CategoryCell),new Size(size.Width, 70)),
                new FastCollectionDataTemplate(typeof(ProductObject).Name, typeof(ProductViewCell),new Size(size.Width / 2 - 1, 260))
            );

            BindingContext = new MainViewModel(this);
        }
    }
}