//  Based on https://github.com/twintechs/TwinTechsFormsLib
//  Special thanks to Twin Technologies from Binwell Ltd.

//  Distributed under Apache 2.0 License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Binwell.Controls.FastCollectionView.FastCollection;
using Binwell.Controls.FastCollectionView.iOS.FastCollection;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FastCollectionView), typeof(FastCollectionViewRenderer))]
namespace Binwell.Controls.FastCollectionView.iOS.FastCollection
{
    public class FastCollectionViewRenderer : ViewRenderer<FastCollectionView.FastCollection.FastCollectionView, FastUiCollectionView>, IGridViewProvider
    {
	    readonly List<string> _cellTypes = new List<string>();
	    FastUiCollectionView _fastUiCollectionView;
	    NSIndexPath _initialIndex;
	    UIRefreshControl _refreshControl;

        public static void Init()
        {
            var temp = DateTime.Now;
        }

        public IList Source
        {
            get => _source;
	        private set {
		        if (_source is INotifyCollectionChanged oldCollection)
                {
                    oldCollection.CollectionChanged -= DataCollectionChanged;
                }
                _source = value;
		        if (_source is INotifyCollectionChanged newCollection)
                {
                    newCollection.CollectionChanged += DataCollectionChanged;
                }
            }
        }

	    bool _isAllowLoadMore = true;
	    bool _loadMoreEnabled;
	    IList _source;

        protected override void OnElementChanged(ElementChangedEventArgs<FastCollectionView.FastCollection.FastCollectionView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null) return;

            e.NewElement.GridViewProvider = this;
            _loadMoreEnabled = e.NewElement.LoadMoreCommand != null;

            _fastUiCollectionView = new FastUiCollectionView
            {
                AllowsMultipleSelection = false,
                SelectionEnable = e.NewElement.SelectionEnabled,
                BackgroundColor = Element.BackgroundColor.ToUIColor(),
                RowSpacing = Element.RowSpacing,
                ColumnSpacing = Element.ColumnSpacing,
                ContentInset = new UIEdgeInsets((nfloat) Element.ContentPaddingTop, (nfloat) Element.ContentPaddingLeft,
                        (nfloat) Element.ContentPaddingBottom, (nfloat) Element.ContentPaddingRight),
                ShowsHorizontalScrollIndicator = false,
                AlwaysBounceVertical = !e.NewElement.IsHorizontal,
                CanCancelContentTouches = false,
                Frame = Bounds,
				Bounds = Bounds
            };


            var flowLayout = (UICollectionViewFlowLayout) _fastUiCollectionView.CollectionViewLayout;

            if (flowLayout != null)
            {
                if (e.NewElement.IsHorizontal)
                    flowLayout.ScrollDirection = UICollectionViewScrollDirection.Horizontal;

                flowLayout.SectionInset = new UIEdgeInsets((nfloat) Element.SectionPaddingTop, (nfloat)Element.SectionPaddingLeft,
                    (nfloat) Element.SectionPaddingBottom, 0);
            }

            if (e.NewElement.IsPullToRefreshEnabled)
            {
                _refreshControl = new UIRefreshControl();
                _refreshControl.AddTarget(this, new Selector(@"pullToRefresh"), UIControlEvent.ValueChanged);
                _fastUiCollectionView.AddSubview(_refreshControl);
            }

            Unbind(e.OldElement);

			Source = e.NewElement.ItemsSource as IList;
			RegisterCellTypes(e.NewElement.ItemTemplateSelector);
            SetDataSource(_fastUiCollectionView);
            _fastUiCollectionView.WeakDelegate = new FastUiCollectionViewDelegate(HandleOnScrolled, GetSizeForItem, HandleScrollStarted, HandleScrollEnded);
            _fastUiCollectionView.ReloadData();
            _fastUiCollectionView.ScrollEnabled = Element.IsScrollEnabled;
            ScrollToInitialIndex();

