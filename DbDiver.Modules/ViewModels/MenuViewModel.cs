using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

//
namespace DbDiver.Modules.ViewModels
{
    public class MenuViewModel : BindableBase
    {
        public DelegateCommand ShowAboutWindowCommand { get; private set; }

        public MenuViewModel()
        {
            ShowAboutWindowCommand = new DelegateCommand(ShowAboutWindow);
        }

        public void ShowAboutWindow()
        {
            AboutWindow wind = new AboutWindow();
            wind.ShowDialog();

        }
    }
}
