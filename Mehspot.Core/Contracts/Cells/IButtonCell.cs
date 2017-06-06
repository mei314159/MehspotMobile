using System;
namespace Mehspot.Core.Builders
{
    public interface IButtonCell: IViewCell
    {
        event Action<object> OnButtonTouched;
    }
}
