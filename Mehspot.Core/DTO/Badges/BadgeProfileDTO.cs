namespace MehSpot.Models.ViewModels
{
    public class BadgeProfileDTO<T> where T: IBadgeProfileValues
    {
        public T Values { get; set; }
        public BadgeProfileDetailsDTO Details { get; set; }
    }
}