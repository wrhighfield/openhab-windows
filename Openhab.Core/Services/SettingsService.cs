﻿using Newtonsoft.Json;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Model;
using Windows.Storage;

namespace OpenHAB.Core.Services
{
    /// <summary>
    /// Service that handles all settings operations.
    /// </summary>
    public class SettingsService : ISettingsService
    {
        private ApplicationDataContainer _settingsContainer;

        /// <inheritdoc />
        public OpenHABVersion ServerVersion { get; set; }

        /// <inheritdoc />
        public void Save(Settings settings)
        {
            EnsureSettingsContainer();
            _settingsContainer.Values[Constants.Local.SettingsKey] = JsonConvert.SerializeObject(settings);
        }

        /// <inheritdoc />
        public void SaveCurrentSitemap(string name)
        {
            _settingsContainer.Values[Constants.Local.SitemapKey] = name;
        }

        /// <inheritdoc />
        public string LoadLastSitemap()
        {
            if (!_settingsContainer.Values.TryGetValue(Constants.Local.SitemapKey, out object sitemapKey))
            {
                return null;
            }

            return sitemapKey.ToString();
        }

        /// <inheritdoc />
        public Settings Load()
        {
            EnsureSettingsContainer();

            if (!_settingsContainer.Values.ContainsKey(Constants.Local.SettingsKey))
            {
                return new Settings();
            }

            string json = _settingsContainer.Values[Constants.Local.SettingsKey].ToString();

            if (json == null)
            {
                return new Settings();
            }

            return JsonConvert.DeserializeObject<Settings>(json);
        }

        private void EnsureSettingsContainer()
        {
            if (_settingsContainer == null)
            {
                _settingsContainer = ApplicationData.Current.RoamingSettings;
            }
        }
    }
}
