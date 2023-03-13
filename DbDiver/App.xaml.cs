using DbDiver.ViewModels;
using DbDiver.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using DbDiver.Modules.Searcher;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DbDiver.DAL;
using DbDiver.Core;
namespace DbDiver
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry){}

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<SearcherModule>();
        }

    }
}
