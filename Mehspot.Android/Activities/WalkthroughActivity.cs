
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Database;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Mehspot.AndroidApp.Views;
using Mehspot.AndroidApp.Wrappers;
using Mehspot.Core;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Subdivision;
using Mehspot.Core.Services;
using AlertDialog = Android.App.AlertDialog;

namespace Mehspot.AndroidApp.Activities
{
    [Activity(Label = "WalkthroughActivity")]
    public class WalkthroughActivity : AppCompatActivity
    {
        private const ushort CameraRequestCode = 19081;
        private const ushort GalleryRequestCode = 19082;
        private const ushort PermissionsRequestCode = 19083;
        private volatile bool profileImageChanged;
        private int[] layouts;
        private ViewPager viewPager;
        private LinearLayout dotsLayout;
        private TextView[] dots;
        private ProfileService profileService;
        private SubdivisionService subdivisionService;
        private ActivityHelper viewHelper;

        private ImageView UserImage;
        private Button Continue1Button;

        private bool subdivisionSelectorEnabled;
        private bool subdivisionsLoaded;
        List<SubdivisionDTO> Subdivisions;
        private ExtendedEditText ZipField;
        private Button SubdivisionButton;
        private Button Continue2Button;
        private ProfileDto profile;
        private Button Continue3Button;
        private Button Continue4Button;
        private BadgeGroup badgeGroup;
        private View[] badgeGroups;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            viewHelper = new ActivityHelper(this);
            profileService = new ProfileService(MehspotAppContext.Instance.DataStorage);
            subdivisionService = new SubdivisionService(MehspotAppContext.Instance.DataStorage);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.WalkthroughActivity);

            layouts = new int[]
            {
                        Resource.Layout.WalkthroughStep1,
                        Resource.Layout.WalkthroughStep2,
                        Resource.Layout.WalkthroughStep3,
                        Resource.Layout.WalkthroughStep4,
            };

            viewPager = (ViewPager)FindViewById(Resource.Id.viewPager);
            dotsLayout = (LinearLayout)FindViewById(Resource.Id.layoutPanel);
            AddDots(0);

            WalkthroughPagerAdapter adapter = new WalkthroughPagerAdapter(layouts);
            viewPager.PageSelected += ViewPager_PageSelectedAsync;
            viewPager.Adapter = adapter;
            adapter.ViewInstantiated += Adapter_ViewInstantiated;
            adapter.ViewDestroyed += Adapter_ViewDestroyed;
            var continue4Button = FindViewById<Button>(Resource.Walkthrough4.ContinueButton);
            //btnNext.Click += (sender, e) =>
            //{
            // int current = GetItem(+1);
            // if (current < layouts.Length)
            //     //move to next screen
            //     viewPager.CurrentItem = current;
            // else
            // {
            //     //lauch main screen here
            //     Intent intent = new Intent(this, typeof(MainActivity));
            //     StartActivity(intent);

            // }
            //};

