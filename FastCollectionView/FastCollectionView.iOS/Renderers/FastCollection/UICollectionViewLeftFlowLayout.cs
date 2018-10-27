using System;
using CoreGraphics;
using UIKit;

namespace Binwell.Controls.FastCollectionView.iOS.Renderers.FastCollection
{
	public class UiCollectionViewLeftFlowLayout : UICollectionViewFlowLayout
	{
		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
		{
			var attributes = base.LayoutAttributesForElementsInRect(rect);

			if (ScrollDirection == UICollectionViewScrollDirection.Horizontal) return attributes;

			var maxY = -1.0;
			var leftMargin = SectionInset.Left;
			foreach (var layoutAttribute in attributes)
			{
				if (layoutAttribute.Frame.Y >= maxY)
				{
					leftMargin = SectionInset.Left;
				}

				var frame = layoutAttribute.Frame;
				frame.X = leftMargin;
				layoutAttribute.Frame = frame;


				leftMargin += layoutAttribute.Frame.Width;
				maxY = Math.Max(layoutAttribute.Frame.GetMaxY(), maxY);
			}

			return attributes;
		}
	}
}