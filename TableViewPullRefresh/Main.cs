/*
Copyright (c) 2009, Martin R. Bowling

Betterified by Chris Hardy (in other words he made it work =P)

Copying and distribution of this file, with or without
modification, are permitted in any medium without royalty.
This file is offered as-is, without any warranty.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace TableViewPullRefresh
{
    public class Application
    {
        static void Main (string[] args)
        {
            UIApplication.Main (args, null, "AppController");
        }
    }

    [Register ("AppController")]
    public class AppController : UIApplicationDelegate
    {
        private UIWindow window;

        public override bool FinishedLaunching (
            UIApplication app, NSDictionary options)
        {
            // Create the main view controller
            var vc = new MainViewController ();

            // Create the main window and add main view
            // controller as a subview
            window = new UIWindow (UIScreen.MainScreen.Bounds);
            window.AddSubview(vc.View);

            window.MakeKeyAndVisible ();
            return true;
        }

        // This method is allegedly required in iPhoneOS 3.0
        public override void OnActivated (
            UIApplication application)
        {
        }
    }

    [Register]
    public class MainViewController : UIViewController
    {
        private UITableView tableView;
        private List<string> list;
		
		private bool checkForRefresh;
		private bool reloading;
		private RefreshTableHeaderView refreshHeaderView;
		private NSTimer ReloadTimer;
		
		// add this string to the extra arguments for mtouch
		// -gcc_flags "-framework QuartzCore -L${ProjectDir} -lAdMobSimulator3_0 -ObjC"

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
			
			checkForRefresh = false;
			reloading = false;

            list = new List<string>()
            {
                "Tangerine",
                "Mango",
                "Grapefruit",
                "Orange",
                "Banana"
            };

            tableView = new UITableView()
            {
                Delegate = new TableViewDelegate(list),
                DataSource = new TableViewDataSource(list),
                AutoresizingMask =
                    UIViewAutoresizing.FlexibleHeight|
                    UIViewAutoresizing.FlexibleWidth,
                //BackgroundColor = UIColor.Yellow,
            };

            // Set the table view to fit the width of the app.
            tableView.SizeToFit();

            // Reposition and resize the receiver
            tableView.Frame = new RectangleF (
                0, 0, this.View.Frame.Width,
                this.View.Frame.Height);

			RefreshTableHeaderView refreshHeaderView = new RefreshTableHeaderView();
			refreshHeaderView.BackgroundColor = new UIColor (226.0f,231.0f,237.0f,1.0f);
			tableView.AddSubview (refreshHeaderView);
            // Add the table view as a subview
            this.View.AddSubview(tableView);
			
			tableView.DraggingStarted += delegate { 
				checkForRefresh = true; 	 
			};
			
			tableView.Scrolled += delegate(object sender, EventArgs e) {
				
				if (checkForRefresh) {
					if (refreshHeaderView.isFlipped && (tableView.ContentOffset.Y > - 65.0f) && (tableView.ContentOffset.Y < 0.0f) && !reloading)
					{
						refreshHeaderView.flipImageAnimated (true);
						refreshHeaderView.setStatus (TableViewPullRefresh.RefreshTableHeaderView.RefreshStatus.PullToReloadStatus);
					}
					else if ((!refreshHeaderView.isFlipped) && (this.tableView.ContentOffset.Y < -65.0f))
					{	
						refreshHeaderView.flipImageAnimated (true);
						refreshHeaderView.setStatus(TableViewPullRefresh.RefreshTableHeaderView.RefreshStatus.ReleaseToReloadStatus );
					}
				}
			};
	
			tableView.DraggingEnded += delegate(object sender, EventArgs e) {

				if (this.tableView.ContentOffset.Y <= -65.0f){
					
					//ReloadTimer = NSTimer.CreateRepeatingScheduledTimer (new TimeSpan (0, 0, 0, 10, 0), () => dataSourceDidFinishLoadingNewData ());
					ReloadTimer = NSTimer.CreateScheduledTimer (new TimeSpan (0, 0, 0, 5, 0),
					                                            delegate { 
										// for this demo I cheated and am just going to pretend data is reloaded
										// in real world use this function to really make sure data is reloaded
										
										ReloadTimer = null;
										Console.WriteLine ("dataSourceDidFinishLoadingNewData() called from NSTimer");
			
										reloading = false;
										refreshHeaderView.flipImageAnimated (false);
										refreshHeaderView.toggleActivityView();
										UIView.BeginAnimations("DoneReloadingData");
										UIView.SetAnimationDuration(0.3);
										tableView.ContentInset = new UIEdgeInsets (0.0f,0.0f,0.0f,0.0f);
										refreshHeaderView.setStatus(TableViewPullRefresh.RefreshTableHeaderView.RefreshStatus.PullToReloadStatus);
										UIView.CommitAnimations();
										refreshHeaderView.setCurrentDate();
					});
					 
					reloading = true;
					tableView.ReloadData();
					refreshHeaderView.toggleActivityView();
					UIView.BeginAnimations("ReloadingData");
					UIView.SetAnimationDuration(0.2);
					this.tableView.ContentInset = new UIEdgeInsets (60.0f,0.0f,0.0f,0.0f);
					UIView.CommitAnimations();
				}
				
				checkForRefresh = false;
				
			};

        }
		

        private class TableViewDelegate : UITableViewDelegate
        {
            private List<string> list;

            public TableViewDelegate(List<string> list)
            {
                this.list = list;
            }

            public override void RowSelected (
                UITableView tableView, NSIndexPath indexPath)
            {
                Console.WriteLine(
                    "TableViewDelegate.RowSelected: Label={0}",
                     list[indexPath.Row]);
            }
        }

        private class TableViewDataSource : UITableViewDataSource
        {
            static NSString kCellIdentifier =
                new NSString ("MyIdentifier");
            private List<string> list;

            public TableViewDataSource (List<string> list)
            {
                this.list = list;
            }
		
            public override int RowsInSection (
                UITableView tableview, int section)
            {
                return list.Count;
            }

			
            public override UITableViewCell GetCell (
                UITableView tableView, NSIndexPath indexPath)
            {
                UITableViewCell cell =
                    tableView.DequeueReusableCell (
                        kCellIdentifier);
                if (cell == null)
                {
                    cell = new UITableViewCell (
                        UITableViewCellStyle.Default,
                        kCellIdentifier);
                }
                cell.TextLabel.Text = list[indexPath.Row];
                return cell;
            }
        }

	

    }
}
