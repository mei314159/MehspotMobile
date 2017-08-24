namespace Mehspot.iOS.Controllers.Subdivisions
{

	public class VerifySubdivisionResult
	{
		public VerifySubdivisionResult(int? nameOptionId, int? locationOptionId)
		{
			NameOptionId = nameOptionId;
			AddressOptionId = locationOptionId;
		}

		public int? NameOptionId { get; set; }

		public int? AddressOptionId { get; set; }

		public string NewName { get; set; }

        public string NewAddress { get; set; }
	}
}
