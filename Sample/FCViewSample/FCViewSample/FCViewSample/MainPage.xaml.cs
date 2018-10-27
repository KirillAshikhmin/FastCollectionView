using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Binwell.Controls.FastCollectionView;
using Xamarin.Forms;

namespace FCViewSample
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		void GoToSample1(object sender, EventArgs e)
		{
			Navigation.PushAsync(new Sample1Page());
		}
	}
}
