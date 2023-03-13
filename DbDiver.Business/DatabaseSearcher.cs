using DbDiver.Core;
using DbDiver.Core.Events;
using DbDiver.DAL;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbDiver.Business
{
    public class DatabaseSearcher : BindableBase
    {
        IEventAggregator _eventAggregator;
        IDatabaseItemsExtractor _databaseItemsExtractor;
        public DatabaseSearcher(IEventAggregator eventAggregator, IDatabaseItemsExtractor databaseItemsExtractor)
        {
            _eventAggregator = eventAggregator;
            _databaseItemsExtractor = databaseItemsExtractor;
        }

        public int InspectValues(IEnumerable<DbSearchParameter> SearchParameters, Action<string> AddLogMessage)
        {
            int foundCounter = 0;
            foreach (var parameter in SearchParameters)
            {
                try
                {
                    var foundValues = _databaseItemsExtractor.FindValues(parameter);
                    if (foundValues.Count() > 0)
                    {
                        foreach(var value in foundValues)
                        {
                            string logMessage = $"found {value} in {parameter.TableName}";
                            AddLogMessage(logMessage);
                            foundCounter++;
                        }

                        parameter.Status = SearchStausMessages.FoundMessage;
                        parameter.LastFound = DateTime.Now.ToString();
                        if (string.IsNullOrEmpty(parameter.FirstFound))
                        {
                            parameter.FirstFound = parameter.LastFound;
                            RaisePropertyChanged("SearchParameters");
                        }
                    }
                    else
                    {
                        parameter.Status = SearchStausMessages.NotFoundMessage;
                    }

                }
                catch (Exception exc)
                {
                    parameter.Status = SearchStausMessages.ErrorMessage;
                   AddLogMessage($"Exception while searching '{parameter.SearchItem}' in [{parameter.TableName}]:[{parameter.ColumnName}] : {exc.Message}");
                }
                _eventAggregator.GetEvent<ItemProcessedEvent>().Publish();         
            }
            return foundCounter;
        }
    }
}
