using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OsEngine.Views
{
    public class IntTextBox : TextBox // наследует всё что есть в TextBox
    {
        public IntTextBox()
        {
            PreviewTextInput += IntTextBox_PreviewTextInput; // вводится текст
            TextChanged += IntTextBox_TextChanged; // изменение текста
        }

        private void IntTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.Select(tb.Text.Length, 0); // ставим каретку в конец слова
        }

        private void IntTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) // является цифрой или нет
            {
                e.Handled = true;
            }
        }
    }
}
