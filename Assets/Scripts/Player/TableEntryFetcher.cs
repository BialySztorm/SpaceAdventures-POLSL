using System;
using Entities;
using UnityEngine;
using UnityEngine.Localization;


namespace Player
{
    public class TableEntryFetcher : MonoBehaviour
    {
        private LocalizedString _localizedTable = new LocalizedString();
        private string _tableName;
        public void SetTable(string tableName)
        {
            _tableName = tableName;
            _localizedTable.TableReference = tableName;
        }

        public int GetTableEntityCountWith(string entity = "")
        {
            if (Enum.TryParse(typeof(EntitiesTable), entity, out var result))
            {
                return (int)result;
            }
            throw new ArgumentException("Invalid string value");
        }
        
        public string GetTableEntry(string key)
        {
            _localizedTable.TableEntryReference = key;
            return _localizedTable.GetLocalizedString();
        }
        
    }
}