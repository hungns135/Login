using System;
using UIKit;
using Facebook.LoginKit;
using Facebook.CoreKit;
using System.Collections.Generic;
using System.Drawing;
using CoreGraphics;
using Foundation;

namespace Login
{
	public partial class ViewController : UIViewController
	{

		List<string> readPermissions = new List<string> { "public_profile" };

		LoginButton loginView;
		ProfilePictureView pictureView;
		UILabel nameLabel;

		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// If was send true to Profile.EnableUpdatesOnAccessTokenChange method
			// this notification will be called after the user is logged in and
			// after the AccessToken is gotten
			Profile.Notifications.ObserveDidChange((sender, e) =>
			{

				if (e.NewProfile == null)
					return;

				nameLabel.Text = e.NewProfile.Name;
			});

			// Set the Read and Publish permissions you want to get
			loginView = new LoginButton(new CGRect(51, 20, 218, 46))
			{
				LoginBehavior = LoginBehavior.Native,
				ReadPermissions = readPermissions.ToArray()
			};

			// Handle actions once the user is logged in
			loginView.Completed += (sender, e) =>
			{
				if (e.Error != null)
				{
					// Handle if there was an error
					new UIAlertView("Login", e.Error.Description, null, "Ok", null).Show();
					return;
				}

				if (e.Result.IsCancelled)
				{
					// Handle if the user cancelled the login request
					new UIAlertView("Login", "The user cancelled the login", null, "Ok", null).Show();
					return;
				}

				// Handle your successful login
			/*	NextScreenController nextSreen = this.Storyboard.InstantiateViewController("NextScreen") as NextScreenController;
				if (nextSreen != null)
					this.NavigationController.PushViewController(nextSreen, true);
			*/
				new UIAlertView("Login", "Success!!", null, "Ok", null).Show();


			};

			// Handle actions once the user is logged out
			loginView.LoggedOut += (sender, e) =>
			{
				// Handle your logout
			};


			// The user image profile is set automatically once is logged in
			pictureView = new ProfilePictureView(new CGRect(50, 80, 220, 220));

			// Create the label that will hold user's facebook name
			nameLabel = new UILabel(new RectangleF(20, 319, 280, 21))
			{
				TextAlignment = UITextAlignment.Center,
				BackgroundColor = UIColor.Clear
			};

			// If you have been logged into the app before, ask for the your profile name
			if (AccessToken.CurrentAccessToken != null)
			{
				var request = new GraphRequest("/me?fields=name", null, AccessToken.CurrentAccessToken.TokenString, null, "GET");
				request.Start((connection, result, error) =>
				{
					// Handle if something went wrong with the request
					if (error != null)
					{
						new UIAlertView("Request Profile name Error...", error.Description, null, "Ok", null).Show();
						return;
					}

					// Get your profile name
					var userInfo = result as NSDictionary;
					nameLabel.Text = userInfo["name"].ToString();
				});
			}





			// Add views to main view
			View.AddSubview(loginView);
			View.AddSubview(pictureView);
			View.AddSubview(nameLabel);
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}
