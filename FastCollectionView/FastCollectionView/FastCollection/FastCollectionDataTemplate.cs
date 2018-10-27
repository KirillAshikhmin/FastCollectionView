using System;
using Xamarin.Forms;

namespace Binwell.Controls.FastCollectionView.FastCollection
{
    public class FastCollectionDataTemplate : DataTemplate
    {
        public string Key { get; }
        public Size CellSize { get; }

        public FastCollectionDataTemplate(string key, Type cellType, Size cellSize): base(cellType)
        {
            Key = key;
            CellSize = cellSize;
        }
    }
}
