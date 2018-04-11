using System;
using System.Windows;
using System.Windows.Input;

namespace HidWizards.UCR.Utilities.Commands
{
    // TODO Refactor
    public static class MyCommands
    {
        private static readonly ICommand appCloseCmd = new ApplicationCloseCommand();
        public static ICommand ApplicationCloseCommand
        {
            get { return appCloseCmd; }
        }
    }

    //===================================================================================================
    public class ApplicationCloseCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            // You may not need a body here at all...
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return Application.Current != null && Application.Current.MainWindow != null;
        }

        public void Execute(object parameter)
        {
            Application.Current.MainWindow.Close();
        }
    }
}
