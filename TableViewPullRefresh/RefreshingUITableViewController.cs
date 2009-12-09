using System;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using TableViewPullRefresh;

[Register]
public class RefreshingUITableViewController : UITableViewController
{
	List<string> _list;
	RefreshTableHeaderView _refreshHeaderView;

	public override void ViewDidLoad ()
	{
		base.ViewDidLoad ();
		
		_list = new List<string> {
			"Tangerine",
			"Mango",
			"Grapefruit",
			"Orange",
			"Banana"
		};
		
		_refreshHeaderView = new RefreshTableHeaderView ();
		_refreshHeaderView.BackgroundColor = new UIColor (226f, 231f, 237f, 1f);
		
		TableView.Source = new TableViewSource (this);
		TableView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
		
		TableView.AddSubview (_refreshHeaderView);
	}

	class TableViewSource : UITableViewSource
	{
		static NSString kCellIdentifier = new NSString ("MyIdentifier");

		List<string> _list;
		bool _checkForRefresh;
		bool _reloading;
		NSTimer _reloadTimer;
		RefreshTableHeaderView _refreshHeaderView;
		UITableView _table;

		public TableViewSource (RefreshingUITableViewController rtvc)
		{
			_list = rtvc._list;
			_checkForRefresh = false;
			_reloading = false;
			_refreshHeaderView = rtvc._refreshHeaderView;
			_table = rtvc.TableView;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			Console.WriteLine ("TableViewDelegate.RowSelected: Label={0}", _list[indexPath.Row]);
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			return _list.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (kCellIdentifier);
			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Default, kCellIdentifier);
			}
			cell.TextLabel.Text = _list[indexPath.Row];
			return cell;
		}

		#region UIScrollViewDelegate

		[Export("scrollViewDidScroll:")]
		public void Scrolled (UIScrollView scrollView)
		{
			if (_checkForRefresh) {
				if (_refreshHeaderView.isFlipped && (_table.ContentOffset.Y > -65f) && (_table.ContentOffset.Y < 0f) && !_reloading) {
					_refreshHeaderView.FlipImageAnimated (true);
					_refreshHeaderView.SetStatus (TableViewPullRefresh.RefreshTableHeaderView.RefreshStatus.PullToReloadStatus);
				} else if ((!_refreshHeaderView.isFlipped) && (_table.ContentOffset.Y < -65f)) {
					_refreshHeaderView.FlipImageAnimated (true);
					_refreshHeaderView.SetStatus (TableViewPullRefresh.RefreshTableHeaderView.RefreshStatus.ReleaseToReloadStatus);
				}
			}
		}

		[Export("scrollViewWillBeginDragging:")]
		public void DraggingStarted (UIScrollView scrollView)
		{
			_checkForRefresh = true;
		}

		[Export("scrollViewDidEndDragging:willDecelerate:")]
		public void DraggingEnded (UIScrollView scrollView, bool willDecelerate)
		{
			if (_table.ContentOffset.Y <= -65f) {
				
				//ReloadTimer = NSTimer.CreateRepeatingScheduledTimer (new TimeSpan (0, 0, 0, 10, 0), () => dataSourceDidFinishLoadingNewData ());
				_reloadTimer = NSTimer.CreateScheduledTimer (TimeSpan.FromSeconds (2f), delegate {
					// for this demo I cheated and am just going to pretend data is reloaded
					// in real world use this function to really make sure data is reloaded
					
					_reloadTimer = null;
					Console.WriteLine ("dataSourceDidFinishLoadingNewData() called from NSTimer");
					
					_reloading = false;
					_refreshHeaderView.FlipImageAnimated (false);
					_refreshHeaderView.ToggleActivityView ();
					UIView.BeginAnimations ("DoneReloadingData");
					UIView.SetAnimationDuration (0.3);
					_table.ContentInset = new UIEdgeInsets (0f, 0f, 0f, 0f);
					_refreshHeaderView.SetStatus (TableViewPullRefresh.RefreshTableHeaderView.RefreshStatus.PullToReloadStatus);
					UIView.CommitAnimations ();
					_refreshHeaderView.SetCurrentDate ();
				});
				
				_reloading = true;
				_table.ReloadData ();
				_refreshHeaderView.ToggleActivityView ();
				UIView.BeginAnimations ("ReloadingData");
				UIView.SetAnimationDuration (0.2);
				_table.ContentInset = new UIEdgeInsets (60f, 0f, 0f, 0f);
				UIView.CommitAnimations ();
			}
			
			_checkForRefresh = false;
		}
		
		#endregion
		
	}
}
