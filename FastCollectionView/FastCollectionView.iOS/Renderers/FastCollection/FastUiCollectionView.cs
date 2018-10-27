//  Based on https://github.com/twintechs/TwinTechsFormsLib
//  Special thanks to Twin Technologies from Binwell Ltd.

//  Distributed under Apache 2.0 License: http://www.apache.org/licenses/LICENSE-2.0

using CoreGraphics;
using UIKit;

namespace Binwell.Controls.FastCollectionView.iOS.Renderers.FastCollection
{
    public sealed class FastUiCollectionView : UICollectionView
    {
        public bool SelectionEnable { get; set; }

        public FastUiCollectionView() : this(default(CGRect))
        {
        }

        public FastUiCollectionView(CGRect rect) : base(rect, new UiCollectionViewLeftFlowLayout())
        {
			AutoresizingMask = UIViewAutoresizing.None;
            ContentMode = UIViewContentMode.ScaleAspectFill;
        }
		
        public double RowSpacing
        {
            get => (CollectionViewLayout as UICollectionViewFlowLayout)?.MinimumLineSpacing ?? 0;
	        set
            {
	            if (CollectionViewLayout is UICollectionViewFlowLayout layout) layout.MinimumLineSpacing = (float) value;
            }
        }

        public double ColumnSpacing
        {
            get => (CollectionViewLayout as UICollectionViewFlowLayout)?.MinimumInteritemSpacing ?? 0;
	        set
            {
	            if (CollectionViewLayout is UICollectionViewFlowLayout layout) layout.MinimumInteritemSpacing = (float) value;
            }
        }
		
    }
}