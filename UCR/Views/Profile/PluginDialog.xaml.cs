using System;
using System.Collections.ObjectModel;
using System.Windows;
using UCR.Core.Plugin;
using UCR.Core.Utilities;
using UCR.ViewModels;

namespace UCR.Views.Profile
{
    public partial class PluginDialog: Window
    {
        public string TextResult { get; set; }
        public Plugin Plugin { get; set; }

        public PluginDialog(string question, string answer="")
        {
            InitializeComponent();
            Title = question;
            TxtAnswer.Text = answer;
            InitComboBox();
        }

        private void InitComboBox()
        {
            var plugins = new ObservableCollection<ComboBoxItemViewModel>();
            foreach (var plugin in Toolbox.GetEnumerableOfType<Plugin>())
            {
                plugins.Add(new ComboBoxItemViewModel(plugin.PluginName(), plugin));
            }
            PluginsComboBox.ItemsSource = plugins;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            TxtAnswer.SelectAll();
            TxtAnswer.Focus();
        }

        private void BtnDialogOk_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtAnswer.Text))
            {
                DialogResult = true;
                TextResult = TxtAnswer.Text;
                Plugin = ((ComboBoxItemViewModel) PluginsComboBox.SelectedItem).Value;
            }
            else
            {
                MessageBox.Show("Please fill out the field", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            
        }
    }
}
