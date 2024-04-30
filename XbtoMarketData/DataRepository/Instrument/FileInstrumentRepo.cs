using System.Text.Json;

namespace XbtoMarketData.DataRepository.Instrument
{
    public class FileInstrumentRepo : IInstrumentRepo
    {
        private readonly string _filePath;

        public FileInstrumentRepo(string filePath)
        {
            _filePath = filePath;         
        }

        public async Task<InstrumentDb> AddUpdate(InstrumentDb instrument)
        {
            // Read existing data from file
            List<InstrumentDb>? existingData = await ReadFromFile();

            if (existingData == null)
            {
                existingData = new List<InstrumentDb>();
            }

            // Check if the instrument already exists in the data
            InstrumentDb? existingInstrument = existingData?.Find(i => i.InstrumentName == instrument.InstrumentName);

            if (existingInstrument != null)
            {
                // Update existing instrument
                existingInstrument.BaseCurrency = instrument.BaseCurrency;
                existingInstrument.Kind = instrument.Kind;
            }
            else
            {
                // Add new instrument to data
                existingData.Add(instrument);
            }

            // Write updated data back to file
            await WriteToFile(existingData);

            return instrument;
        }

        public async Task<List<InstrumentDb>?> GetToMonitor()
        {
            // Read existing data from file
            List<InstrumentDb>? existingData = await ReadFromFile();

            // Return the list of instruments to monitor
            return existingData;
        }

        private async Task<List<InstrumentDb>?> ReadFromFile()
        {
            if (File.Exists(_filePath))
            {
                using (FileStream fs = File.OpenRead(_filePath))
                {
                    StreamReader reader = new StreamReader(fs);
                    string fileContent = await reader.ReadToEndAsync();

                    if(string.IsNullOrEmpty(fileContent))
                    {
                        return null;
                    }
                    // Reset position of file stream to the beginning
                    fs.Seek(0, SeekOrigin.Begin);

                    // Deserialize data from file
                    var data = await JsonSerializer.DeserializeAsync<List<InstrumentDb>?>(fs);
                    return data ?? new List<InstrumentDb>();
                }
            }
            else
            {
                return new List<InstrumentDb>();
            }
        }


        private async Task WriteToFile(List<InstrumentDb> data)
        {
            using (FileStream fs = File.Create(_filePath))
            {
                // Serialize data to file
                await JsonSerializer.SerializeAsync(fs, data);
            }
        }
    }
}
