using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Models.Plugin;
using HidWizards.UCR.ViewModels;

namespace HidWizards.UCR.Views.Profile
{
    public partial class PluginDialog: Window
    {
        public string TextResult { get; set; }
        public Plugin Plugin { get; set; }

        private List<Plugin> Plugins;

        public PluginDialog(Context context, string question, string answer="")
        {
            InitializeComponent();
            Title = question;
            TxtAnswer.Text = answer;
            Plugins = context.GetPlugins();
            Plugins.Sort();
            InitComboBox();
        }

        private void InitComboBox()
        {
            var plugins = new ObservableCollection<ComboBoxItemViewModel>();
            foreach (var plugin in Plugins)
            {
                plugins.Add(new ComboBoxItemViewModel(plugin.PluginName(), plugin));
            }
            PluginsListBox.ItemsSource = plugins;
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
                Plugin = ((ComboBoxItemViewModel) PluginsListBox.SelectedItem)?.Value;
                if (Plugin == null)
                {
                    MessageBox.Show("Please select a plugin from the list", "No plugin selected",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    DialogResult = true;
                    TextResult = TxtAnswer.Text;
                }
            }
            else
            {
                MessageBox.Show("Please fill out the field", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            
        }
    }
}
