using System.Collections.Generic;
using Itsdits.Ravar.Util;

namespace Itsdits.Ravar.UI
{
    /// <summary>
    /// Localization class that handles language dictionaries and values.
    /// </summary>
    public static class Localization
    {
        private static Language _language = Language.English;
        private static Dictionary<string, string> _localizedDictionary;
        private static bool _isInit;

        private static void Init()
        {
            var csvHelper = new CsvHelper();
            csvHelper.LoadLocalization("EN");
            _localizedDictionary = csvHelper.BuildDictionary();
            _isInit = true;
        }

        /// <summary>
        /// Gets a localized string value from the respective localization dictionary.
        /// </summary>
        /// <param name="key">Key of the string to get.</param>
        /// <returns>Localized value matching the key passed in parameter.</returns>
        public static string GetLocalizedValue(string key)
        {
            if (!_isInit)
            {
                Init();
            }

            _localizedDictionary.TryGetValue(key, out string value);
            return value;
        }

        /// <summary>
        /// Changes the current localization language for the game.
        /// </summary>
        /// <param name="language">Language to change the localization to.</param>
        public static void ChangeLanguage(Language language)
        {
            _language = language;
        }
    }
}