            LoadProfileAsync();
        }

        async void ViewPager_PageSelectedAsync(object sender, ViewPager.PageSelectedEventArgs e)
        {
            AddDots(e.Position);

            if (e.Position == 0)
            {
                if (!subdivisionsLoaded && subdivisionSelectorEnabled)
                {
                    await LoadSubdivisionsAsync();
                }
            }
        }

        void Adapter_ViewDestroyed(int position, View view)
        {
            switch (position)
            {
                case 0:
                    view.FindViewById<Button>(Resource.Walkthrough1.pictureButton).Click -= PictureButtonClick;
                    view.FindViewById<Button>(Resource.Walkthrough1.ContinueButton).Click -= Continue1Button_Click;
                    break;
                case 1:
                    view.FindViewById<EditText>(Resource.Walkthrough2.zipCode).TextChanged -= ZipCode_TextChangedAsync;
                    FindViewById<Button>(Resource.Walkthrough2.subdivisionButton).Click -= SubdivisionButton_Click;
                    SubdivisionButton.Click -= SubdivisionButton_Click;
                    ZipField.TextChanged -= ZipCode_TextChangedAsync;
                    Continue2Button.Click -= Continue2Button_Click;
                    break;
                case 2:
                    Continue3Button.Click -= Continue3Button_Click;
                    break;
                case 3:
                    Continue4Button.Click -= Continue4Button_ClickAsync;
                    break;
            }
        }

        void Adapter_ViewInstantiated(int position, View view)
        {
            switch (position)
            {
                case 0:
                    UserImage = view.FindViewById<ImageView>(Resource.Walkthrough1.ProfilePicture);
                    UserImage.ClipToOutline = true;
                    SetProfileData();
                    Continue1Button = view.FindViewById<Button>(Resource.Walkthrough1.ContinueButton);
                    view.FindViewById<Button>(Resource.Walkthrough1.pictureButton).Click += PictureButtonClick;
                    view.FindViewById<Button>(Resource.Walkthrough1.ContinueButton).Click += Continue1Button_Click;
                    break;
                case 1:
                    Continue2Button = view.FindViewById<Button>(Resource.Walkthrough2.ContinueButton);
                    Continue2Button.Click += Continue2Button_Click;
                    ZipField = view.FindViewById<ExtendedEditText>(Resource.Walkthrough2.zipCode);
                    ZipField.Mask = "#####";
                    SetProfileData();
                    ZipField.TextChanged += ZipCode_TextChangedAsync;
                    SubdivisionButton = FindViewById<Button>(Resource.Walkthrough2.subdivisionButton);
                    SubdivisionButton.Click += SubdivisionButton_Click;
                    break;
                case 2:
                    Continue3Button = view.FindViewById<Button>(Resource.Walkthrough3.ContinueButton);
                    Continue3Button.Click += Continue3Button_Click;
                    break;
                case 3:
                    Continue4Button = view.FindViewById<Button>(Resource.Walkthrough4.ContinueButton);
                    Continue4Button.Click += Continue4Button_ClickAsync;
                    badgeGroups = new[] { view.FindViewById(Resource.Walkthrough4.Friends),
                        view.FindViewById(Resource.Walkthrough4.Helpers),
                        view.FindViewById(Resource.Walkthrough4.Jobs)
                    };

                    foreach (var badgeGroup in badgeGroups)
                    {
                        badgeGroup.Click += (sender, e) => SelectGroup(badgeGroup);
                    }

                    break;
                default:
                    break;
            }
        }

        private void SetProfileData()
        {
            if (ZipField != null)
            {
                ZipField.TextChanged -= ZipCode_TextChangedAsync;
                ZipField.Text = profile?.Zip;
                ZipField.TextChanged += ZipCode_TextChangedAsync;
            }

            if (UserImage != null)
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    if (!string.IsNullOrWhiteSpace(profile?.ProfilePicturePath))
                    {
                        var imageBitmap = this.GetImageBitmapFromUrl(profile.ProfilePicturePath);
                        RunOnUiThread(() =>
                        {
                            try
                            {
                                UserImage?.SetImageBitmap(imageBitmap);
                                Continue1Button.Visibility = ViewStates.Visible;
                            }
                            catch (Exception ex)
                            {
                            }
                        });
                    }
                });
            }
        }

        private void SelectGroup(View groupView)
        {
            badgeGroup = (BadgeGroup)Enum.Parse(typeof(BadgeGroup), groupView.Tag.ToString());
            foreach (var item in badgeGroups)
            {
                if (item == groupView)
                {
                    item.SetBackgroundColor(new Color(ContextCompat.GetColor(this, Resource.Color.light_green)));
                }
                else
                {
                    item.SetBackgroundColor(Color.White);
                }
            }
            Continue4Button.Visibility = ViewStates.Visible;
        }

        private void Continue1Button_Click(object sender, EventArgs e)
        {
            viewPager.CurrentItem = viewPager.CurrentItem + 1;
        }

        void Continue2Button_Click(object sender, EventArgs e)
        {
            viewPager.CurrentItem = viewPager.CurrentItem + 1;
        }

        private void Continue3Button_Click(object sender, EventArgs e)
        {
            viewPager.CurrentItem = viewPager.CurrentItem + 1;
        }

        private async void Continue4Button_ClickAsync(object sender, EventArgs e)
        {
            if (profile == null)
            {
                await LoadProfileAsync();
                return;
            }

            if (!profileImageChanged && string.IsNullOrWhiteSpace(profile.ProfilePicturePath))
            {
                viewHelper.ShowPrompt("Error", "Please, upload a profile picture", "OK", () =>
                {
                    viewPager.CurrentItem = 0;
                });
                return;
            }

            if (profile.Zip == null)
            {
                viewHelper.ShowPrompt("Error", "Please, set Zip code and Subdivision", "OK", () =>
                {
                    viewPager.CurrentItem = 1;
                });
                return;
            }

            viewHelper.ShowOverlay("Saving profile");

            await TryUploadProfilePictureAsync();
            var result = await profileService.UpdateAsync(profile);

            if (result.IsSuccess)
            {
                MehspotAppContext.Instance.DataStorage.PreferredBadgeGroup = badgeGroup;
                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            }
            else if (!result.IsNetworkIssue)
            {
                viewHelper.ShowAlert("Error", "Can not save user profile");
            }

			viewHelper.HideOverlay();
        }

        private void PictureButtonClick(object sender, EventArgs e)
        {
            if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.ReadExternalStorage) ==
                Permission.Granted)
            {
                DisplayCameraDialog();
            }
            else
            {
                if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.ReadExternalStorage) != Permission.Granted)
                {
                    Android.Support.V4.App.ActivityCompat.RequestPermissions(this, new string[] {
                        Android.Manifest.Permission.ReadExternalStorage}, PermissionsRequestCode);
                }
            }
        }

        async void ZipCode_TextChangedAsync(object sender, TextChangedEventArgs e)
        {
            if (ZipField.IsValid)
            {
                var imm = (InputMethodManager)ZipField.Context.GetSystemService(InputMethodService);
                var result = imm.HideSoftInputFromWindow(ZipField.WindowToken, 0);
            }

            await LoadSubdivisionsAsync();
        }

        void SubdivisionButton_Click(object sender, EventArgs e)
        {
            if (!subdivisionSelectorEnabled)
            {
                return;
            }

            var target = new Intent(this, typeof(SubdivisionsListActivity));
            target.PutExtra("zipCode", profile.Zip);
            target.PutExtra("subdivisions", Subdivisions);
            target.PutExtra<int?>("selectedSubdivisionId", profile.SubdivisionId);
            target.PutExtra<Action<SubdivisionDTO>>("onDismissed", (dto) =>
            {
                SubdivisionButton.Text = dto?.DisplayName ?? "Subdivision";
                profile.SubdivisionId = dto?.Id;
                profile.SubdivisionOptionId = dto?.OptionId;
                if (dto != null)
                {
                    Continue2Button.Visibility = ViewStates.Visible;
                }
            });

            StartActivity(target);
        }

        private async System.Threading.Tasks.Task LoadProfileAsync()
        {
            viewHelper.ShowOverlay("Loading Profile");
            var result = await profileService.LoadProfileAsync();
            viewHelper.HideOverlay();
            if (result.IsSuccess)
            {
                profile = result.Data;
                SetProfileData();
            }
            else if (!result.IsNetworkIssue)
            {
                viewHelper.ShowAlert("Error", "Can not load profile");
            }
        }
        private async System.Threading.Tasks.Task LoadSubdivisionsAsync()
        {
            subdivisionSelectorEnabled = false;
            if (ZipField.IsValid)
            {
                viewHelper.ShowOverlay("Loading Subdivisions");
                var subdivisionsResult = await subdivisionService.ListSubdivisionsAsync(ZipField.Text).ConfigureAwait(false);
                if (subdivisionsResult.IsSuccess)
                {
                    subdivisionsLoaded = true;
                    Subdivisions = subdivisionsResult.Data;
                }
                else if (!subdivisionsResult.IsNetworkIssue)
                {
                    viewHelper.ShowAlert("Error", subdivisionsResult.ErrorMessage);
                }

                RunOnUiThread(() => SubdivisionButton.Text = Subdivisions?.Count <= 0 ? "Add Subdivision" : "Subdivision");
                viewHelper.HideOverlay();
            }

            RunOnUiThread(() =>
            {
                subdivisionSelectorEnabled = ZipField.IsValid;
            });
        }

        async System.Threading.Tasks.Task TryUploadProfilePictureAsync()
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

            Result<string> photoResult = null;
            if (stream != null)
            {
                photoResult = await profileService.UploadProfileImageAsync(stream).ConfigureAwait(false);
                stream.Dispose();
            }

            if (photoResult != null && !photoResult.IsSuccess && !photoResult.IsNetworkIssue)
            {
                viewHelper.ShowAlert("Error", "Can not save profile picture");
                viewHelper.HideOverlay();
            }
        }

        private void AddDots(int currentPage)
        {
            dots = new TextView[layouts.Length];

            string colorsActive = "#C3C3C3";
            string colorsInactive = "#c6d881";

            dotsLayout.RemoveAllViews();
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i] = new TextView(this);
                dots[i].Text = (Html.FromHtml("•")).ToString();
                dots[i].TextSize = 35;
                dots[i].SetTextColor(Color.ParseColor(colorsActive));
                dotsLayout.AddView(dots[i]);
            }

            if (dots.Length > 0)
            {
                dots[currentPage].SetTextColor(Color.ParseColor(colorsInactive));
            }
        }

        int GetItem(int i)
        {
            return viewPager.CurrentItem + i;
        }

        public class WalkthroughPagerAdapter : PagerAdapter
        {
            LayoutInflater layoutInflater;
            int[] _layout;

            public WalkthroughPagerAdapter(int[] layout)
            {
                _layout = layout;
            }

            public event Action<int, View> ViewInstantiated;
            public event Action<int, View> ViewDestroyed;

            public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
            {
                layoutInflater = (LayoutInflater)Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService);
                View view = layoutInflater.Inflate(_layout[position], container, false);
                container.AddView(view);

                ViewInstantiated?.Invoke(position, view);
                return view;
            }

            public override int Count
            {
                get
                {
                    return _layout.Length;
                }
            }

            public override bool IsViewFromObject(View view, Java.Lang.Object objectValue)
            {
                return view == objectValue;
            }

            public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object objectValue)
            {
                View view = (View)objectValue;
                ViewDestroyed?.Invoke(position, view);
                container.RemoveView(view);
            }
        }

        private void DisplayCameraDialog()
        {
            View view = (View)LayoutInflater.From(this).Inflate(Resource.Layout.DialogView, null);
            AlertDialog selectionDialog = new AlertDialog.Builder(this).Create();
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
                            DisplayCameraDialog();
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

        protected override void OnActivityResult(int requestCode, Android.App.Result resultCode, Intent data)
        {
            if (data == null || resultCode != Android.App.Result.Ok)
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

            Continue1Button.Visibility = ViewStates.Visible;
            profileImageChanged = true;
        }

        private string GetImagePath(Android.Net.Uri uri)
        {
            string path = null;
            string[] projection = new[] { Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data };
            using (ICursor cursor = ContentResolver.Query(uri, projection, null, null, null))
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
    }
}
