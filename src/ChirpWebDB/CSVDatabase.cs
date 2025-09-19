using System.Globalization;
using CsvHelper;

namespace simpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    string filepath;
    public CSVDatabase(string filepath)
    {
        this.filepath = filepath;
    }

    public IEnumerable<T> Read(int? limit = null)
    {
        using (StreamReader reader = new StreamReader(filepath))
        using (CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var result = csvReader.GetRecords<T>().ToList();
            if (limit == null) {  return result; }
            if(limit > result.Count) { limit = result.Count;  }
            result.RemoveRange(0, result.Count - (int)limit);
            return result;
        }
    }

    public void Store(T record)
    {
        using (var stream = new StreamWriter(filepath, append: true))
        using (var csvWriter = new CsvWriter(stream, CultureInfo.InvariantCulture))
        {
            if (!File.Exists(filepath))
            {
                csvWriter.WriteHeader<T>();
                csvWriter.NextRecord();
            }

            csvWriter.WriteRecord(record);
            csvWriter.NextRecord();
            stream.Flush();
        }
    }
}