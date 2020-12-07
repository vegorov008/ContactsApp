using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ContactsApp.Models;
using SQLite;

namespace ContactsApp.Services
{
    public class SqLiteDataProvider
    {
        readonly string _dbName = "contactsdb.db3";
        readonly string _dbPath;

        readonly object _syncObject = new object();

        SQLiteConnection _db = null;

        const byte _attemptsMax = 100;
        const int _timeout = 100;

        public SqLiteDataProvider()
        {
            try
            {
                var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                _dbPath = folder.Substring(0, folder.LastIndexOf(@"/") + 1) + _dbName;
                //_dbPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + _dbName;
                Initialize();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
        }

        SQLiteConnection OpenConnection()
        {
            return new SQLiteConnection(_dbPath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex);
        }

        public void Initialize()
        {
            try
            {
                lock (_syncObject)
                {
                    _db = OpenConnection();

                    _db.CreateTable<Contact>(CreateFlags.AllImplicit);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
        }

        public async Task AddOrUpdate<TEntity>(TEntity entity) where TEntity : class, IHasIdentity
        {
            try
            {
                byte attempt = 0;
                bool success = false;

                while (!success && attempt < _attemptsMax)
                {
                    try
                    {
                        attempt++;
                        lock (_syncObject)
                        {
                            _db.InsertOrReplace(entity);
                        }
                        success = true;
                    }
                    catch (SQLiteException ex)
                    {
                        //ExceptionHandler.HandleException(ex);
                    }

                    if (!success)
                    {
                        await Task.Delay(_db.BusyTimeout > TimeSpan.MinValue ? _db.BusyTimeout : TimeSpan.FromMilliseconds(_timeout));
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
        }

        public async Task Remove<TEntity>(object id) where TEntity : class
        {
            try
            {
                byte attempt = 0;
                bool success = false;

                while (!success && attempt < _attemptsMax)
                {
                    try
                    {
                        attempt++;
                        lock (_syncObject)
                        {
                            _db.Delete<TEntity>(id);
                        }
                        success = true;
                    }
                    catch (SQLiteException ex)
                    {
                        ExceptionHandler.HandleException(ex);
                    }

                    if (!success)
                    {
                        await Task.Delay(_db.BusyTimeout > TimeSpan.MinValue ? _db.BusyTimeout : TimeSpan.FromMilliseconds(_timeout));
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
        }

        public async Task RemoveAll<TEntity>() where TEntity : class
        {
            try
            {
                byte attempt = 0;
                bool success = false;

                while (!success && attempt < _attemptsMax)
                {
                    try
                    {
                        attempt++;
                        lock (_syncObject)
                        {
                            _db.DeleteAll<TEntity>();
                        }
                        success = true;
                    }
                    catch (SQLiteException ex)
                    {
                        ExceptionHandler.HandleException(ex);
                    }

                    if (!success)
                    {
                        await Task.Delay(_db.BusyTimeout > TimeSpan.MinValue ? _db.BusyTimeout : TimeSpan.FromMilliseconds(_timeout));
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
        }

        public async Task<TEntity> Select<TEntity>(Guid key) where TEntity : class, IHasIdentity, new()
        {
            TEntity result = null;
            try
            {
                byte attempt = 0;
                bool success = false;

                while (!success && attempt < _attemptsMax)
                {
                    try
                    {
                        attempt++;
                        lock (_syncObject)
                        {
                            result = _db.Table<TEntity>().FirstOrDefault(x => x.Id == key);

                            //if (_db.Table<TEntity>().FirstOrDefault(x => x.Id.Equals(key)) != null)
                            //{
                            //    result = _db.GetWithChildren<TEntity>(key);
                            //}
                        }
                        success = true;
                    }
                    catch (SQLiteException ex)
                    {
                        ExceptionHandler.HandleException(ex);
                    }

                    if (!success)
                    {
                        await Task.Delay(_db.BusyTimeout > TimeSpan.MinValue ? _db.BusyTimeout : TimeSpan.FromMilliseconds(_timeout));
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
            return result;
        }

        public async Task<List<TEntity>> Select<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class, new()
        {
            List<TEntity> result = null;
            try
            {
                byte attempt = 0;
                bool success = false;

                while (!success && attempt < _attemptsMax)
                {
                    try
                    {
                        attempt++;
                        lock (_syncObject)
                        {
                            result = _db.Table<TEntity>().ToList();

                            //if (_db.Table<TEntity>().Count() > 0)
                            //{
                            //    result = _db.GetAllWithChildren<TEntity>(filter);
                            //}
                        }
                        success = true;
                    }
                    catch (SQLiteException ex)
                    {
                        ExceptionHandler.HandleException(ex);
                    }

                    if (!success)
                    {
                        await Task.Delay(_db.BusyTimeout > TimeSpan.MinValue ? _db.BusyTimeout : TimeSpan.FromMilliseconds(_timeout));
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
            return result;
        }

        public async Task<List<TEntity>> SelectAll<TEntity>() where TEntity : class, new()
        {
            List<TEntity> result = null;
            try
            {
                result = await Select<TEntity>(null);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
            return result;
        }
    }
}
