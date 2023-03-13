using DbDiver.Business;
using DbDiver.Core;
using DbDiver.Core.Events;
using DbDiver.DAL;
using DbDiver.Modules.Exceptions;
using DbDiver.Modules.Models;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace DbDiver.Modules.ViewModels
{
    public class DiverParametersViewModel : BindableBase
    {
        private string _databasePath;
        private int _selectedDatabaseIdx;
        public int SelectedDatabaseIdx
        {
            get { return _selectedDatabaseIdx; }
            set
            {
                SetProperty(ref _selectedDatabaseIdx, value);
            }
        }
        
        public string DatabasePath
        {
            get { return _databasePath; }
            set { 
                SetProperty(ref _databasePath, value);
                DiveCommand.RaiseCanExecuteChanged();
            }
        }

        private string _tableName;
        public string TableName
        {
            get { return _tableName; }
            set { 
                SetProperty(ref _tableName, value);
                AddItemCommand.RaiseCanExecuteChanged();
            }
        }
        
        private string _searchItem;
        public string SearchItem
        {
            get { return _searchItem; }
            set { 
                SetProperty(ref _searchItem, value); 
                AddItemCommand.RaiseCanExecuteChanged();
            }
        }
        
        private string _columnName;
        public string ColumnName
        {
            get { return _columnName; }
            set { 
                SetProperty(ref _columnName, value);
                AddItemCommand.RaiseCanExecuteChanged();
            }
        }
        
        private string _descripion;
        private readonly IEventAggregator _eventAggregator;

        public string Description
        {
            get { return _descripion; }
            set
            {
                SetProperty(ref _descripion, value);
                AddItemCommand.RaiseCanExecuteChanged();
            }
        }

        public DiverParametersViewModel(IEventAggregator eventAggregator)
        {
            AddItemCommand = new DelegateCommand(AddItem, CanBeAdded);
            DiveCommand = new DelegateCommand(Dive, CanPerformDive);
            ScheduleDiveCommand = new DelegateCommand(ScheduleDive, CanPerformScheduledDive);
            SearchParameters = new ObservableCollection<DbSearchParameter>();
            BrowseCommand = new DelegateCommand(BrowseSqlFile);
            SaveItemsCommand = new DelegateCommand(SaveItems);
            LoadItemsCommand = new DelegateCommand(LoadItems);
            Settings settings = new Settings();
            try
            {
                settings.Load();
                SearchParameters = settings.Parameters;
                DatabasePath = settings.DatabasePath;
                SelectedDatabaseIdx = settings.SelectedDatabaseIdx;

            }
            catch (SettingsNotFoundException)
            {
                MessageBox.Show("Settings file not found");
            }
            catch
            {
                MessageBox.Show("Settings loading error");
            }
            SetStatusNotSearched();
            _eventAggregator = eventAggregator;
        }

        ~DiverParametersViewModel()
        {
            Settings settings = new Settings(SearchParameters, DatabasePath, SelectedDatabaseIdx);
            settings.Save();
            
        }

        private void AddItem() =>       
            SearchParameters.Add(new DbSearchParameter(TableName, ColumnName, SearchItem, Description, null, null));
        

        private void BrowseSqlFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            DatabasePath = ofd.FileName;
            SetStatusNotSearched();
        }

        private bool CanBeAdded()
        {
            return ColumnName?.Length >0 && TableName?.Length>0 && SearchItem?.Length>0 && Description?.Length > 0; 
        }

        public DelegateCommand AddItemCommand { get; private set; }
        public DelegateCommand DiveCommand { get; private set; }
        public DelegateCommand ScheduleDiveCommand { get; private set; }

        public DelegateCommand BrowseCommand { get; private set; }
        public DelegateCommand LoadItemsCommand { get;private set; }
        public DelegateCommand SaveItemsCommand { get; private set; }

        public ObservableCollection<DbSearchParameter> SearchParameters { set; get; }
        public ObservableCollection<string> LogItems { set; get; } = new ObservableCollection<string>();

        bool diveStarted = false;
        public async void Dive()
        {
           await Task.Run(() =>
           {
               diveStarted = true;
               _eventAggregator.GetEvent<DiveStartedEvent>().Publish(SearchParameters.Count);
               int foundCount = 0;
               if (!string.IsNullOrEmpty(DatabasePath))
               {
                   AddLogMessage($"dive started");

                   IDatabaseItemsExtractor databaseItemsExtractor = null;
                   try
                   {
                       switch (SelectedDatabaseIdx)
                       {
                           case 0:
                               {
                                   databaseItemsExtractor = new SqliteItemsExtractor($"Data Source = {DatabasePath};"); 
                                   break;
                               };
                           case 1:
                               {
                                   var database = Path.GetFileName(DatabasePath);
                                   var server = Path.GetDirectoryName(DatabasePath);
                                   databaseItemsExtractor = new MsSqlItemsExtractor($"Server={server};Database={database};");
                                   break;
                               };
                       };
                       var databaseSearcher = new DatabaseSearcher(_eventAggregator, databaseItemsExtractor);
                       foundCount = databaseSearcher.InspectValues(SearchParameters, AddLogMessage);
                   }
                   catch (Exception exc) { 
                   
                       MessageBox.Show($"Error database initialization: {exc.Message}");
                       AddLogMessage($"Error database initialization: {exc.Message}");
                   }
                  
                   AddLogMessage($"program finished with {foundCount} found values");
               }
               RaisePropertyChanged("SearchParameters");
               _eventAggregator.GetEvent<DiveFinishedEvent>().Publish();
               diveStarted = false;
           });
        }

        bool scheduledDiveStarted = false;
        Timer timer = null;

        public async void ScheduleDive()
        {
            await Task.Run(() =>
            {
                if (!scheduledDiveStarted)
                {
                    _eventAggregator.GetEvent<ScheduledDiveStartedEvent>().Publish(SearchParameters.Count);
                    AddLogMessage("scheduled dive with interval of 1 minute started");
                    timer = new(60000);
                    timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
                    timer.Enabled = true;
                    scheduledDiveStarted = true;
                }
                else
                {
                    timer.Stop();
                    _eventAggregator.GetEvent<ScheduledDiveFinishedEvent>().Publish();
                    AddLogMessage("scheduled dive stopped");
                    scheduledDiveStarted = false;
                }
            });

        }
        public async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await Task.Run(() =>
            {
                int foundCount = 0;
                if (!string.IsNullOrEmpty(DatabasePath))
                {
                    AddLogMessage($"dive started");

                    IDatabaseItemsExtractor databaseItemsExtractor = null;
                    try
                    {
                        switch (SelectedDatabaseIdx)
                        {
                            case 0:
                                {
                                    databaseItemsExtractor = new SqliteItemsExtractor($"Data Source = {DatabasePath};");
                                    break;
                                };
                            case 1:
                                {
                                    var database = Path.GetFileName(DatabasePath);
                                    var server = Path.GetDirectoryName(DatabasePath);
                                    databaseItemsExtractor = new MsSqlItemsExtractor($"Server={server};Database={database};");
                                    break;
                                };
                        };
                        var databaseSearcher = new DatabaseSearcher(_eventAggregator, databaseItemsExtractor);
                        foundCount = databaseSearcher.InspectValues(SearchParameters, AddLogMessage);
                    }
                    catch (Exception exc)
                    {

                        MessageBox.Show($"Error database initialization: {exc.Message}");
                        AddLogMessage($"Error database initialization: {exc.Message}");
                    }

                    AddLogMessage($"program finished with {foundCount} found values");
                }
                RaisePropertyChanged("SearchParameters");
            });

        }

        public bool CanPerformDive() => (DatabasePath?.Length > 0) && scheduledDiveStarted == false;
        
        public bool CanPerformScheduledDive() => diveStarted == false;

        private void LoadItems()
        {
            try
            {
                SearchParameters = new ObservableCollection<DbSearchParameter>(ItemsLoader.LoadItems());
                RaisePropertyChanged("SearchParameters");
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            
        }

        private void SaveItems()
        {
            try
            {
                ItemsLoader.SaveItems(SearchParameters);
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        public void SetStatusNotSearched()
        {
            foreach (var v in SearchParameters)
            {
                v.Status = "Not searched";
            }
        }

        public void AddLogMessage(string message)
        {
            Application.Current.Dispatcher.Invoke(
            () =>
            {
                string dot = message.EndsWith(".") ? string.Empty : ".";
                LogItems.Add($"{DateTime.Now}: {message}{dot}");
                RaisePropertyChanged("LogItems");
            });        
        }
    }
}
