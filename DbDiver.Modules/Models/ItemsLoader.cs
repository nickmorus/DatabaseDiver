using DbDiver.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

//loads
namespace DbDiver.Modules.Models
{
    public static class ItemsLoader
    {
        public static IEnumerable<DbSearchParameter> LoadItems()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            var filePath = ofd.FileName;
            var items = File.ReadAllLines(filePath);
            var result = new ObservableCollection<DbSearchParameter>();
            var regDescr = new Regex($"//.*");

            for (int i = 0; i < items.Length; i++)
            {
                var descrMatch = regDescr.Match(items[i]).ToString();
                var description = descrMatch.Replace("//", string.Empty);
                items[i] = items[i].Replace(descrMatch, string.Empty);

                var splittedItem = items[i].Split('|');
                var table = splittedItem[0];
                var column = splittedItem[1];
                var search = splittedItem[2];
                var firstFound = splittedItem[3];
                var lastFound = splittedItem[4];
                result.Add(new DbSearchParameter(table, column, search, description, firstFound, lastFound));
            }
            return result;
        }

        public static void SaveItems(IEnumerable<DbSearchParameter> searchParameters)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.ShowDialog();
                var filePath = sfd.FileName;
                var itemsStr = new List<string>();
                foreach (var item in searchParameters)
                {
                    itemsStr.Add($"{item.TableName}|{item.ColumnName}|{item.SearchItem}|{item.FirstFound}|{item.LastFound}//{item.Description}");
                }
                File.WriteAllLines(filePath, itemsStr);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
