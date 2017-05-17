using System;
using System.Net;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Mehspot.Core;
using Mehspot.Core.DTO;

namespace Mehspot.AndroidApp.Resources.layout
{

    public class BadgeSummaryItem : RelativeLayout
    {
        readonly BadgeSummaryDTO dto;

        public event Action<BadgeSummaryDTO> Clicked;

        public BadgeSummaryItem (Context context, BadgeSummaryDTO dto) : base (context)
        {
            this.dto = dto;
            LayoutInflater inflater = (LayoutInflater)Context.GetSystemService (Context.LayoutInflaterService);
            inflater.Inflate (Resource.Layout.BadgeSummaryItem, this);

            BadgeName.Text = MehspotResources.ResourceManager.GetString(dto.BadgeName);
            //UIImage.FromFile("badges/" + badge.BadgeName.ToLower() + (badge.IsRegistered ? string.Empty : "b"))
            Picture.SetImageBitmap(BitmapFactory.DecodeFile("drawable/badges/" + dto.BadgeName.ToLower() + (dto.IsRegistered ? string.Empty : "b")));

            this.Click += Handle_Click;
        }

        public TextView BadgeName {
            get {
                return (TextView)FindViewById (Resource.Id.BadgeName);
            }
        }

        public ImageView Picture {
            get {
                return (ImageView)FindViewById (Resource.Id.Picture);
            }
        }

        void Handle_Click (object sender, EventArgs e)
        {
            this.Clicked?.Invoke(this.dto);
        }

        private Bitmap GetImageBitmapFromUrl (string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient ()) {
                var imageBytes = webClient.DownloadData (url);
                if (imageBytes != null && imageBytes.Length > 0) {
                    imageBitmap = BitmapFactory.DecodeByteArray (imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }
    }
}
