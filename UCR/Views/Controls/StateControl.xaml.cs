using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.ViewModels;

namespace HidWizards.UCR.Views.Controls
{
    public partial class StateControl : UserControl
    {
        public static readonly DependencyProperty PluginPropertyProperty = DependencyProperty.Register("PluginProperty", typeof(PluginProperty), typeof(StateControl), new PropertyMetadata(default(PluginProperty)));

        public PluginProperty PluginProperty
        {
            get { return (PluginProperty)GetValue(PluginPropertyProperty); }
            set { SetValue(PluginPropertyProperty, value); }
        }

        public ObservableCollection<ComboBoxItemViewModel> States { get; set; }

        public StateControl()
        {
            InitializeComponent();
            Loaded += UserControl_Loaded;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (PluginProperty == null) return;
            PopulateStates();
        }

        private void PopulateStates()
        {
            States = new ObservableCollection<ComboBoxItemViewModel>();
            ComboBoxItemViewModel selectedState = null;
            foreach (var profileState in PluginProperty.Plugin.Profile.AllStates)
            {
                var comboBoxItem = new ComboBoxItemViewModel(profileState.Title, profileState);
                States.Add(comboBoxItem);
                if (profileState.Guid.Equals((Guid)PluginProperty.Property)) selectedState = comboBoxItem;
            }
            StateComboBox.ItemsSource = States;
            if (selectedState != null)
            {
                StateComboBox.SelectedItem = selectedState;
            }
            else
            {
                StateComboBox.SelectedIndex = 0;
            }
        }


        private void StateComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PluginProperty.Property = ((State)((ComboBoxItemViewModel) StateComboBox.SelectedItem).Value).Guid;
        }
    }
}
