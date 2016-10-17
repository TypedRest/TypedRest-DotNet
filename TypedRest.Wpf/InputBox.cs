using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace TypedRest.Wpf
{
    /// <summary>
    /// Simple text input window.
    /// </summary>
    public static class InputBox
    {
        /// <summary>
        /// Asks the user to enter a string.
        /// </summary>
        /// <param name="prompt">The prompt to show to the user.</param>
        /// <param name="owner">The parent window. May be <c>null</c>.</param>
        /// <returns>The string the user entered or <c>null</c> if the user closed the window without pressing OK.</returns>
        public static string Show(string prompt, Window owner = null)
        {
            var window = new Window
            {
                Owner = owner,
                Title = Assembly.GetCallingAssembly().GetName().Name,
                Height = 150,
                Width = 400
            };

            var promptTextBlock = new TextBlock
            {
                Text = prompt,
                Margin = new Thickness(7),
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var inputBox = new TextBox
            {
                TabIndex = 0,
                Width = 350,
                Margin = new Thickness(7),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var okButton = new Button
            {
                Content = "OK",
                IsDefault = true,
                TabIndex = 1,
                Width = 70,
                Height = 30,
                Margin = new Thickness(7),
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            okButton.Click += delegate
            {
                window.DialogResult = true;
                window.Close();
            };

            window.Content = new StackPanel {Children = {promptTextBlock, inputBox, okButton}};
            return (window.ShowDialog() ?? false) ? inputBox.Text : null;
        }
    }
}