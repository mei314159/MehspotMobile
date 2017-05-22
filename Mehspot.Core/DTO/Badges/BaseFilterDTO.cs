namespace Mehspot.Core.DTO.Search
{
    public class BaseFilterDTO
    {
        [SearchProperty(Label = "Max Distance", CellType = CellType.Range, MinValue = 0, MaxValue = 200, DefaultValue = 20, Order = -2)]
        public int? DistanceFrom { get; set; }

        [SearchProperty(Label = "Zip Code", CellType = CellType.Text, Mask = "#####", Order = -1)]
        public string ZipCode { get; set; }

        [SearchProperty(Label = "Has Profile Picture", CellType = CellType.Boolean, Order = 999)]
        public bool? HasPicture { get; set; }

        [SearchProperty(Label = "Has References", CellType = CellType.Boolean, Order = 1000)]
        public bool? HasReferences { get; set; }
    }

}
