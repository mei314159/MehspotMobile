using System;
using System.IO;
using System.Runtime.InteropServices;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Facebook;
using Android.Support.V4.Widget;
using Mehspot.AndroidApp.Wrappers;
using Mehspot.AndroidApp.Core.Builders;
using Mehspot.Core;
using Mehspot.Core.DTO;
using Mehspot.Core.Models;
using Mehspot.Core.Services;
using Mehspot.Core.Contracts.Wrappers;
using Android.Graphics;
using Java.Net;
using Android.Provider;
using System.Collections.Generic;
using System.Collections;
using Java.Nio;

namespace Mehspot.AndroidApp
{
	public class ProfileFragment : Android.Support.V4.App.Fragment, IProfileViewController
	{
		volatile bool profileImageChanged;

		private ProfileModel<View> model;
		private ProfileService profileService;
		private SubdivisionService subdivisionService;
		private string profilePicturePath;

		public Button SaveButton => this.Activity.FindViewById<Button>(Resource.ProfileViewActivity.saveProfileButton);
		public Button ChangePhotoButton => this.Activity.FindViewById<Button>(Resource.ProfileViewActivity.changePhotoButton);
		public Button SignOutButton => this.Activity.FindViewById<Button>(Resource.ProfileViewActivity.signoutButton);
		public TextView UserNameLabel => this.Activity.FindViewById<TextView>(Resource.ProfileViewActivity.userNameLabel);
		public TextView UserFullNameLabel => this.Activity.FindViewById<TextView>(Resource.ProfileViewActivity.userFullNameLabel);
		public ImageView UserImage => this.Activity.FindViewById<ImageView>(Resource.ProfileViewActivity.profilePhoto);
		public View ProfileContainer => this.Activity.FindViewById<LinearLayout>(Resource.ProfileViewActivity.container);

		#region IProfileViewController 
		public IViewHelper ViewHelper { get; private set; }
		public string UserName
		{
			get
			{
				return this.UserNameLabel.Text;
			}
			set
			{
				this.UserNameLabel.Text = value;
			}
		}

		public string FullName
		{
			get
			{
				return this.UserFullNameLabel.Text;
			}
			set
			{
				this.UserFullNameLabel.Text = value;
			}
		}

		public string ProfilePicturePath
		{
			get
			{
				return profilePicturePath;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					profilePicturePath = value;
					var url = new URL(profilePicturePath);
					if (profilePicturePath != null)
					{
						var bitmap = this.Activity.GetImageBitmapFromUrl(profilePicturePath);
						UserImage.SetImageBitmap(bitmap);
					}
				}
			}
		}

		public bool SaveButtonEnabled
		{
			get
			{
				return SaveButton.Enabled;
			}
			set
			{
				SaveButton.Enabled = value;
			}
		}
		#endregion

