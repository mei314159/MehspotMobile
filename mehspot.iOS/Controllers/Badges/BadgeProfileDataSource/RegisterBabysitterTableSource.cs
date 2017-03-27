using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using mehspot.iOS.Views;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Services;
using UIKit;

namespace mehspot.iOS.Controllers.Badges.BadgeProfileDataSource
{
    public class RegisterBabysitterTableSource: UITableViewSource
    {
        private KeyValuePair<string, string> [] genders = new [] {
                new KeyValuePair<string, string>(null, "N/A"),
                new KeyValuePair<string, string>("M", "Male"),
                new KeyValuePair<string, string>("F", "Female")
        };

        KeyValuePair<int?, string> [] states;
        KeyValuePair<int?, string> [] subdivisions;

        private readonly List<UITableViewCell> cells;

        private readonly BadgeProfileDTO<EditBabysitterProfileDTO> profile;
        private readonly ProfileService profileService;

        private RegisterBabysitterTableSource (BadgeProfileDTO<EditBabysitterProfileDTO> profile, ProfileService profileService)
        {
            this.profile = profile;
            this.profileService = profileService;
            cells = new List<UITableViewCell> ();
        }

        public static async Task<RegisterBabysitterTableSource> Create (BadgeProfileDTO<EditBabysitterProfileDTO> profile, ProfileService profileService)
        {
            var result = new RegisterBabysitterTableSource (profile, profileService);
            await result.InitializeAsync ().ConfigureAwait(false);
            return result;
        }
        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var item = cells [indexPath.Row];
            return item;
        }

        public override nint RowsInSection (UITableView tableview, nint section)
        {
            return cells.Count;
        }


        public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            return cells [indexPath.Row].Frame.Height;
        }

        private async Task InitializeAsync () {
            this.states = await GetStates ();
            this.subdivisions = await GetSubdivisions (profile.Profile.Zip);
            cells.Add (PickerCell.Create (profile, a => a.Profile.State, (model, property) => { model.Profile.State = property; }, v => states.FirstOrDefault (a => a.Key == v).Value, "State", states));
            cells.Add (TextEditCell.Create (profile, a => a.Profile.City, "City"));
            var zipCell = TextEditCell.Create (profile, a => a.Profile.Zip, "Zip");
            zipCell.Mask = "#####";
            var subdivisionEnabled = !string.IsNullOrWhiteSpace (profile.Profile.Zip) && zipCell.IsValid;
            var subdivisionCell = PickerCell.Create (profile, a => a.Profile.SubdivisionId, (model, property) => { model.Profile.SubdivisionId = (int?)property; }, v => subdivisions.FirstOrDefault (a => a.Key == v).Value, "Subdivision", subdivisions, !subdivisionEnabled);
            zipCell.ValueChanged += (arg1, arg2) => ZipCell_ValueChanged (arg1, arg2, subdivisionCell);
            cells.Add (zipCell);
            cells.Add (subdivisionCell);

            foreach (var badgeValue in profile.BadgeValues) {

            }
        }

        async void ZipCell_ValueChanged (TextEditCell sender, string value, PickerCell subdivisionCell)
        {
            subdivisionCell.IsReadOnly = true;
            if (sender.IsValid) {
                subdivisionCell.RowValues = (await GetSubdivisions (profile.Profile.Zip)).Select (a => new KeyValuePair<object, string> (a.Key, a.Value)).ToArray ();
            }

            subdivisionCell.IsReadOnly = !sender.IsValid;
        }

        private async Task<KeyValuePair<int?, string> []> GetSubdivisions (string zipCode)
        {
            if (!string.IsNullOrWhiteSpace (zipCode)) {
                var subdivisionsResult = await profileService.GetSubdivisionsAsync (zipCode);
                if (subdivisionsResult.IsSuccess) {
                    return subdivisionsResult.Data.Select (a => new KeyValuePair<int?, string> (a.Id, a.DisplayName)).ToArray ();
                }
            }

            return null;
        }

        private async Task<KeyValuePair<int?, string> []> GetStates ()
        {
            var statesResult = await profileService.GetStatesAsync ();
            if (statesResult.IsSuccess) {
                return statesResult.Data.Select (a => new KeyValuePair<int?, string> (a.Id, a.Name)).ToArray ();
            }

            return null;
        }
    }
}
