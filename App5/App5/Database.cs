using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace App5
{
    public class Database
    {
        readonly SQLiteAsyncConnection _database;

        public Database(string dbPath)
        {
            //Establishing the conection
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<TideInformation>().Wait();
        }

        // Show the registers
        public Task<List<TideInformation>> GetTideAsync()
        {
            return _database.Table<TideInformation>().ToListAsync();
        }

        // Save registers
        public Task<int> SaveTideAsync(TideInformation contact)
        {
            return _database.InsertAsync(contact);
        }

        // Delete registers
        public Task<int> DeleteTideAsync<T>()
        {
            return _database.DeleteAllAsync<TideInformation>();
        }

        // Save registers
        public Task<int> UpdateTideAsync(TideInformation contact)
        {
            return _database.UpdateAsync(contact);
        }
    }
}
