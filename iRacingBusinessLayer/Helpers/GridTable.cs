namespace iRacingBusinessLayer.Helpers
{
    public class GridTable<T> : IDisposable
    {
        private Dictionary<string, T> _actualDictionary = new Dictionary<string, T>();
        private List<Dictionary<string, T>> _dictionaries;

        public List<string> Headers { get; set; }
        public T[,] Table { get; set; }
        public int NumRows
        {
            get
            {
                return Table.GetLength(0);
            }
        }
        public int NumCols
        {
            get
            {
                return Table.GetLength(1);
            }
        }


        public GridTable()
        {
            Headers = new List<string>();
            _actualDictionary = new Dictionary<string, T>();
            _dictionaries = new List<Dictionary<string, T>>();
            Table = new T[0, 0];
        }

        public GridTable(List<string> headers)
        {
            Headers = headers;
            _actualDictionary = new Dictionary<string, T>();
            _dictionaries = new List<Dictionary<string, T>>();
            Table = new T[0, 0];
        }

        public void WriteField(string fieldName, T t)
        {
            _actualDictionary.Add(fieldName, t);
        }

        public void NextRecord()
        {
            if (_actualDictionary.Count == 0) return;

            _dictionaries.Add(_actualDictionary);
            _actualDictionary = new Dictionary<string, T>();
        }

        public void Flush()
        {
            NextRecord();

            if (Headers.Count == 0)
            {
                List<string> allKeys = _dictionaries.SelectMany(x => x.Select(y => y.Key)).ToList();
                Headers = allKeys.Distinct().ToList();
            }

            Table = new T[_dictionaries.Count, Headers.Count];

            for (int i = 0; i < _dictionaries.Count; i++)
            {
                Dictionary<string, T> dictionary = _dictionaries.ElementAt(i);
                for (int j = 0; j < Headers.Count; j++)
                {
                    string header = Headers.ElementAt(j);
                    Table[i, j] = dictionary.GetValueOrDefault(header);
                }
            }
        }

        public void Clear()
        {
            _actualDictionary.Clear();
            _dictionaries.Clear();
            Headers.Clear();
            Table = new T[0, 0];
        }

        public void Dispose()
        {
            Clear();
        }
    }

}
