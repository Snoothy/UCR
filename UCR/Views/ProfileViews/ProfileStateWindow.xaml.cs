using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.Views.ProfileViews
{
    /// <summary>
    /// Interaction logic for ProfileStateWindow.xaml
    /// </summary>
    public partial class ProfileStateWindow : Window
    {
        private Context _context;
        private Profile _profile;

        public ObservableCollection<State> States { get; set; }

        public ProfileStateWindow(Context context, Profile profile)
        {
            _context = context;
            _profile = profile;
            DataContext = this;
            
            InitializeStates();
            
            InitializeComponent();
            Title = "Manage states: " + profile.Title;
        }

        private void InitializeStates()
        {
            States = new ObservableCollection<State>();
            foreach (var state in _profile.AllStates)
            {
                States.Add(state);
            }
        }

        private void AddState_OnClick(object sender, RoutedEventArgs e)
        {
            AddState();
        }

        private void AddState()
        {
            if (string.IsNullOrEmpty(StateTextBox.Text))
            {
                MessageBox.Show("Please input a name for the state", "Missing state name!", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return;
            }

            var state = _profile.AddState(StateTextBox.Text);
            if (state == null)
            {
                MessageBox.Show("The state name already exists", "Failed to add state!", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return;
            }

            StateTextBox.Text = "";
            States.Add(state);
        }

        private void RemoveState_OnClick(object sender, RoutedEventArgs e)
        {
            var state = StateListBox.SelectedItem as State;

            if (state == null)
            {
                MessageBox.Show("Select a state to remove it", "No state selected!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            if (!_profile.RemoveState(state))
            {
                MessageBox.Show("The selected state cannot be removed from the profile", "Failed to remove state!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            States.Remove(state);
        }

        private void AddState_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddState();
            }
        }
    }
}
