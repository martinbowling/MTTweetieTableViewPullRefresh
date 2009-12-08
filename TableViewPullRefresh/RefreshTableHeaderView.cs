
using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;

namespace TableViewPullRefresh
{


	public partial class RefreshTableHeaderView : UIView
	{
		public enum RefreshStatus {
			ReleaseToReloadStatus =	0,
			PullToReloadStatus = 1,
			LoadingStatus = 2,
		}

		
		private UILabel lastUpdatedLabel;
		private UILabel statusLabel;
		private UIImageView arrowImage;
		private UIActivityIndicatorView activityView;
		
		public bool isFlipped;
		
		public RefreshTableHeaderView ( ) :base()
		{
		
			this.BackgroundColor = new UIColor (226.0f,231.0f,237.0f,1.0f);
			
			lastUpdatedLabel = new UILabel (new RectangleF (0.0f, this.Frame.Height - 30.0f, 320.0f, 20.0f));
			lastUpdatedLabel.Font = UIFont.SystemFontOfSize(12.0f);
			lastUpdatedLabel.TextColor = UIColor.Black; //new UIColor (87.0f,108.0f,137.0f,1.0f);
			lastUpdatedLabel.ShadowColor = UIColor.FromWhiteAlpha(0.9f,1.0f);
			lastUpdatedLabel.ShadowOffset = new SizeF (0.0f,1.0f);
			lastUpdatedLabel.BackgroundColor = UIColor.Clear; 
			lastUpdatedLabel.TextAlignment = UITextAlignment.Center; 
			
			if (NSUserDefaults.StandardUserDefaults["EGORefreshTableView_LastRefresh"] != null)
			{
				lastUpdatedLabel.Text = NSUserDefaults.StandardUserDefaults["EGORefreshTableView_LastRefresh"].ToString();
			}
			else
			{
				setCurrentDate();
			}
			
			this.AddSubview (lastUpdatedLabel);
			
			statusLabel = new UILabel (new RectangleF (0.0f, this.Frame.Height - 48.0f, 320.0f, 20.0f));
			statusLabel.Font = UIFont.BoldSystemFontOfSize(13.0f); 
			statusLabel.TextColor = UIColor.Black; //new UIColor (87.0f,108.0f,137.0f,1.0f);
			statusLabel.ShadowColor = UIColor.FromWhiteAlpha(0.9f,1.0f);
			statusLabel.ShadowOffset = new SizeF (0.0f,1.0f);
			statusLabel.BackgroundColor = UIColor.Clear; 
			statusLabel.TextAlignment = UITextAlignment.Center; 
			setStatus(RefreshTableHeaderView.RefreshStatus.PullToReloadStatus);
			this.AddSubview(statusLabel); 
	
			arrowImage = new UIImageView(new RectangleF (25.0f, this.Frame.Height - 65.0f, 30.0f, 55.0f));
			arrowImage.Image = UIImage.FromFile ("blueArrow.png");
			arrowImage.ContentMode = UIViewContentMode.ScaleAspectFit;
			arrowImage.Layer.Transform = CATransform3D.MakeRotation(3.141592653589793238462643f, 0.0f,0.0f,1.0f);
			this.AddSubview(arrowImage);
			
			activityView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
			activityView.Frame = new RectangleF (25.0f, this.Frame.Height - 38.0f, 20.0f, 20.0f);
			activityView.HidesWhenStopped = true;
			
			this.AddSubview(activityView);

			this.isFlipped = false;
			
			
		}
		
		public void setStatus (RefreshStatus status) 
		{
		
			switch (status) 
			{			
				case RefreshStatus.LoadingStatus:
					this.statusLabel.Text = "Loading...";
					break;
					
				case RefreshStatus.PullToReloadStatus:
					this.statusLabel.Text = "Pull down to refresh...";
					break;
				case RefreshStatus.ReleaseToReloadStatus:
					this.statusLabel.Text = "Release to refresh...";
					break;
			}
		}
		
		
		public void toggleActivityView()
		{
			if (activityView.IsAnimating)
			{
				activityView.StopAnimating();
				arrowImage.Hidden = false;
			}
			else
			{
				activityView.StartAnimating();
				arrowImage.Hidden = true;
				this.setStatus (RefreshStatus.LoadingStatus);
			}

		}
		
		
		public void setCurrentDate()
		{
			lastUpdatedLabel.Text = String.Format("Last Updated: {0}", DateTime.Now.ToString("G"));
			NSUserDefaults.StandardUserDefaults["EGORefreshTableView_LastRefresh"] = new NSString(DateTime.Now.ToString("G"));
			NSUserDefaults.StandardUserDefaults.Synchronize();
		}
		
		public void flipImageAnimated(bool animated)
		{
		
			UIView.BeginAnimations("flipImage");
			UIView.SetAnimationDuration (animated ? .18f : 0.0f);
			arrowImage.Layer.Transform = isFlipped ? CATransform3D.MakeRotation(3.141592653589793238462643f, 0.0f, 0.0f, 1.0f) : CATransform3D.MakeRotation(3.141592653589793238462643f * 2, 0.0f, 0.0f, 1.0f);
			UIView.CommitAnimations();
			isFlipped = !isFlipped;
			
		}
		
		public void drawRect()
		{
		
			CGContext context = UIGraphics.GetCurrentContext();
			context.DrawPath (CGPathDrawingMode.FillStroke);
			//UIColor strokeColor = new UIColor (87.0f,108.0f,137.0f,1.0f)
			CGColor strokeColor = new CGColor (87.0f,108.0f,137.0f,1.0f);
			context.SetStrokeColorWithColor (strokeColor);
			context.BeginPath();
			context.MoveTo(0.0f,this.Bounds.Size.Height -1);
					
			context.AddLineToPoint (this.Bounds.Size.Width, this.Bounds.Size.Height -1);
			context.StrokePath();                     

		}
		
	}
}
