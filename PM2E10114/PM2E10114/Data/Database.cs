using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using PM2E10114.Models;
using System.Threading.Tasks;

namespace PM2E10114.Data
{
    public class Database
    {
        SQLiteAsyncConnection db;

        public Database (string dbPath)
        {
            db = new SQLiteAsyncConnection(dbPath);
            db.CreateTableAsync<Sitios>().Wait();
        }
        public Task<int> Agreagar(Sitios sitios)
        {
            if (sitios.ID == 0)
            {
                return db.InsertAsync(sitios);
            }
            else
            {
                return null;
            }
        }
        public Task<List<Sitios>> GetSitiosAsync()
        {
            return db.Table<Sitios>().ToListAsync();
        }
        public Task<Sitios> GetSitiosByIdAsync(int ID)
        {
            return db.Table<Sitios>().Where(a => a.ID == ID).FirstOrDefaultAsync();
        }

        public Task<int> DeleteSitioAsync(int sitioID)
        {
            return db.DeleteAsync<Sitios>(sitioID);
        }


    }
}
