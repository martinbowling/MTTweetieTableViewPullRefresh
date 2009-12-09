/*
Copyright (c) 2009, Martin R. Bowling

Betterified by Chris Hardy (in other words he made it work =P)

Copying and distribution of this file, with or without
modification, are permitted in any medium without royalty.
This file is offered as-is, without any warranty.
*/

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

	[Register("AppController")]
	public class AppController : UIApplicationDelegate
	{
		private UIWindow window;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// Create the main view controller
			var vc = new RefreshingUITableViewController ();
			
			// Create the main window and add main view
			// controller as a subview
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.AddSubview (vc.View);
			
			window.MakeKeyAndVisible ();
			return true;
		}

		// This method is allegedly required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
	}


}
