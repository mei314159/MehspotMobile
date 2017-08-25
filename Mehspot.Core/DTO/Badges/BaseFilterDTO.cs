namespace Mehspot.Core.DTO.Search
{
    public interface IBaseFilterDTO
    {
        int? DistanceFrom { get; set; }

        string ZipCode { get; set; }

        bool? HasPicture { get; set; }

        bool? HasReferences { get; set; }
    }

    public class BaseFilterDTO: IBaseFilterDTO
    {
        [Cell(Label = "Max Distance", CellType = CellType.Range, MinValue = 0, MaxValue = 200, DefaultValue = 20, Order = -2)]
        public virtual int? DistanceFrom { get; set; }

        public string ZipCode { get; set; }

        [Cell(Label = "Has Profile Picture", CellType = CellType.Boolean, Order = 999)]
        public bool? HasPicture { get; set; }

        [Cell(Label = "Has References", CellType = CellType.Boolean, Order = 1000)]
        public bool? HasReferences { get; set; }
    }


    public class BabysitterFilterDTO : IBaseFilterDTO
    {
        [Cell(Label = "Max Distance", CellType = CellType.Range, MinValue = 0, MaxValue = 25, DefaultValue = 20, Order = -2)]
        public int? DistanceFrom { get; set; }

		public string ZipCode { get; set; }

		[Cell(Label = "Has Profile Picture", CellType = CellType.Boolean, Order = 999)]
		public bool? HasPicture { get; set; }

		[Cell(Label = "Has References", CellType = CellType.Boolean, Order = 1000)]
		public bool? HasReferences { get; set; }
    }
}
