using LiteDB;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProbaDotnetSDK.Data
{
    internal class UnitOfWork : IDisposable
    {
        private LiteDatabase LiteDatabase { get; }
        private ILogger Logger { get; }

        public UnitOfWork(ILogger logger)
        {
            LiteDatabase = new LiteDatabase("./db/sdkdb.db");
            Logger = logger;
            BasicData = LiteDatabase.GetCollection<BasicData>();
            SessionsData = LiteDatabase.GetCollection<SessionData>();
        }

        public ILiteCollection<BasicData> BasicData { get; }
        private ILiteCollection<SessionData> SessionsData { get; }
        public void Dispose()
        {

            LiteDatabase?.Dispose();
        }
    }
}
