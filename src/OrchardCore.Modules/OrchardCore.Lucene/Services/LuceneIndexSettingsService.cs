using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrchardCore.Environment.Shell;
using OrchardCore.Lucene.Model;

namespace OrchardCore.Lucene
{
    /// <summary>
    /// This class persists the indexing state, a cursor, on the filesystem alongside the index itself.
    /// This state has to be on the filesystem as each node has its own local storage for the index.
    /// </summary>
    public class LuceneIndexSettingsService
    {
        private readonly string _indexSettingsFilename;
        private readonly List<LuceneIndexSettings> _indexSettings;

        public LuceneIndexSettingsService(
            IOptions<ShellOptions> shellOptions,
            ShellSettings shellSettings
            )
        {
            _indexSettingsFilename = PathExtensions.Combine(
                shellOptions.Value.ShellsApplicationDataPath,
                shellOptions.Value.ShellsContainerName,
                shellSettings.Name,
                "lucene.settings.json");

            if (!File.Exists(_indexSettingsFilename))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_indexSettingsFilename));
                File.WriteAllText(_indexSettingsFilename, "");
            }

            _indexSettings = JsonConvert.DeserializeObject<List<LuceneIndexSettings>>(File.ReadAllText(_indexSettingsFilename)) ?? new List<LuceneIndexSettings>();
        }

        public IEnumerable<LuceneIndexSettings> List() => _indexSettings.ToArray();

        public string GetIndexAnalyzer(string indexName)
        {
            var setting = _indexSettings.FirstOrDefault(x => String.Equals(x.IndexName, indexName, StringComparison.OrdinalIgnoreCase));
            if (setting != null)
            {
                return setting.AnalyzerName;
            }

            return null;
        }

        public void EditIndex(LuceneIndexSettings settings)
        {
            lock (this)
            {
                var existing = _indexSettings.FirstOrDefault(x => String.Equals(x.IndexName, settings.IndexName, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                {
                    _indexSettings.Remove(existing);
                }
                _indexSettings.Add(settings);
                Update();
            }
        }

        public void CreateIndex(LuceneIndexSettings settings)
        {
            lock (this)
            {

                _indexSettings.Add(settings);
                Update();
            }
        }

        public void DeleteIndex(LuceneIndexSettings settings)
        {
            lock (this)
            {
                _indexSettings.Remove(settings);
                Update();
            }
        }

        private void Update()
        {
            lock (this)
            {
                File.WriteAllText(_indexSettingsFilename, JsonConvert.SerializeObject(_indexSettings, Formatting.Indented));
            }
        }
    }
}
