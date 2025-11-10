using AirJointUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;

namespace AirJointUI.Component
{
    public partial class CharacterKeyboard : Window
    {
        private bool _isCaps;
        public CharacterKeyboard(string txt)
        {
            InitializeComponent();
            _isCaps = false;
            TextBox iptTxt = this.FindControl<TextBox>("txtResult");
            iptTxt.Text = txt;
            this.FindControl<Button>("btnCase").Content = Properties.Resources.ResourceManager.GetString("CharacterKeyboardNormal", I18NExt.Culture);
            this.FindControl<Image>("imgClear").IsVisible = iptTxt.Text != null && iptTxt.Text.Length > 0;
        }

        private void ClearClick(object sender, PointerPressedEventArgs e)
        {
            TextBox iptTxt = this.FindControl<TextBox>("txtResult");
            iptTxt.Text = "";
            this.FindControl<Image>("imgClear").IsVisible = false;
        }
        private void HandleClick(object sender, RoutedEventArgs e)
        {
            Button clkBtn = (Button)sender;
            TextBox iptTxt = this.FindControl<TextBox>("txtResult");
            switch (clkBtn.Name)
            {
                case "btnKG":
                    iptTxt.Text = iptTxt.Text + " ";
                    break;
                case "btnCase":
                    _isCaps = !_isCaps;
                    if (_isCaps == false)
                    {
                        clkBtn.Content = Properties.Resources.ResourceManager.GetString("CharacterKeyboardNormal", I18NExt.Culture);
                        clkBtn.Background = Brush.Parse("#f5f5f5");
                        clkBtn.Foreground = Brush.Parse("#000");
                        this.FindControl<Button>("btnQ").Content = "q";
                        this.FindControl<Button>("btnW").Content = "w";
                        this.FindControl<Button>("btnE").Content = "e";
                        this.FindControl<Button>("btnR").Content = "r";
                        this.FindControl<Button>("btnT").Content = "t";
                        this.FindControl<Button>("btnY").Content = "y";
                        this.FindControl<Button>("btnU").Content = "u";
                        this.FindControl<Button>("btnI").Content = "i";
                        this.FindControl<Button>("btnO").Content = "o";
                        this.FindControl<Button>("btnP").Content = "p";
                        this.FindControl<Button>("btnA").Content = "a";
                        this.FindControl<Button>("btnS").Content = "s";
                        this.FindControl<Button>("btnD").Content = "d";
                        this.FindControl<Button>("btnF").Content = "f";
                        this.FindControl<Button>("btnG").Content = "g";
                        this.FindControl<Button>("btnH").Content = "h";
                        this.FindControl<Button>("btnJ").Content = "j";
                        this.FindControl<Button>("btnK").Content = "k";
                        this.FindControl<Button>("btnL").Content = "l";
                        this.FindControl<Button>("btnZ").Content = "z";
                        this.FindControl<Button>("btnX").Content = "x";
                        this.FindControl<Button>("btnC").Content = "c";
                        this.FindControl<Button>("btnV").Content = "v";
                        this.FindControl<Button>("btnB").Content = "b";
                        this.FindControl<Button>("btnN").Content = "n";
                        this.FindControl<Button>("btnM").Content = "m";
                    }
                    else
                    {
                        clkBtn.Content = Properties.Resources.ResourceManager.GetString("CharacterKeyboardCaps", I18NExt.Culture);
                        clkBtn.Background = Brush.Parse("#67C23A");
                        clkBtn.Foreground = Brush.Parse("#fff");
                        this.FindControl<Button>("btnQ").Content = "Q";
                        this.FindControl<Button>("btnW").Content = "W";
                        this.FindControl<Button>("btnE").Content = "E";
                        this.FindControl<Button>("btnR").Content = "R";
                        this.FindControl<Button>("btnT").Content = "T";
                        this.FindControl<Button>("btnY").Content = "Y";
                        this.FindControl<Button>("btnU").Content = "U";
                        this.FindControl<Button>("btnI").Content = "I";
                        this.FindControl<Button>("btnO").Content = "O";
                        this.FindControl<Button>("btnP").Content = "P";
                        this.FindControl<Button>("btnA").Content = "A";
                        this.FindControl<Button>("btnS").Content = "S";
                        this.FindControl<Button>("btnD").Content = "D";
                        this.FindControl<Button>("btnF").Content = "F";
                        this.FindControl<Button>("btnG").Content = "G";
                        this.FindControl<Button>("btnH").Content = "H";
                        this.FindControl<Button>("btnJ").Content = "J";
                        this.FindControl<Button>("btnK").Content = "K";
                        this.FindControl<Button>("btnL").Content = "L";
                        this.FindControl<Button>("btnZ").Content = "Z";
                        this.FindControl<Button>("btnX").Content = "X";
                        this.FindControl<Button>("btnC").Content = "C";
                        this.FindControl<Button>("btnV").Content = "V";
                        this.FindControl<Button>("btnB").Content = "B";
                        this.FindControl<Button>("btnN").Content = "N";
                        this.FindControl<Button>("btnM").Content = "M";
                    }
                    break;
                case "btnClose":
                    Close(null);
                    break;
                case "btnEnter":
                    Close(iptTxt.Text);
                    break;
                case "btnDel":
                    if (iptTxt.Text.Length > 1)
                    {
                        iptTxt.Text = iptTxt.Text.Substring(0, iptTxt.Text.Length - 1);
                    }
                    else
                    {
                        iptTxt.Text = "";
                    }
                    break;
                default:
                    iptTxt.Text = iptTxt.Text + clkBtn.Content;
                    break;
            }
            this.FindControl<Image>("imgClear").IsVisible = iptTxt.Text != null && iptTxt.Text.Length > 0;
        }
    }
}
