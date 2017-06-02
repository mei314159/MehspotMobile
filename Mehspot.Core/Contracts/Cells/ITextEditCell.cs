namespace Mehspot.Core.Builders
{

    public interface ITextEditCell : IViewCell
    {
        bool IsValid { get; }
        void SetKeyboardType(KeyboardType type);
    }

}