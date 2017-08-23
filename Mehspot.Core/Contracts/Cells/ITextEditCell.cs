namespace Mehspot.Core.Builders
{

    public interface ITextEditCell : IViewCell
    {
        bool Editable { get; set; }
        bool Hidden { get; set; }
        bool IsValid { get; }
        string Text { get; set; }
        bool Multiline { get; set; }
        int? MaxLength { get; set; }
        void SetKeyboardType(KeyboardType type);
    }

}