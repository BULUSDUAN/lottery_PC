using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.DbHelper
{
    internal static class ConvertExtend
    {
        public static DataSet DataReaderToDataSet(this IDataReader reader)
        {
            DataTable table = new DataTable();
            int fieldCount = reader.FieldCount;
            for (int i = 0; i < fieldCount; i++)
            {
                table.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
            }
            table.BeginLoadData();
            object[] values = new object[fieldCount];
            while (reader.Read())
            {
                reader.GetValues(values);
                table.LoadDataRow(values, true);
            }
            table.EndLoadData();
            DataSet set = new DataSet();
            set.Tables.Add(table);
            return set;
        }

        public static SortedList<TKey, TValue> KeyValuePairsToSortedList<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
        {
            SortedList<TKey, TValue> list = new SortedList<TKey, TValue>();
            if ((keyValuePairs != null) && (keyValuePairs != Convert.DBNull))
            {
                foreach (KeyValuePair<TKey, TValue> pair in keyValuePairs)
                {
                    list.Add(pair.Key, pair.Value);
                }
            }
            return list;
        }

        public static KeyValuePair<TKey, TValue>[] ObjectListToKeyValuePairList<TKey, TValue>(this IEnumerable<object> objectList)
        {
            List<KeyValuePair<TKey, TValue>> list = new List<KeyValuePair<TKey, TValue>>();
            if (objectList != null)
            {
                int num = 1;
                TKey key = default(TKey);
                TValue local2 = default(TValue);
                foreach (object obj2 in objectList)
                {
                    if (num == 1)
                    {
                        key = (TKey)obj2;
                        num = 2;
                    }
                    else
                    {
                        local2 = (TValue)obj2;
                        num = 1;
                        list.Add(new KeyValuePair<TKey, TValue>(key, local2));
                    }
                }
            }
            return list.ToArray();
        }
    }
}