            SetNativeControl(_fastUiCollectionView);
	        e.NewElement.GetScrollPositionCommand = new Command(GetScrollPosition);
        }

	    void GetScrollPosition(object obj) {
		    var func = obj as Func<Point, object>;
		    if (func == null) return;
		    var offset = _fastUiCollectionView.ContentOffset;
		    var point = new Point(offset.X, offset.Y);
		    func.Invoke(point);

	    }

	    void SetDataSource(UICollectionView collectionView)
        {
            if (collectionView == null) return;

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                collectionView.PrefetchingEnabled = false;

            collectionView.WeakDataSource = new FastCollectionDataSource(GetCell, RowsInSection, NumberOfSections);
        }

        [Export("pullToRefresh")]
        // ReSharper disable once UnusedMember.Local
        void Refresh()
        {
            if (Element != null && Element.IsPullToRefreshEnabled)
                Element?.Refresh();
        }

	    CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath) {
		    var templateSelector = Element.ItemTemplateSelector;
            if (Source == null || Source.Count < indexPath.Row || templateSelector==null) return CGSize.Empty;
			var item = Source[indexPath.Row];
		    if (item == null) return CGSize.Empty;
		    var key = templateSelector.GetKey(item) ?? item.GetType().Name;
	        var size = templateSelector.GetSizesByKey(key);

			return new CGSize(size.Width, size.Height);
        }

	    void RegisterCellTypes(FastCollectionTemplateSelector templateSelector) {
		    if (templateSelector!=null)
			    foreach (var template in templateSelector.DataTemplates) {
			    var contain = _cellTypes.Contains(template.Key);
			    if (contain) continue;
			    _fastUiCollectionView.RegisterClassForCell(typeof(FastUiCollectionViewCell), new NSString(template.Key));
			    _cellTypes.Add(template.Key);
		    }
	    }

	    protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var gridView = sender as FastCollectionView.FastCollection.FastCollectionView;
	        if (!(_fastUiCollectionView?.CollectionViewLayout is UICollectionViewFlowLayout flowLayout)) return;
            switch (e.PropertyName)
            {
                case "ItemTemplate":
                case "ItemsSource":
                    if (gridView?.ItemTemplateSelector == null) break;
                    Source = gridView.ItemsSource as IList;
                    RegisterCellTypes(gridView.ItemTemplateSelector);
                    _fastUiCollectionView.ReloadData();
                    ScrollToInitialIndex();
                    break;
                case "LoadMoreCommand":
                    _loadMoreEnabled = Element?.LoadMoreCommand != null;
                    break;
                case "IsScrollEnabled":
                    Device.BeginInvokeOnMainThread(() => _fastUiCollectionView.ScrollEnabled = Element.IsScrollEnabled);
                    break;
                case "IsHorizontal":
                    flowLayout.ScrollDirection = Element.IsHorizontal
                        ? UICollectionViewScrollDirection.Horizontal
                        : UICollectionViewScrollDirection.Vertical;
                    break;
                case "IsPullToRefreshEnabled":
                        if (_refreshControl != null) break;
                        _refreshControl = new UIRefreshControl();
                        _refreshControl.AddTarget(this, new Selector(@"pullToRefresh"), UIControlEvent.ValueChanged);
                        _fastUiCollectionView.AddSubview(_refreshControl);
                    break;
                case "ContentPaddingLeft":
                case "ContentPaddingTop":
                case "ContentPaddingBottom":
                case "ContentPaddingRight":
                    _fastUiCollectionView.ContentInset = new UIEdgeInsets((float) Element.ContentPaddingTop,
                        (float) Element.ContentPaddingLeft, (float) Element.ContentPaddingBottom,
                        (float) Element.ContentPaddingRight);
                    break;
                case "SectionPaddingLeft":
                case "SectionPaddingBottom":
                case "SectionPaddingTop":
                    flowLayout.SectionInset = new UIEdgeInsets((float)Element.SectionPaddingTop, (float)Element.SectionPaddingLeft,
                        (float)Element.SectionPaddingBottom, 0);
                    break;
                case "IsRefreshing":
                    if (_refreshControl != null && Element != null)
                    {
                        if (Element.IsRefreshing && ! _refreshControl.Refreshing)
                            _refreshControl.BeginRefreshing();
                        else if (!Element.IsRefreshing && _refreshControl.Refreshing)
                            _refreshControl.EndRefreshing();
                    }
                    break;
                case "Width":
                case "Height":
					_fastUiCollectionView.Frame = new CGRect(Element.X, Element.Y, Math.Abs(Element.Width) < 0.1 ? Bounds.Width : Element.Width, Math.Abs(Element.Height) < 0.1 ? Bounds.Height : Element.Height);
					break;
            }
        }

	    void Unbind(FastCollectionView.FastCollection.FastCollectionView oldElement)
        {
            if (oldElement == null) return;

	        if (oldElement.ItemsSource is INotifyCollectionChanged itemsSource)
                itemsSource.CollectionChanged -= DataCollectionChanged;
        }


	    readonly object _lock = new object();

	    void DataCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
		    if (_fastUiCollectionView == null) return;
		    lock (_lock) {

			    void Change() {
				    int count;
				    var paths = new List<NSIndexPath>();
				    switch (e.Action) {
					    case NotifyCollectionChangedAction.Add:
						    if (e.NewItems == null) return;
						    count = e.NewItems.Count;

						    if (count == 0) return;
						    for (var i = 0; i < count; i++) {
							    paths.Add(NSIndexPath.FromRowSection(e.NewStartingIndex + i, 0));
						    }

						    _fastUiCollectionView.InsertItems(paths.ToArray());
						    break;
					    case NotifyCollectionChangedAction.Remove:
						    if (e.OldItems == null) return;
						    count = e.OldItems.Count;
						    if (count == 0) return;

						    for (var i = 0; i < count; i++) {
							    paths.Add(NSIndexPath.FromRowSection(e.OldStartingIndex + i, 0));
						    }

						    _fastUiCollectionView.DeleteItems(paths.ToArray());
						    break;
					    case NotifyCollectionChangedAction.Replace:
						    count = e.NewItems.Count;
						    if (count == 0) return;
						    for (var i = 0; i < count; i++) {
							    paths.Add(NSIndexPath.FromRowSection(e.OldStartingIndex + i, 0));
						    }

						    _fastUiCollectionView.ReloadItems(paths.ToArray());
						    break;
					    case NotifyCollectionChangedAction.Move:
						    _fastUiCollectionView.MoveItem(NSIndexPath.Create(e.OldStartingIndex), NSIndexPath.Create(e.NewStartingIndex));
						    break;
					    case NotifyCollectionChangedAction.Reset:
						    _fastUiCollectionView.ReloadData();
						    break;
				    }
			    }
			    if (Element.CollectionChangedWithoutAnimation) PerformWithoutAnimation(Change);
			    else Change();
		    }
	    }

	    void HandleScrollStarted(CGPoint contentOffset, ScrollActionType scrollActionType) {
		    Element.RaiseOnStartScroll(contentOffset.X, contentOffset.Y, scrollActionType);
	    }

	    void HandleScrollEnded(CGPoint contentOffset, ScrollActionType scrollActionType, bool fullStop) {
		    Element.RaiseOnStopScroll(contentOffset.X,contentOffset.Y, scrollActionType, fullStop);
	    }

	    void HandleOnScrolled(CGPoint contentOffset, ScrollActionType type)
        {
            if (Element == null) return;
            Element.RaiseOnScroll(contentOffset.X, contentOffset.X, contentOffset.Y, type);

            if (Control != null && !Control.IsFirstResponder)
            {
                Control.BecomeFirstResponder();
                UIApplication.SharedApplication.SendAction(new Selector(@"resignFirstResponder"), null, null, null);
            }

            if (!_loadMoreEnabled || _fastUiCollectionView == null) return;


            if (Element.IsHorizontal || _fastUiCollectionView.ContentSize.Height < _fastUiCollectionView.Bounds.Height || contentOffset.Y==-1) return;

            var dy = _fastUiCollectionView.ContentSize.Height - _fastUiCollectionView.Bounds.Height - contentOffset.Y;

            if (dy > 40 && !_isAllowLoadMore)
                _isAllowLoadMore = true;

            if (dy >= 30 || !_isAllowLoadMore) return;

            _isAllowLoadMore = false;
            Element.RaiseLoadMoreEvent();
        }

	    void ScrollToInitialIndex()
        {
            if (_fastUiCollectionView?.DataSource == null) return;

            ScrollToItem(0, false);
            _initialIndex = null;
        }

        public int RowsInSection(UICollectionView collectionView, nint sectionNumber)
        {
            if (Element?.ItemTemplateSelector == null) return 0;
            var list = Source as ICollection;
            var count = list?.Count;
            return count ?? 0;
        }

	    int NumberOfSections(UICollectionView collectionView)
        {
            if (Element?.ItemTemplateSelector == null) return 0;
            return Source != null ? 1 : 0;
        }

        public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath) {

            var item = Source?[indexPath.Row];
            if (item == null) return null;

			var templateSelector = Element.ItemTemplateSelector;
	        if (templateSelector == null) return null;
			var key = templateSelector.GetKey(item);
	        try {
				if (!(collectionView.DequeueReusableCell(new NSString(key), indexPath) is FastUiCollectionViewCell collectionCell)) {
			        return null;
		        }

		        collectionCell.RecycleCell(item, Element?.ItemTemplateSelector, Element);

		        if (collectionView is FastUiCollectionView gridCollectionView && gridCollectionView.SelectionEnable && collectionCell.CellTapped == null)
			        collectionCell.CellTapped = CellTapped;

		        return collectionCell;
	        }
	        catch (Exception e) {
		        throw new NotSupportedException($@"Check the key ""{key}"" is declared in FastGridTemplateSelector", e);
	        }
        }



	    void CellTapped(object obj)
        {
            Element?.InvokeItemSelectedEvent(this, obj);
        }

        public void ReloadData()
        {
            if (_fastUiCollectionView != null)
                InvokeOnMainThread(_fastUiCollectionView.ReloadData);

        }

        public void ScrollToItem(int row, bool animated)
        {
			ScrollToItem(row, animated, UICollectionViewScrollPosition.None);
        }

	    public void ScrollTo(float x, float y) {
			_fastUiCollectionView?.SetContentOffset(new CGPoint(x,y), true);

		}

	    public void ScrollToItem(int row, bool animated, UICollectionViewScrollPosition position)
		{
			var indexPath = NSIndexPath.FromRowSection(row, 0);
			if (_fastUiCollectionView != null && _fastUiCollectionView.NumberOfSections() > 0 &&
				_fastUiCollectionView.NumberOfItemsInSection(0) > 0)
				InvokeOnMainThread(
					() => _fastUiCollectionView?.ScrollToItem(indexPath, position, animated));
			else
				_initialIndex = indexPath;
		}

        protected override void Dispose(bool disposing)
        {
            _refreshControl?.RemoveTarget(this, new Selector(@"pullToRefresh"), UIControlEvent.ValueChanged);
            _refreshControl?.Dispose();
            _refreshControl = null;

            _initialIndex?.Dispose();

            _fastUiCollectionView = null;
            Source = null;

            base.Dispose(disposing);
        }

        public int GetVisibleItemsCount()
        {
			var firstCell = _fastUiCollectionView.VisibleCells.FirstOrDefault();
            if (firstCell == null) return 0;
			var first = _fastUiCollectionView.IndexPathForCell(firstCell);

			// TODO Improve for support multiple cell types
			var size = GetSizeForItem(_fastUiCollectionView, null, first);
			var cnt = (int) (_fastUiCollectionView.Bounds.Height / size.Height)-1;
            return cnt;
        }
    }
}