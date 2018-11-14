using System;
using Android.Support.V7.Widget;
using Android.Widget;
using Binwell.Controls.FastCollectionView.FastCollection;

namespace Binwell.Controls.FastCollectionView.Droid.Renderers.FastCollection
{
    public class EndlessRecyclerViewScrollListener : RecyclerView.OnScrollListener
    {
        // The minimum amount of items to have below your current scroll position
        // before loading more.
	    readonly int _visibleThreshold = 5;
        // True if we are still waiting for the last set of data to load.
	    bool _loading = true;
        // The total number of items in the dataset after the last load
	    int _previousTotalItemCount;

        readonly RecyclerView.LayoutManager _layoutManager;
	    readonly Binwell.Controls.FastCollectionView.FastCollection.FastCollectionView _gridView;
	    readonly ScrollRecyclerView _recyclerView;
	    int _startScrollPosition;
	    readonly float _density;

	    public event Action LoadMore;

        public EndlessRecyclerViewScrollListener(LinearLayoutManager layoutManager, Binwell.Controls.FastCollectionView.FastCollection.FastCollectionView gridView, ScrollRecyclerView recyclerView) {
	        _layoutManager = layoutManager;
	        _gridView = gridView;
	        _recyclerView = recyclerView;
	        _density = recyclerView.Resources.DisplayMetrics.Density;
        }
		

	    public bool EnableLoadMore { get; set; }

	    public static int GetLastVisibleItem(int[] lastVisibleItemPositions)
        {
            var maxSize = 0;
            for (var i = 0; i < lastVisibleItemPositions.Length; i++)
            {
                if (i == 0 || lastVisibleItemPositions[i] > maxSize)
                {
                    maxSize = lastVisibleItemPositions[i];
                }
            }
            return maxSize;
        }

	    ScrollState _lastState = ScrollState.Idle;
	    public override void OnScrollStateChanged(RecyclerView recyclerView, int newState) {
		    base.OnScrollStateChanged(recyclerView, newState);
		    var state = (ScrollState) newState;
		    var x = _recyclerView.GetHorizontalScrollOffset() / _density;
		    var y = _startScrollPosition / _density;

		    if (_lastState == ScrollState.Idle && (state == ScrollState.TouchScroll || state == ScrollState.Fling)) {
			    _startScrollPosition = _recyclerView.GetVerticalScrollOffset();
		    }

		    if (state == ScrollState.TouchScroll || state == ScrollState.Fling) {
			    _gridView.RaiseOnStartScroll(x,y, state == ScrollState.TouchScroll ? ScrollActionType.Finger : ScrollActionType.Fling);
		    }

		    if (_lastState == ScrollState.TouchScroll && (state == ScrollState.Fling || state == ScrollState.Idle)) {
			    _gridView.RaiseOnStopScroll(x, y, ScrollActionType.Finger, state == ScrollState.Idle);
		    }

		    if (_lastState == ScrollState.Fling && state == ScrollState.Idle) {
			    _gridView.RaiseOnStopScroll(x, y, ScrollActionType.Fling, true);
		    }

		    _lastState = state;
	    }

	    // This happens many times a second during a scroll, so be wary of the code you place here.
        // We are given a few useful parameters to help us work out if we need to load some more data,
        // but first we check if we are waiting for the previous load to finish.
        public override void OnScrolled(RecyclerView view, int dx, int dy) {
			if (dy==0) return;
	        _startScrollPosition += dy;
	        _gridView.RaiseOnScroll(dy/_density, _recyclerView.GetHorizontalScrollOffset()/_density, _startScrollPosition/_density, ScrollActionType.Finger);


	        if (!EnableLoadMore) return;
            var lastVisibleItemPosition = 0;
            var totalItemCount = _layoutManager.ItemCount;

            if (_layoutManager is StaggeredGridLayoutManager manager)
            {
                var lastVisibleItemPositions = manager.FindLastVisibleItemPositions(null);
                // get maximum element within the list
                lastVisibleItemPosition = GetLastVisibleItem(lastVisibleItemPositions);
            }
            else if (_layoutManager is LinearLayoutManager)
            {
                lastVisibleItemPosition = ((LinearLayoutManager) _layoutManager).FindLastVisibleItemPosition();
            }

            // If the total item count is zero and the previous isn't, assume the
            // list is invalidated and should be reset back to initial state
            if (totalItemCount < _previousTotalItemCount)
            {
                _previousTotalItemCount = totalItemCount;
                if (totalItemCount == 0)
                {
                    _loading = true;
                }
            }
            // If it’s still loading, we check to see if the dataset count has
            // changed, if so we conclude it has finished loading and update the current page
            // number and total item count.
            if (_loading && (totalItemCount > _previousTotalItemCount))
            {
                _loading = false;
                _previousTotalItemCount = totalItemCount;
            }

            // If it isn’t currently loading, we check to see if we have breached
            // the visibleThreshold and need to reload more data.
            // If we do need to reload some more data, we execute onLoadMore to fetch the data.
            // threshold should reflect how many total columns there are too
            if (_loading || (lastVisibleItemPosition + _visibleThreshold) <= totalItemCount) return;

            LoadMore?.Invoke();
            _loading = true;
        }

        // Call this method whenever performing new searches
        public void ResetState()
        {
            _loading = true;
            _previousTotalItemCount = 0;
        }
    }
}