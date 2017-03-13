using System.Collections.Generic;

namespace Mehspot.Core.DTO.Search
{
    public class SearchBabysitterDTO : SearchFilterDTO
    {
        public string HourlyRate { get; set; }

        public bool HasCar { get; set; }

        public bool HasCertifications { get; set; }

        public int? AgeRange { get; set; }


        public override string ToString ()
        {
            var parameters = new List<string> ();
            if (!string.IsNullOrWhiteSpace (HourlyRate)) {
                parameters.Add ($"Details.HourlyRate~lte~'{HourlyRate}'");
            }

            if (AgeRange.HasValue && AgeRange > 0) {
                parameters.Add ($"AgeRange~eq~'{AgeRange}'");
            }

            if (HasCar) {
                parameters.Add ($"HasCar~eq~'true'");
            }

            if (HasCertifications) {
                parameters.Add ($"HasCertifications~eq~'true'");
            }

            var parametersString = string.Join ("~and~", parameters);
            return $"{base.ToString ()}~and~{parametersString}";
        }
    }
}
