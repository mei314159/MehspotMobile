using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Database;
using Android.Support.V4.Widget;
using Mehspot.AndroidApp.Wrappers;
using Mehspot.AndroidApp.Core.Builders;
using Mehspot.Core;
using Mehspot.Core.DTO;
using Mehspot.Core.Models;
using Mehspot.Core.Services;
using Mehspot.Core.Contracts.Wrappers;
using Android.Graphics;
using Android.Provider;
using Android.Content.PM;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using System.Linq;

namespace Mehspot.AndroidApp
{
    public class ProfileFragment : Android.Support.V4.App.Fragment, IProfileViewController
    {
        private const ushort CameraRequestCode = 23060;
        private const ushort GalleryRequestCode = 23061;
        private const ushort PermissionsRequestCode = 23062;

        private volatile bool profileImageChanged;
        private ProfileModel<View> model;
        private ProfileService profileService;
        private SubdivisionService subdivisionService;
        private string profilePicturePath;

        public Button SaveButton => this.View?.FindViewById<Button>(Resource.ProfileViewActivity.saveProfileButton);
        public Button ChangePhotoButton => this.View?.FindViewById<Button>(Resource.ProfileViewActivity.changePhotoButton);
        public Button SignOutButton => this.View?.FindViewById<Button>(Resource.ProfileViewActivity.signoutButton);
        public TextView UserNameLabel => this.View?.FindViewById<TextView>(Resource.ProfileViewActivity.userNameLabel);
        public TextView UserFullNameLabel => this.View?.FindViewById<TextView>(Resource.ProfileViewActivity.userFullNameLabel);
        public ImageView UserImage => this.View?.FindViewById<ImageView>(Resource.ProfileViewActivity.profilePhoto);
        public View ProfileContainer => this.View?.FindViewById<LinearLayout>(Resource.ProfileViewActivity.container);
        public SwipeRefreshLayout refresher => this.View?.FindViewById<SwipeRefreshLayout>(Resource.ProfileViewActivity.profileRefresher);

        #region IProfileViewController 
        public IViewHelper ViewHelper { get; private set; }

