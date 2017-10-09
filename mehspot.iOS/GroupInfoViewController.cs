using Foundation;
using System;
using UIKit;
using MonoTouch.Dialog;
using Mehspot.Core.DTO.Groups;
using Mehspot.Core;
using Mehspot.iOS.Wrappers;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.Services;

namespace mehspot.iOS
{
    public class GroupInfoViewController : DialogViewController
    {
        private IViewHelper viewHelper;
        private GroupService groupService;

        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public GroupTypeEnum GroupType { get; set; }
        public GroupUserTypeEnum GroupUserType { get; set; }

        public GroupInfoViewController() : base(UITableViewStyle.Grouped, null, true)
        {
        }

        public override void ViewDidLoad()
        {
            this.viewHelper = new ViewHelper(this.View);
            this.groupService = new GroupService(MehspotAppContext.Instance.DataStorage);
            this.Root = new RootElement("Group Info") {
                new Section (string.Empty) {
                    new StringElement ("Name", this.GroupName),
                    new StringElement ("Description", this.GroupDescription),
                    new StringElement ("Group Type", MehspotResources.ResourceManager.GetString($"GroupTypeEnum_{this.GroupType}")),
                    new StringElement ("User Role", MehspotResources.ResourceManager.GetString($"GroupUserTypeEnum_{this.GroupUserType}")),
                }
            };

            if (GroupUserType != GroupUserTypeEnum.Owner)
            {
                UIButton leaveButton = new UIButton(UIButtonType.System);
                leaveButton.SetTitle("Leave Group", UIControlState.Normal);
                leaveButton.TouchUpInside += LeaveButton_TouchUpInside;
                leaveButton.Frame = new CoreGraphics.CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, 44);
                leaveButton.SetTitleColor(UIColor.Red, UIControlState.Normal);
                Root.Add(new Section(string.Empty) {
                    new UIViewElement(string.Empty, leaveButton, false)
                });
            }
        }

        void LeaveButton_TouchUpInside(object sender, EventArgs e)
        {
            UIAlertController alert = UIAlertController
                    .Create("Please confirm",
                            $"Do you really want to leave a group \"{GroupName}\"?",
                            UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
            alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, async (obj) =>
            {
                viewHelper.ShowOverlay("Wait");
                var result = await groupService.LeaveGroupAsync(this.GroupId).ConfigureAwait(false);
                InvokeOnMainThread(() =>
                {
                    viewHelper.HideOverlay();
                    if (result.IsSuccess)
                    {
                        this.DismissViewController(true, null);
                    }
                    else
                    {
                        viewHelper.ShowAlert("Error", result.ErrorMessage);
                    }
                });
            }));

            this.PresentViewController(alert, true, null);

        }
    }
}