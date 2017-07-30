using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UCR.Models;

namespace UCR.Views
{
    /// <summary>
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        private UCRContext Ctx { get; set; }
        private Profile Profile { get; set; }

        public ProfileWindow(UCRContext ctx, Profile profile)
        {
            Ctx = ctx;
            Profile = profile;
            InitializeComponent();
            Title = "Edit " + profile.Title;
            DataContext = Profile;
        }
    }
}
