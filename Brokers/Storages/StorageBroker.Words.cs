//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Models.Words;

namespace Yodol_telegram_bot_.Brokers.Storages
{
    public partial class StorageBroker
    {
        private const string WordsFile = "words.json";

        public async ValueTask<Word> InsertWordAsync(Word word)
        {
            await this.semaphore.WaitAsync();

            try
            {
                var words = await ReadAllAsync<Word>(WordsFile);
                words.Add(word);
                await WriteAllAsync(WordsFile, words);

                return word;
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        public async ValueTask<List<Word>> SelectAllWordsAsync() =>
            await ReadAllAsync<Word>(WordsFile);

        public async ValueTask<Word?> SelectWordByIdAsync(Guid wordId)
        {
            var words = await ReadAllAsync<Word>(WordsFile);

            return words.FirstOrDefault(word => word.Id == wordId);
        }

        public async ValueTask<List<Word>> SelectWordsByPackageIdAsync(Guid packageId)
        {
            var words = await ReadAllAsync<Word>(WordsFile);

            return words.Where(word => word.PackageId == packageId).ToList();
        }

        public async ValueTask<List<Word>> SelectWordsByUserTelegramIdAsync(
            long userTelegramId)
        {
            var words = await ReadAllAsync<Word>(WordsFile);

            return words.Where(word =>
                word.UserTelegramId == userTelegramId).ToList();
        }

        public async ValueTask<Word> UpdateWordAsync(Word word)
        {
            await this.semaphore.WaitAsync();

            try
            {
                var words = await ReadAllAsync<Word>(WordsFile);
                int index = words.FindIndex(storedWord => storedWord.Id == word.Id);

                if (index >= 0)
                {
                    words[index] = word;
                }

                await WriteAllAsync(WordsFile, words);

                return word;
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        public async ValueTask<Word> DeleteWordAsync(Word word)
        {
            await this.semaphore.WaitAsync();

            try
            {
                var words = await ReadAllAsync<Word>(WordsFile);
                words.RemoveAll(storedWord => storedWord.Id == word.Id);
                await WriteAllAsync(WordsFile, words);

                return word;
            }
            finally
            {
                this.semaphore.Release();
            }
        }
    }
}
