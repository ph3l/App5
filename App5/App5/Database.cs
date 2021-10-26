using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.Linq;

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

        public async Task<List<string>> GetListAsync()
        {
            DateTime thisDay = DateTime.Today;
            string todaysDate = thisDay.ToString("dd/MM/yyyy");

            List<string> outputList = new List<string>();
            var myOutput = await _database.QueryAsync<TideInformation>("SELECT firstLowTide FROM TideInformation WHERE date = ?", "10/30/2021");
            foreach (var s in myOutput)
            {
                outputList.Add(s.firstLowTide);
            }
            return outputList;
        }


        public bool checkDate(string myDateValue)
        {
            var count = _database.ExecuteScalarAsync<int>("select * from TideInformation where date=?", myDateValue);
            // maybe prompt which value is already there
            if (count.Result > 0)
            {
                // value does exist
                return true;
            }
            else
            {
                // valued does not exist
                return false;
            }
        }
    }
}
