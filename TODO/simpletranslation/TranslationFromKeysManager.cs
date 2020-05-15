using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class TranslationFromKeysManager : Singleton<TranslationFromKeysManager>
{
    private string separator = "\t";
    private TranslationKeyValues[] translatedEntries;

    public string GetStringForID(int stringID)
    {
        if (stringID < translatedEntries.Length)
        {
            return translatedEntries[stringID].GetValueForKey(UserSettings.Instance.GetString("language"));
        }
        else
        {
            HayError("string ID not in array");
            return "_MISSING ID_";
        }
    }

    /************************************************** LIFECYCLE **********************************************************/

    protected override void SingletonAwake()
    {
        LoadNarrationAssets();
    }

    /************************************************** PRIVATE **********************************************************/

    private void LoadNarrationAssets()
    {
        TextAsset csvDataAsset = Resources.Load<TextAsset>("translationsheet/MundaunTranslation"); // No extension needed
        // array starts at 0
        int firstLanguageColumn = 4;

        // a lot of assumptions were made: File has to be ; separated
        HayDebug("length: " + csvDataAsset.text.Length);

        // this gives an array with alternating 0 entries and a line
        // string[] allLines = Regex.Split(csvDataAsset.text, "\n\r|\n|\r");
        string[] splitFile = new string[] { "\r\n", "\r", "\n" };
        string[] filteredLines = csvDataAsset.text.Split(splitFile, System.StringSplitOptions.RemoveEmptyEntries);

        // language tags start at index 4 (en) index 0-3 is key, description, file, name, topic
        string[] csvKeys = Regex.Split(filteredLines[0], separator);
        // filteredLines.Length - 1 because the last line is empty
        translatedEntries = new TranslationKeyValues[filteredLines.Length - 1];

        int translatedEntryNr = 0;
        for (int i = 1; i < filteredLines.Length - 1; i++)
        {
            string[] completeTranslationLine = Regex.Split(filteredLines[i], separator);
            int lineKey = int.Parse(completeTranslationLine[0]);
            HayDebug("adding key " + lineKey);
            translatedEntries[lineKey] = new TranslationKeyValues();

            for (int j = firstLanguageColumn; j < csvKeys.Length; j++)
            {
                // translatedEntries[translatedEntryNr].translationLine.Add(csvKeys[j], completeTranslationLine[j]);
                translatedEntries[lineKey].translationLine.Add(csvKeys[j], completeTranslationLine[j]);
            }

            translatedEntryNr++;
        }

        HayDebug("translation has " + translatedEntries.Length);
    }
}
