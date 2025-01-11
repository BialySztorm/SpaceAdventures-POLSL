using System.Collections.Generic;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Settings;

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
        public List<string> GetTableEntryKeysStartingWith(string prefix)
        {
            var table = LocalizationSettings.StringDatabase.GetTable(_tableName);
            var collection = LocalizationEditorSettings.GetStringTableCollection(_tableName);
            var entries = new List<string>();

            foreach (var entry in table)
            {
                var key = collection.SharedData.GetKey(entry.Key);
                if (key.StartsWith(prefix))
                {
                    entries.Add(key);
                }
            }

            return entries;
        }

        public int GetTableEntryCountWith(string prefix = "", string suffix= "")
        {
            var table = LocalizationSettings.StringDatabase.GetTable(_tableName);
            var collection = LocalizationEditorSettings.GetStringTableCollection(_tableName);
            var count = 0;

            foreach (var entry in table)
            {
                var key = collection.SharedData.GetKey(entry.Key);
                if (key.StartsWith(prefix) && key.EndsWith(suffix))
                {
                    count++;
                }
            }

            return count;
        }
        
        public string GetTableEntry(string key)
        {
            _localizedTable.TableEntryReference = key;
            return _localizedTable.GetLocalizedString();
        }
        
    }
}