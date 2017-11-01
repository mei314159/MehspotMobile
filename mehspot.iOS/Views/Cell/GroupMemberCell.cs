using System;

using Foundation;
using Mehspot.Core;
using Mehspot.Core.DTO.Groups;
using SDWebImage;
using UIKit;

namespace mehspot.iOS.Views.Cell
{
    public partial class GroupMemberCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("GroupMemberCell");
        public static readonly UINib Nib;

        static GroupMemberCell()
        {
            Nib = UINib.FromName("GroupMemberCell", NSBundle.MainBundle);
        }

        protected GroupMemberCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
        GroupPrememberDTO member;

        public GroupPrememberDTO Member
        {
            get
            {
                return member;
            }

            set
            {
                member = value;
                Initialize();
            }
        }

        private void Initialize()
        {
            if (!string.IsNullOrWhiteSpace(Member.ProfilePicturePath))
                this.ProfilePicture.SetImage(new Uri(Member.ProfilePicturePath));

            this.Username.Text = Member.Username;
            this.Role.Text = MehspotResources.ResourceManager.GetString("GroupUserTypeEnum_" + Member.GroupUserType);
            if (Member.IsExistingUser)
            {
                this.Username.TextColor = UIColor.DarkTextColor;
                this.Role.TextColor = UIColor.DarkGray;
            }
            else
            {
                this.Username.TextColor = UIColor.LightGray;
                this.Role.TextColor = UIColor.LightGray;
            }
        }
    }
}
