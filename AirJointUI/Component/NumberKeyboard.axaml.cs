using Avalonia.Controls;
using Avalonia.Interactivity;
using ReactiveUI;
using System;
using System.Globalization;

namespace AirJointUI.Component
{
    public partial class NumberKeyboard : Window
    {
        public NumberKeyboard(string txt)
        {
            InitializeComponent();
            TextBox iptTxt = this.FindControl<TextBox>("txtResult");
            iptTxt.Text = txt;
        }
        private void HandleNumberClick(object sender, RoutedEventArgs e)
        {
            Button clkBtn = (Button)sender;
            TextBox iptTxt = this.FindControl<TextBox>("txtResult");

            switch (clkBtn.Name)
            {
                case "btn0":
                    if (iptTxt.Text != null && iptTxt.Text.Length > 10) { return; }
                    if (iptTxt.Text.Contains('.'))
                    {
                        iptTxt.Text = iptTxt.Text + "0";
                    }
                    else
                    {
                        iptTxt.Text = Convert.ToDouble(iptTxt.Text + "0").ToString();
                    }
                    break;
                case "btn1":
                    if (iptTxt.Text != null && iptTxt.Text.Length > 10) { return; }
                    iptTxt.Text = Convert.ToDouble(iptTxt.Text + "1").ToString();
                    break;
                case "btn2":
                    if (iptTxt.Text != null && iptTxt.Text.Length > 10) { return; }
                    iptTxt.Text = Convert.ToDouble(iptTxt.Text + "2").ToString();
                    break;
                case "btn3":
                    if (iptTxt.Text != null && iptTxt.Text.Length > 10) { return; }
                    iptTxt.Text = Convert.ToDouble(iptTxt.Text + "3").ToString();
                    break;
                case "btn4":
                    if (iptTxt.Text != null && iptTxt.Text.Length > 10) { return; }
                    iptTxt.Text = Convert.ToDouble(iptTxt.Text + "4").ToString();
                    break;
                case "btn5":
                    if (iptTxt.Text != null && iptTxt.Text.Length > 10) { return; }
                    iptTxt.Text = Convert.ToDouble(iptTxt.Text + "5").ToString();
                    break;
                case "btn6":
                    if (iptTxt.Text != null && iptTxt.Text.Length > 10) { return; }
                    iptTxt.Text = Convert.ToDouble(iptTxt.Text + "6").ToString();
                    break;
                case "btn7":
                    if (iptTxt.Text != null && iptTxt.Text.Length > 10) { return; }
                    iptTxt.Text = Convert.ToDouble(iptTxt.Text + "7").ToString();
                    break;
                case "btn8":
                    if (iptTxt.Text != null && iptTxt.Text.Length > 10) { return; }
                    iptTxt.Text = Convert.ToDouble(iptTxt.Text + "8").ToString();
                    break;
                case "btn9":
                    if (iptTxt.Text != null && iptTxt.Text.Length > 10) { return; }
                    iptTxt.Text = Convert.ToDouble(iptTxt.Text + "9").ToString();
                    break;
                case "btnClear":
                    iptTxt.Text = "0";
                    break;
                case "btndot":
                    if (iptTxt.Text != null && iptTxt.Text.Length > 10)
                    {
                        return;
                    }
                    if (iptTxt.Text != null && iptTxt.Text.IndexOf(".") != -1)
                    {
                        return;
                    }
                    if (string.IsNullOrEmpty(iptTxt.Text))
                    {
                        return;
                    }
                    iptTxt.Text = iptTxt.Text + ".";
                    break;
                case "btnDel":
                    if (iptTxt.Text.Length == 1)
                    {
                        iptTxt.Text = "0";
                    }
                    else if (iptTxt.Text.Length > 1)
                    {
                        iptTxt.Text = Convert.ToDouble(iptTxt.Text.Substring(0, iptTxt.Text.Length - 1)).ToString();
                    }
                    break;
                case "btnClose":
                    Close(null);
                    break;
                case "btnEnter":
                    Close(Convert.ToDouble(iptTxt.Text).ToString());
                    break;
            }
        }
    }

}
