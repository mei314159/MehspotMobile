using Foundation;
using System;
using UIKit;
using Mehspot.Core.DTO.Groups;

namespace mehspot.iOS
{
    public partial class GroupsListItemCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("GroupsListItemCell");
        public static readonly UINib Nib;
        GroupsListItemDTO item;

        public GroupsListItemDTO Item
        {
            get
            {
                return item;
            }

            set
            {
                item = value;
                if (value != null)
                {
                    this.GroupName.Text = value.GroupName;
                    this.LastMessage.Text = value.LastMessage;
                    this.GroupTypeName.Text = value.GroupType.ToString();
                    this.GroupType.Image = UIImage.FromBundle(value.GroupType == GroupTypeEnum.Closed ? "closed-group" : "private-group");
                }
            }
        }

        public GroupsListItemCell(IntPtr handle) : base(handle)
        {
        }

        static GroupsListItemCell()
        {
            Nib = UINib.FromName("GroupsListItemCell", NSBundle.MainBundle);
        }
    }
}