		public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate(Resource.Layout.ProfileViewActivity, container, false);
		}

		public override void OnViewCreated(View view, Bundle savedInstanceState)
		{
			base.OnViewCreated(view, savedInstanceState);
			ProfileContainer.Visibility = ViewStates.Gone;

			profileService = new ProfileService(MehspotAppContext.Instance.DataStorage);
			subdivisionService = new SubdivisionService(MehspotAppContext.Instance.DataStorage);
			ViewHelper = new ActivityHelper(this.Activity);

			model = new ProfileModel<View>(profileService, subdivisionService, this, new AndroidCellBuilder(this.Activity));
			model.LoadingStart += Model_LoadingStart;
			model.LoadingEnd += Model_LoadingEnd;
			model.SignedOut += Model_SignedOut;
			UserImage.ClipToOutline = true;

			var refresher = this.Activity.FindViewById<SwipeRefreshLayout>(Resource.ProfileViewActivity.profileRefresher);
			refresher.SetColorSchemeColors(Resource.Color.xam_dark_blue,
														Resource.Color.xam_purple,
														Resource.Color.xam_gray,
														Resource.Color.xam_green);
			refresher.Refresh += async (sender, e) =>
			{
				await model.RefreshView();
				refresher.Refreshing = false;
			};

			SaveButton.Click += SaveButton_Click;
			ChangePhotoButton.Click += ChangePhotoButton_Click;
			SignOutButton.Click += SignOutButton_Click;
		}

		public override async void OnStart()
		{
			base.OnStart();
			if (!model.dataLoaded)
			{
				await model.RefreshView();
			}
		}

		public void ReloadData()
		{
			var wrapper = this.Activity.FindViewById<LinearLayout>(Resource.ProfileViewActivity.profileContentWrapper);
			wrapper.RemoveAllViews();
			foreach (var item in model.Cells)
			{
				wrapper.AddView(item);
			}
		}

		private void Model_LoadingStart()
		{
			this.SaveButton.Enabled = this.ChangePhotoButton.Enabled = false;
			ViewHelper.ShowOverlay("Loading");
		}

		private void Model_LoadingEnd()
		{
			ViewHelper.HideOverlay();
			ProfileContainer.Visibility = ViewStates.Visible;
			this.SaveButton.Enabled = this.ChangePhotoButton.Enabled = true;
		}

		void ChangePhotoButton_Click(object sender, EventArgs e)
		{
			View view = (View)LayoutInflater.From(this.Activity).Inflate(Resource.Layout.DialogView, null);
			AlertDialog builder = new AlertDialog.Builder(this.Activity).Create();
			builder.SetView(view);
			builder.SetCanceledOnTouchOutside(false);
			builder.Show();

			Button openCameraButton = view.FindViewById<Button>(Resource.DialogView.openCamera);
			openCameraButton.Click += delegate
			{
				builder.Cancel();
				builder.Dismiss();
				Intent intentCamera = new Intent(MediaStore.ActionImageCapture);
				StartActivityForResult(intentCamera, 0);
			};

			Button openGalleryButton = view.FindViewById<Button>(Resource.DialogView.openPhotoGallery);
			openGalleryButton.Click += delegate
			{
				builder.Cancel();
				Intent intentGallery = new Intent();//(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
				intentGallery.SetType("image/*");
				intentGallery.SetAction(Intent.ActionGetContent);
				StartActivityForResult(Intent.CreateChooser(intentGallery, "Select Photo"), 1);
			};

			Button closeDialog = view.FindViewById<Button>(Resource.DialogView.closeView);
			closeDialog.Click += delegate
			{
				builder.Cancel();
			};
		}

		public override void OnActivityResult(int requestCode, int resultCode, Intent data)
		{
			if (data == null)
				return;
			switch (requestCode)
			{
				case 0:
					base.OnActivityResult(requestCode, resultCode, data);
					Bitmap bitmap = (Bitmap)data.Extras.Get("data");
					UserImage.SetImageBitmap(bitmap);
					break;
				case 1:
					UserImage.SetImageURI(data.Data);
					break;
			}
			profileImageChanged = true;
		}

		void SignOutButton_Click(object sender, EventArgs e)
		{
			model.Signout();
		}

		private void Model_SignedOut()
		{
			Xamarin.Facebook.Login.LoginManager.Instance.LogOut();
			Intent intent = new Intent(this.Activity, typeof(SignInActivity));
			StartActivity(intent);
			Activity.Finish();
		}

		public void HideKeyboard()
		{

		}

		async void SaveButton_Click(object sender, EventArgs e)
		{
			byte[] dataBytes = null;
			if (profileImageChanged)
			{
				UserImage.BuildDrawingCache(true);
				Bitmap bitmap = UserImage.GetDrawingCache(true);

				using (var stream = new MemoryStream())
				{
					bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
					dataBytes = stream.ToArray();
				}

				profileImageChanged = false;
			}

			await model.SaveProfileAsync(dataBytes);
		}
	}
}
