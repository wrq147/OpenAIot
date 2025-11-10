using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace AirJointUI.Component
{
    public class SwitchButton : CheckBox
    {
        public static readonly StyledProperty<string> ContentFalseProperty =
    AvaloniaProperty.Register<SwitchButton, string>(nameof(ContentFalse), string.Empty);

        public static readonly StyledProperty<string> ContentTrueProperty =
AvaloniaProperty.Register<SwitchButton, string>(nameof(ContentTrue), string.Empty);

        public string ContentFalse
        {
            get { return GetValue(ContentFalseProperty); }
            set { SetValue(ContentFalseProperty, value); }
        }

        public string ContentTrue
        {
            get { return GetValue(ContentTrueProperty); }
            set { SetValue(ContentTrueProperty, value); }
        }
    }
}
