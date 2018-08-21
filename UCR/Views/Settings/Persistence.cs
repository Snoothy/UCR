using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using HidWizards.UCR.Views;
using HidWizards.UCR.ViewModels;

namespace HidWizards.UCR.Views.Settings
{
    public interface IPersistence
    {
        #region init
        #endregion init

        #region Constructor

        void IPersistence();

        #endregion Constructor

        #region CreateSettings

        IUCRSettings CreateSettings();

        #endregion CreateSettings

        #region Overrides

        void OnLocationChanged(EventArgs e);

        void OnRenderSizeChanged(SizeChangedInfo info);

        void OnStateChanged(EventArgs e);

        #endregion Overrides

        #region Helpers

        void ApplySettings();

        #endregion Helpers
    }
}