using System.Globalization;
using CsvHelper;

namespace simpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    String filepath;
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
        List<T> records = Read().ToList();
        records.Add(record);

        using (StreamWriter writer = new StreamWriter(filepath))
        using (CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csvWriter.WriteRecords(records);
            writer.Flush();
        }
    }
}