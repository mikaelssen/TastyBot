﻿using Interfaces.Contracts.Database;
using System;
using LiteDB;
using System.IO;

namespace Databases
{
    public class LiteDB : ILiteDB
    {
        private readonly string connectionString = $"Filename={AppContext.BaseDirectory}Databases/LiteDB.db;Password=QDgDAKIAEReA9EgYG102TJ1eQO0";

        public LiteDatabase GetDatabase()
        {
            Directory.CreateDirectory(AppContext.BaseDirectory + "Databases");
            return new LiteDatabase(connectionString);
        }

        public ILiteCollection<T> GetColumnByName<T>(string name)
        {
            var liteDB = GetDatabase();
            return liteDB.GetCollection<T>(name);
        }
    }
}