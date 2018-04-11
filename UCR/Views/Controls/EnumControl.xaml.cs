using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.ViewModels;

namespace HidWizards.UCR.Views.Controls
{
    public partial class EnumControl : UserControl
    {
        public static readonly DependencyProperty PluginPropertyProperty = DependencyProperty.Register("PluginProperty", typeof(PluginProperty), typeof(EnumControl), new PropertyMetadata(default(PluginProperty)));

        public PluginProperty PluginProperty
        {
            get { return (PluginProperty)GetValue(PluginPropertyProperty); }
            set { SetValue(PluginPropertyProperty, value); }
        }

        public ObservableCollection<ComboBoxItemViewModel> Enums { get; set; }

        public EnumControl()
        {
            InitializeComponent();
            Loaded += UserControl_Loaded;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (PluginProperty == null) return;
            PopulateEnums();
        }

        private void PopulateEnums()
        {
            Enums = new ObservableCollection<ComboBoxItemViewModel>();
            ComboBoxItemViewModel selectedEnum = null;
            foreach (var enumValue in Enum.GetValues(PluginProperty.Property.GetType()))
            {
                var comboBoxItem = new ComboBoxItemViewModel(enumValue.ToString(), enumValue);
                Enums.Add(comboBoxItem);
                if (enumValue.Equals(PluginProperty.Property)) selectedEnum = comboBoxItem;
            }
            EnumComboBox.ItemsSource = Enums;
            if (selectedEnum != null)
            {
                EnumComboBox.SelectedItem = selectedEnum;
            }
            else
            {
                EnumComboBox.SelectedIndex = 0;
            }
        }


        private void EnumComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PluginProperty.Property = ((ComboBoxItemViewModel) EnumComboBox.SelectedItem).Value;
        }
    }
}
