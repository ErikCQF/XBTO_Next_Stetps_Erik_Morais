using System.Text.Json;

namespace XbtoMarketData.DataRepository.Price
{
    public class FilePriceRepo : IPriceRepo
    {
        private readonly string _filePath;

        public FilePriceRepo(string filePath)
        {
            _filePath = filePath;
        }

        public async Task<PriceDb> AddUpdate(PriceDb instrument)
        {
            // Read existing data from file
            PriceDb[] existingData = await ReadFromFile();

            // Check if the instrument already exists in the data
            PriceDb existingInstrument = existingData.FirstOrDefault(i => i.InstrumentName == instrument.InstrumentName);

            if (existingInstrument != null)
            {
                // Update existing instrument
                existingInstrument.Price = instrument.Price;
                existingInstrument.MarkPrice = instrument.MarkPrice;
            }
            else
            {
                // Add new instrument to data
                var newData = existingData.ToList();
                newData.Add(instrument);
                existingData = newData.ToArray();
            }

            // Write updated data back to file
            await WriteToFile(existingData);

            return instrument;
        }

        private async Task<PriceDb[]> ReadFromFile()
        {
            if (File.Exists(_filePath))
            {
                using (FileStream fs = File.OpenRead(_filePath))
                {
                    return await JsonSerializer.DeserializeAsync<PriceDb[]>(fs);
                }
            }
            else
            {
                return new PriceDb[0];
            }
        }

        private async Task WriteToFile(PriceDb[] data)
        {
            using (FileStream fs = File.Create(_filePath))
            {
                await JsonSerializer.SerializeAsync(fs, data);
            }
        }
    }
}