using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DbDiver.Core
{
    public class DbSearchParameter : INotifyPropertyChanged
    {
        private string _tableName;
        private string _columnName;
        private string _searchItem;
        private string _description;
        private string _status = "Not searched";
        private string _firstFoundDate;
        private string _lastFoundDate;

        public DbSearchParameter() {}

        public DbSearchParameter(string tableName, string columnName, string searchItem, string description, string firstFoundDate, string lastFoundDate)
        {
            TableName = tableName;
            ColumnName = columnName;
            SearchItem = searchItem;
            Description = description;
            _firstFoundDate = firstFoundDate;
            _lastFoundDate = lastFoundDate;
        }

        [DisplayName("Table name")]
        public string TableName { get => _tableName; set { _tableName = value; OnPropertyChanged(); } }

        [DisplayName("Column name")]
        public string ColumnName { get => _columnName; set { _columnName = value; OnPropertyChanged(); } }

        [DisplayName("Search item")]
        public string SearchItem { get => _searchItem; set { _searchItem = value; OnPropertyChanged(); }}

        [DisplayName("Description")]
        public string Description { get => _description; set { _description = value; OnPropertyChanged(); } }

        [DisplayName("Status")]
        public string Status { get => _status; set { _status = value; OnPropertyChanged(); } }
        
        [DisplayName("First found")]
        public string FirstFound { get => _firstFoundDate; set { _firstFoundDate = value; OnPropertyChanged(); } }
        
        [DisplayName("Last found")]
        public string LastFound { get => _lastFoundDate; set { _lastFoundDate = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
