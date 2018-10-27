using System.Collections.Generic;
using Binwell.Controls.FastCollectionView.FastCollection;
using FCViewSample.Cells;
using Xamarin.Forms;

namespace FCViewSample
{
	public class Sample1Page : ContentPage
	{
		public Sample1Page()
		{
			Title = "Sample 1";
			InitializeUi();
		}

		void InitializeUi()
		{
			var collectionView = new FastCollectionView()
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};
			var templateSelector = new FastCollectionTemplateSelector();
			templateSelector.DataTemplates.AddRange(
				new List<FastCollectionDataTemplate>()
				{
					new FastCollectionDataTemplate(typeof(bool).Name, typeof(FullSizeCell), new Size(App.ScreenSize.Width, 100)),
					new FastCollectionDataTemplate(typeof(string).Name, typeof(SemiSizeCell), new Size(App.ScreenSize.Width/2, 100)),
					new FastCollectionDataTemplate(typeof(int).Name, typeof(QuarterSizeCell), new Size(App.ScreenSize.Width/4, 100))
				});
			templateSelector.Prepare();
			collectionView.ItemTemplateSelector = templateSelector;
			var items = new List<object>
			{
				true,
				"one",
				"two",
				1,
				2,
				3,
				4
			};
			collectionView.ItemsSource = items;
			Content = collectionView;
		}
	}
}