using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OsEngine.Views
{
    public class MyDecimalTextBox : TextBox
    {
        public MyDecimalTextBox()
        {
            PreviewTextInput += MyDecimalTextBox_PreviewTextInput; ; // вводится текст
            TextChanged += MyDecimalTextBox_TextChanged; ; // изменение текста
        }

        private void MyDecimalTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            //tb.Select(tb.Text.Length, 0); // ставим каретку в конец слова
        }

        private void MyDecimalTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // является цифрой и разрешаем вводить точку и разрешаем вводить минус
            if (!Char.IsDigit(e.Text, 0) && !e.Text.Contains(".") && !e.Text.Contains("-"))
            {
                e.Handled = true;
            }
        }
    }
}
