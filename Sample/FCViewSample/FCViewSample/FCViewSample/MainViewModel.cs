using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using FCViewSample.Core.DataObjects;
using MvvmHelpers;
using Xamarin.Forms;

namespace FCViewSample.Core
{
    public class MainViewModel : BaseViewModel
    {
        readonly ContentPage _page;
        ObservableRangeCollection<object> _itemsSource = new ObservableRangeCollection<object>();
        ICommand _loadMoreCommand;
        ICommand _refreshCommand;
        ICommand _itemSelectedCommand;

        public ObservableRangeCollection<object> ItemsSource
        {
            get => _itemsSource;
            set
            {
                _itemsSource = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(ContentPage page)
        {
            _page = page;
            GenerateSource();
        }

        public ICommand LoadMoreCommand => _loadMoreCommand ?? (_loadMoreCommand = new Command(async () => await LoadMoreCommandAsync()));

        async Task LoadMoreCommandAsync()
        {
            IsBusy = true;
            await Task.Delay(3000);
            GenerateSource();
            IsBusy = false;
        }
        public ICommand RefreshCommand =>_refreshCommand ?? (_refreshCommand = new Command(async () => await RefreshCommandAsync()));

        async Task RefreshCommandAsync()
        {
            IsBusy = true;
            await Task.Delay(3000);
            ItemsSource.Clear();
            await Task.Delay(150);
            GenerateSource();
            IsBusy = false;
        }
        public ICommand ItemSelectedCommand => _itemSelectedCommand ?? (_itemSelectedCommand = new Command(async (o) => await ItemSelectedCommandAsync(o)));

        async Task ItemSelectedCommandAsync(object obj)
        {
            if (obj is ProductObject product) await _page.DisplayAlert("Selected item", product.Name, "Ok");
        }

        void GenerateSource()
        {
            var size = DependencyService.Get<IUiService>().GetDisplaySize;

            var imageSize = (int)((size.Width / 2 - 40) * 2);

            var imageUrl = $"https://loremflickr.com/{imageSize}/{imageSize}/";
            var r = new Random(DateTime.Now.Millisecond);

            string GetImage(string name)
            {
                return $"{imageUrl}{name}?random={r.Next()}";
            }

            var items = new List<object>
            {
                new CategoryObject {Name = "Фрукты"},
                new ProductObject()
                {
                    ImageUrl = GetImage("pear"),
                    Name = "Груши",
                    Price = "120 руб"
                },
                new ProductObject()
                {
                    ImageUrl = GetImage("apple"),
                    Name = "Яблоки",
                    Price = "50 руб"
                },
                new ProductObject()
                {
                    ImageUrl = GetImage("banana"),
                    Name = "Бананы",
                    Price = "55 руб"
                },
                new ProductObject()
                {
                    ImageUrl = GetImage("orange"),
                    Name = "Апельсины",
                    Price = "89 руб"
                },
                new CategoryObject {Name = "Овощи"},
                new ProductObject()
                {
                    ImageUrl = GetImage("tomato"),
                    Name = "Помидоры",
                    Price = "110 руб."
                },
                new ProductObject()
                {
                    ImageUrl = GetImage("cucumber"),
                    Name = "Огурцы",
                    Price = "900 руб."
                },
                new ProductObject()
                {
                    ImageUrl = GetImage("Eggplant"),
                    Name = "Баклажаны",
                    Price = "280 руб."
                },
                new ProductObject()
                {
                    ImageUrl = GetImage("Pumpkin"),
                    Name = "Тыква",
                    Price = "40 руб."
                },
            };
            ItemsSource.AddRange(items);
        }
    }
}
