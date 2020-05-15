using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class TranslationKeyValues
{
    // holds language tag and the text to it
    public IDictionary<string, string> translationLine;
    
    private string newLinePattern = "<newline>";

    public TranslationKeyValues()
    {
        translationLine = new Dictionary<string, string>();
    }

    public string GetValueForKey(string key)
    {
        string answer;
        string original;
        translationLine.TryGetValue("en", out original);
        translationLine.TryGetValue(key, out answer);

        if (original.Length > 0 && answer.Length == 0)
        {
            answer = "_MISSING TRANSLATION_";
        }

        answer = Regex.Replace(answer, newLinePattern, Environment.NewLine);
        return answer;
    }
}
