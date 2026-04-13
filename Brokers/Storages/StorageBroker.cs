//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using System.Text.Json;

namespace Yodol_telegram_bot_.Brokers.Storages
{
    public partial class StorageBroker : IStorageBroker
    {
        private readonly string storagePath;
        private readonly SemaphoreSlim semaphore;
        private readonly JsonSerializerOptions jsonOptions;

        public StorageBroker()
        {
            this.storagePath = "Storage";
            this.semaphore = new SemaphoreSlim(1, 1);

            this.jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            EnsureStorageDirectoryExists();
        }

        private void EnsureStorageDirectoryExists()
        {
            if (!Directory.Exists(this.storagePath))
            {
                Directory.CreateDirectory(this.storagePath);
            }
        }

        private string GetFilePath(string fileName) =>
            Path.Combine(this.storagePath, fileName);

        private async ValueTask<List<T>> ReadAllAsync<T>(string fileName)
        {
            string filePath = GetFilePath(fileName);

            if (!File.Exists(filePath))
            {
                return new List<T>();
            }

            string json = await File.ReadAllTextAsync(filePath);

            return JsonSerializer.Deserialize<List<T>>(json, this.jsonOptions)
                ?? new List<T>();
        }

        private async ValueTask WriteAllAsync<T>(string fileName, List<T> items)
        {
            string filePath = GetFilePath(fileName);

            string json = JsonSerializer.Serialize(items, this.jsonOptions);

            await File.WriteAllTextAsync(filePath, json);
        }
    }
}
