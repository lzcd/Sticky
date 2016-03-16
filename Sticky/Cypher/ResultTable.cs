using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticky.Cypher
{
    public class ResultTable
    {
        public Dictionary<string, int> ColumnOrdinalByName { get; } = new Dictionary<string, int>();

        private Dictionary<int, Dictionary<int, string>> dataByColumnOrdinalByRowIndex = new Dictionary<int, Dictionary<int, string>>();

        public int RowCount { get; private set; }

        public int AddRow()
        {
            RowCount++;
            return RowCount - 1;
        }

        public string this[int rowIndex, string columnName]
        {
            get
            {
                var columnOrdinal = ColumnOrdinalByName[columnName];
                return this[rowIndex, columnOrdinal];
            }
            set
            {
                var columnOrdinal = default(int);
                if (!ColumnOrdinalByName.TryGetValue(columnName, out columnOrdinal))
                {
                    columnOrdinal = ColumnOrdinalByName.Values.Count;
                    ColumnOrdinalByName.Add(columnName, columnOrdinal);
                }
                this[rowIndex, columnOrdinal] = value;
            }
        }

        public string this[int rowIndex, int columnOrdinal]
        {
            get
            {
                var data = default(string);
                if (!TryGetDataByOrdinals(rowIndex, columnOrdinal, out data))
                {
                    return null;
                }
                return data;
            }
            set
            {
                SetData(rowIndex, columnOrdinal, value);
            }
        }

        public bool TryGetDataByOrdinals(int rowIndex, int columnOrdinal, out string data)
        {
            data = default(string);

            var dataByColumnOrdinal = default(Dictionary<int, string>);
            if (!dataByColumnOrdinalByRowIndex.TryGetValue(rowIndex, out dataByColumnOrdinal))
            {
                return false;
            }
            if (!dataByColumnOrdinal.TryGetValue(columnOrdinal, out data))
            {
                return false;
            }
            return true;
        }

        public void SetData(int rowIndex, int columnOrdinal, string data)
        {
            var dataByColumnOrdinal = default(Dictionary<int, string>);
            if (!dataByColumnOrdinalByRowIndex.TryGetValue(rowIndex, out dataByColumnOrdinal))
            {
                dataByColumnOrdinal = new Dictionary<int, string>();
                dataByColumnOrdinalByRowIndex.Add(rowIndex, dataByColumnOrdinal);
            }
            dataByColumnOrdinal[columnOrdinal] = data;
        }
    }
}