        public bool IsActive => Activity != null && this.IsVisible;
        public string UserName
        {
            get
            {
                return this.UserNameLabel?.Text;
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
                return this.UserFullNameLabel?.Text;
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
                    //var url = new URL(profilePicturePath);
                    if (profilePicturePath != null)
                    {
                        try
                        {
                            var bitmap = this.Activity.GetImageBitmapFromUrl(profilePicturePath);
                            UserImage.SetImageBitmap(bitmap);
                        }
                        catch (Exception ex)
                        {
                        }
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

        public override void OnDestroyView()
        {
            model.LoadingStart -= Model_LoadingStart;
            model.LoadingEnd -= Model_LoadingEnd;
            model.SignedOut -= Model_SignedOut;
            base.OnDestroyView();
        }

        public override async void OnStart()
        {
            base.OnStart();
            (this.Activity as MainActivity)?.SelectTab(this.GetType());
            if (!model.dataLoaded)
            {
                await model.RefreshView(true);
            }
        }

        public void ReloadData()
        {
            var wrapper = this.View.FindViewById<LinearLayout>(Resource.ProfileViewActivity.profileContentWrapper);
            wrapper.RemoveAllViews();

            foreach (var item in model.Cells)
            {
                wrapper.AddView(item);
            }
        }

        private void Model_LoadingStart()
        {
            Activity.RunOnUiThread(() =>
            {
                this.SaveButton.Enabled = this.ChangePhotoButton.Enabled = false;
                //ViewHelper.ShowOverlay("Loading");
                refresher.Refreshing = true;
            });
        }

        private async void Model_LoadingEnd(Result<ProfileDto> profileResult)
        {
            if (View == null)
                return;
            if (profileResult.IsSuccess)
            {
                await model.InitializeTableAsync(profileResult.Data).ConfigureAwait(false);
            }
            else if (!profileResult.IsNetworkIssue)
            {
                ViewHelper.ShowAlert("Error", "Can not load profile data");
            }

            Activity.RunOnUiThread(() =>
            {
                if (!this.IsActive)
                    return;
                ProfileContainer.Visibility = ViewStates.Visible;
                this.SaveButton.Enabled = this.ChangePhotoButton.Enabled = true;
                //ViewHelper.HideOverlay();
                refresher.Refreshing = false;
            });
        }

        private void ChangePhotoButton_Click(object sender, EventArgs e)
        {
            if (ContextCompat.CheckSelfPermission(this.Activity, Android.Manifest.Permission.ReadExternalStorage) ==
                Permission.Granted)
            {
                DisplayAlertDialog();
            }
            else
            {
                if (ContextCompat.CheckSelfPermission(this.Activity, Android.Manifest.Permission.ReadExternalStorage) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this.Activity, new string[] {
                        Android.Manifest.Permission.ReadExternalStorage}, PermissionsRequestCode);
                }
            }
        }

        private void DisplayAlertDialog()
        {
            View view = (View)LayoutInflater.From(this.Activity).Inflate(Resource.Layout.DialogView, null);
            AlertDialog selectionDialog = new AlertDialog.Builder(this.Activity).Create();
            selectionDialog.SetView(view);
            selectionDialog.SetCanceledOnTouchOutside(false);


            Button openCameraButton = view.FindViewById<Button>(Resource.DialogView.openCamera);
            openCameraButton.Click += (sender, e) =>
                        {
                            selectionDialog.Cancel();
                            selectionDialog.Dismiss();
                            Intent intentCamera = new Intent(MediaStore.ActionImageCapture);
                            StartActivityForResult(intentCamera, CameraRequestCode);
                        };

            Button openGalleryButton = view.FindViewById<Button>(Resource.DialogView.openPhotoGallery);
            openGalleryButton.Click += (sender, e) =>
                        {
                            selectionDialog.Cancel();
                            Intent intentGallery = new Intent();
                            intentGallery.SetType("image/*");
                            intentGallery.SetAction(Intent.ActionGetContent);
                            StartActivityForResult(Intent.CreateChooser(intentGallery, "Select Photo"), GalleryRequestCode);
                        };

            Button closeDialog = view.FindViewById<Button>(Resource.DialogView.closeView);
            closeDialog.Click += (sender, e) =>
                        {
                            selectionDialog.Cancel();
                        };

            selectionDialog.Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            switch (requestCode)
            {
                case PermissionsRequestCode:
                    {
                        if (grantResults.FirstOrDefault() == Permission.Granted)
                        {
                            DisplayAlertDialog();
                        }
                        else
                        {
                            // alert permission denied
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (data == null || resultCode != (int)Android.App.Result.Ok)
                return;

            switch (requestCode)
            {
                case CameraRequestCode:
                    base.OnActivityResult(requestCode, resultCode, data);
                    Bitmap bitmap = (Bitmap)data.Extras.Get("data");
                    UserImage.SetImageBitmap(bitmap);
                    break;
                case GalleryRequestCode:
                    var imagePath = GetImagePath(data.Data);
                    var imageBitmap = BitmapFactory.DecodeFile(imagePath);
                    if (imageBitmap != null)
                    {
                        UserImage.SetImageBitmap(imageBitmap);
                    }
                    else
                    {
                        UserImage.SetImageURI(data.Data);
                    }
                    break;
            }

            profileImageChanged = true;
        }

        private string GetImagePath(Android.Net.Uri uri)
        {
            string path = null;
            string[] projection = new[] { Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data };
            using (ICursor cursor = this.Activity.ContentResolver.Query(uri, projection, null, null, null))
            {
                if (cursor != null)
                {
                    int columnIndex = cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data);
                    cursor.MoveToFirst();
                    path = cursor.GetString(columnIndex);
                }
            }
            return path;
        }

        private void SignOutButton_Click(object sender, EventArgs e)
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

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            Stream stream = null;
            if (profileImageChanged)
            {
                UserImage.BuildDrawingCache(true);
                Bitmap bitmap = UserImage.GetDrawingCache(true);

                stream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);

                profileImageChanged = false;
            }

            var result = await model.SaveProfileAsync(stream);
            if (result){
                this.UserName = model.Profile.UserName;
            }
            if (stream != null)
            {
                stream.Dispose();
            }
        }

        public void InvokeOnMainThread(Action action)
        {
            this.Activity?.RunOnUiThread(action);
        }
    }

}
