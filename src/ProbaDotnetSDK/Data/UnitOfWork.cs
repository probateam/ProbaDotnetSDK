using LiteDB;
using ProbaDotnetSDK.SharedClasses;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProbaDotnetSDK.Data
{
    internal class UnitOfWork : IDisposable
    {
        private LiteDatabase LiteDatabase { get; }
        private ILogger Logger { get; }
        private string DbPath { get; } = "./sdkdb.db";
        public UnitOfWork(ILogger logger)
        {
            LiteDatabase = new LiteDatabase(DbPath);
            Logger = logger;
            BasicData = LiteDatabase.GetCollection<BasicData>();
            SessionsData = LiteDatabase.GetCollection<SessionData>();
            RemoteConfigurations = LiteDatabase.GetCollection<RemotoConfigurationModel>();
        }
        public void DropDataBase()
        {
            if (File.Exists(DbPath))
                File.Delete(DbPath);
        }
        public ILiteCollection<BasicData> BasicData { get; }
        public ILiteCollection<SessionData> SessionsData { get; }
        public ILiteCollection<RemotoConfigurationModel> RemoteConfigurations { get; }
        public void Dispose()
        {

            LiteDatabase?.Dispose();
        }
    }
}
