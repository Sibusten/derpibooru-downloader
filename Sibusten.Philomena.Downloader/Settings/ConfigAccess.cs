using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;

namespace Sibusten.Philomena.Downloader.Settings
{
    public class ConfigAccess : IDisposable
    {
        private const string configFilename = "PhilomenaDownloader.db";
        private const string booruCollectionName = "boorus";
        private const string presetCollectionName = "presets";

        ILiteDatabase _db;
        private ILiteCollection<BooruConfig> _booruCollection;
        private ILiteCollection<SearchPreset> _presetCollection;

        public ConfigAccess()
        {
            string configPath = Path.Join(Directory.GetCurrentDirectory(), configFilename);
            _db = new LiteDatabase(configPath);

            _booruCollection = _db.GetCollection<BooruConfig>(booruCollectionName);
            _presetCollection = _db.GetCollection<SearchPreset>(presetCollectionName);
        }

        public List<BooruConfig> GetBoorus()
        {
            return _booruCollection.FindAll().ToList();
        }

        public BooruConfig? GetBooru(string name)
        {
            return _booruCollection.FindOne(p => p.Name == name);
        }

        public void UpsertBooru(BooruConfig booru)
        {
            _booruCollection.Upsert(booru);
        }

        public void DeleteBooru(ObjectId booruId)
        {
            _booruCollection.Delete(booruId);
        }

        public List<SearchPreset> GetPresets()
        {
            return _presetCollection.FindAll().ToList();
        }

        public SearchPreset? GetPreset(string name)
        {
            return _presetCollection.FindOne(p => p.Name == name);
        }

        public void UpsertPreset(SearchPreset preset)
        {
            _presetCollection.Upsert(preset);
        }

        public void DeletePreset(ObjectId presetId)
        {
            _presetCollection.Delete(presetId);
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
