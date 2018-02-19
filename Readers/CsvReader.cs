using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Starship.Core.Extensions;

namespace Starship.Core.Readers {
    public class CsvReader<T> : CsvReader where T : new() {
        public CsvReader(Stream stream) : base(stream) {
            Init();
        }

        public CsvReader(string filename) : base(filename) {
            Init();
        }

        private void Init() {
            Properties = new Dictionary<int, PropertyInfo>();

            foreach (var property in typeof(T).GetProperties()) {
                var csvAttribute = property.GetCustomAttribute<CsvAttribute>();

                if (csvAttribute != null) {
                    Properties[csvAttribute.Index] = property;
                }
            }
        }

        public T ReadRow() {
            var item = new T();
            var row = new CsvRow();

            if (ReadRow(row)) {
                var index = 0;

                foreach (var column in row) {
                    if (Properties.ContainsKey(index)) {
                        var property = Properties[index];
                        property.SetValue(item, column.As(property.PropertyType));
                    }

                    index += 1;
                }

                return item;
            }

            return default(T);
        }

        public int CurrentRowIndex { get; set; }

        private Dictionary<int, PropertyInfo> Properties { get; set; }
    }

    public class CsvReader : StreamReader {
        public CsvReader(Stream stream)
            : base(stream) {
        }

        public CsvReader(string filename)
            : base(filename) {
        }

        public class CsvRow : List<string> {
            public string LineText { get; set; }
        }

        public List<CsvRow> GetRows() {
            var results = new List<CsvRow>();

            while (true) {
                var row = new CsvRow();

                if (!ReadRow(row)) {
                    break;
                }

                results.Add(row);
            }

            return results;
        }

        public bool ReadRow(CsvRow row) {
            row.LineText = ReadLine();
            if (string.IsNullOrEmpty(row.LineText))
                return false;

            int pos = 0;
            int rows = 0;

            while (pos < row.LineText.Length) {
                string value;

                if (row.LineText[pos] == '"') {
                    pos++;

                    int start = pos;
                    while (pos < row.LineText.Length) {
                        if (row.LineText[pos] == '"') {
                            pos++;

                            if (pos >= row.LineText.Length || row.LineText[pos] != '"') {
                                pos--;
                                break;
                            }
                        }
                        pos++;
                    }
                    value = row.LineText.Substring(start, pos - start);
                    value = value.Replace("\"\"", "\"");
                }
                else {
                    int start = pos;

                    while (pos < row.LineText.Length && row.LineText[pos] != ',')
                        pos++;

                    value = row.LineText.Substring(start, pos - start);
                }

                if (rows < row.Count)
                    row[rows] = value;
                else
                    row.Add(value);

                rows++;

                while (pos < row.LineText.Length && row.LineText[pos] != ',')
                    pos++;
                if (pos < row.LineText.Length)
                    pos++;
            }

            while (row.Count > rows)
                row.RemoveAt(rows);

            return (row.Count > 0);
        }
    }
}