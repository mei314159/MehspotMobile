using System.Collections.Generic;

namespace Mehspot.Core.DTO.Search
{
    public class SearchFilterDTO
    {
        public string MaxDistance { get; set; }

        public string Zip { get; set; }

        public bool HasPicture { get; set; }

        public bool HasReferences { get; set; }

        public override string ToString () {

            var parameters = new List<string> ();
            if (!string.IsNullOrWhiteSpace (Zip)) {
                parameters.Add ($"Details.ZipCode~eq~'{Zip}'");
            }

            if (!string.IsNullOrWhiteSpace (MaxDistance)) {
                parameters.Add ($"Details.DistanceFrom~lte~'{MaxDistance}'");
            }

            if (HasPicture) {
                parameters.Add ($"Details.HasPicture~eq~'true'");
            }

            if (HasReferences) {
                parameters.Add ($"Details.HasReferences~eq~'true'");
            }

            return string.Join ("~and~", parameters);
        }
    }
    
}
