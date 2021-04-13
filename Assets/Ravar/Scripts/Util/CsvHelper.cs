// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Itsdits.Ravar.Util
{
    /// <summary>
    /// Helper class that loads and parses CSV files.
    /// </summary>
    /// <remarks>Used for localization.</remarks>
    public class CsvHelper
    {
        private TextAsset _localizationFile;
        private const string Filepath = "Localization/Locale_";
        private const char SurroundingChar = '"';
        private readonly string[] LineSeparator = {Environment.NewLine};

        /// <summary>
        /// Loads a localized text asset into the CsvHelper to be used in the localized dictionary.
        /// </summary>
        /// <param name="languageCode">Two letter code used to represent the language to load.</param>
        public void LoadLocalization(string languageCode)
        {
            _localizationFile = Resources.Load<TextAsset>(Filepath + languageCode.ToUpper());
        }
        
        /// <summary>
        /// Builds the localization dictionary from the text asset currently loaded in the CsvHelper.
        /// </summary>
        /// <returns>A new dictionary with the localized strings of the current language.</returns>
        public Dictionary<string, string> BuildDictionary()
        {
            var dictionary = new Dictionary<string, string>();
            
            // Handle Windows \r\n malarkey.
            string[] lines = _localizationFile.text.Split(LineSeparator, StringSplitOptions.RemoveEmptyEntries);
            
            // Regex to split the fields without cutting off dialog strings that use punctuation marks.
            var csvParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            for (var i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] fields = csvParser.Split(line);
                for (var j = 0; j < fields.Length; j++)
                {
                    fields[j] = fields[j].TrimStart(SurroundingChar);
                    fields[j] = fields[j].TrimEnd(SurroundingChar);
                }

                string key = fields[0];
                string value = fields[1];
                if (dictionary.ContainsKey(key))
                {
                    continue;
                }
                
                dictionary.Add(key, value);
            }

            return dictionary;
        }
    }
}