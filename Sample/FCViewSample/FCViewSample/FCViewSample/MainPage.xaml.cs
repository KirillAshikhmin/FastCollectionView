using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Binwell.Controls.FastCollectionView.FastCollection;
using FCViewSample.Cells;
using FCViewSample.DataObjects;
using Xamarin.Forms;

namespace FCViewSample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            var size = DependencyService.Get<IUiService>().GetDisplaySize;

            var templateSelector = new FastCollectionTemplateSelector();
            templateSelector.DataTemplates.AddRange(
                new List<FastCollectionDataTemplate>()
                {
                    new FastCollectionDataTemplate(typeof(CategoryObject).Name, typeof(CategoryCell),
                        new Size(size.Width, 70)),
                    new FastCollectionDataTemplate(typeof(ProductObject).Name, typeof(ProductViewCell),
                        new Size(size.Width / 2 - 1, 220))
                });
            templateSelector.Prepare();
            collectionView.ItemTemplateSelector = templateSelector;
            GenerateSource();
            collectionView.RefreshCommand = new Command(RefreshExecute);
            collectionView.ItemSelectedCommand=new Command(ItemSelectedExecute);
        }

        private void ItemSelectedExecute(object obj)
        {
            if (obj is ProductObject product) DisplayAlert("Selected item", product.Name, "Ok");
        }

        private async void RefreshExecute(object obj)
        {
            collectionView.IsRefreshing = true;
            await Task.Delay(3000);
            GenerateSource();
            collectionView.IsRefreshing = false;
        }

        private void GenerateSource()
        {
            var size = DependencyService.Get<IUiService>().GetDisplaySize;

            var imageSize = (size.Width / 2 - 40) * 2;

            var imageUrl = $"https://loremflickr.com/{imageSize}/{imageSize}/";
            var r = new Random(DateTime.Now.Millisecond);

            string getImage(string name)
            {
                return $"{imageUrl}{name}?random={r.Next()}";
            }

            var items = new List<object>
            {
                new CategoryObject {Name = "Фрукты"},
                new ProductObject()
                {
                    ImageUrl = getImage("pear"),
                    Name = "Груши",
                    Price = "120 руб"
                },
                new ProductObject()
                {
                    ImageUrl = getImage("apple"),
                    Name = "Яблоки",
                    Price = "50 руб"
                },
                new ProductObject()
                {
                    ImageUrl = getImage("banana"),
                    Name = "Бананы",
                    Price = "55 руб"
                },
                new ProductObject()
                {
                    ImageUrl = getImage("orange"),
                    Name = "Апельсины",
                    Price = "89 руб"
                },
                new CategoryObject {Name = "Овощи"},
                new ProductObject()
                {
                    ImageUrl = getImage("tomato"),
                    Name = "Помидоры",
                    Price = "110 руб."
                },
                new ProductObject()
                {
                    ImageUrl = getImage("cucumber"),
                    Name = "Огурцы",
                    Price = "900 руб."
                },
                new ProductObject()
                {
                    ImageUrl = getImage("Eggplant"),
                    Name = "Баклажаны",
                    Price = "280 руб."
                },
                new ProductObject()
                {
                    ImageUrl = getImage("Pumpkin"),
                    Name = "Тыква",
                    Price = "40 руб."
                },
            };
            collectionView.ItemsSource = items;
        }
    }
}