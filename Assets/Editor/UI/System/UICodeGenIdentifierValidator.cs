using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public class UICodeGenIdentifierValidator
{

    private static readonly HashSet<string> LangKeywords = new()
    {
        "abstract","as","base","bool","break","byte","case","catch","char","checked",
        "class","const","continue","decimal","default","delegate","do","double","else",
        "enum","event","explicit","extern","false","finally","fixed","float","for",
        "foreach","goto","if","implicit","in","int","interface","internal","is","lock",
        "long","namespace","new","null","object","operator","out","override","params",
        "private","protected","public","readonly","ref","return","sbyte","sealed",
        "short","sizeof","stackalloc","static","string","struct","switch","this","throw",
        "true","try","typeof","uint","ulong","unchecked","unsafe","ushort","using",
        "virtual","void","volatile","while"
    };

    private StringBuilder _builder = null;

    public UICodeGenIdentifierValidator() => _builder = new();


    private bool IsValidIdentifier(string value)
    {

        if (string.IsNullOrWhiteSpace(value))
            return false;

        if (!(char.IsLetter(value[0]) || value[0] == '_'))
            return false;

        for (int i = 1; i < value.Length; i++)
        {
            var c = value[i];

            if (!(char.IsLetterOrDigit(c) || c == '_'))
                return false;
        }

        return !LangKeywords.Contains(value);
    }

    private string ToIdentifier(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("UI element name is empty.");

        _builder.Clear();
        var isNextUpper = false;

        foreach (var chara in input.Trim())
        {
            if (chara == '-' || chara == ' ' || chara == '.')
            {
                isNextUpper = true;
                continue;
            }

            if (_builder.Length == 0)
            {
                if (char.IsLetter(chara) || chara == '_')
                    _builder.Append(char.ToLowerInvariant(chara));
                else if (char.IsDigit(chara))
                    _builder.Append('_').Append(chara);
                continue;
            }

            if (char.IsLetterOrDigit(chara) || chara == '_')
            {
                _builder.Append(isNextUpper ? char.ToUpperInvariant(chara) : chara);
                isNextUpper = false;
            }
        }

        var result = _builder.ToString();

        if (LangKeywords.Contains(result)) result = $"@{result}";

        if (!IsValidIdentifier(result.TrimStart('@')))
            throw new InvalidOperationException($"Invalid generated C# identifier from UI name '{input}'.");

        return result;
    }

    public void Validate(VisualTreeAsset uxml, IEnumerable<VisualElement> visualElements)
    {
        var seenFields = new Dictionary<string, string>();

        foreach (var elem in visualElements)
        {
            if (string.IsNullOrWhiteSpace(elem.name)) continue;

            var fieldName = $"_{ToIdentifier(elem.name)}";
            var constantName = $"ElementName_{char.ToUpperInvariant(fieldName[1]) + fieldName.Substring(2)}";

            if (seenFields.TryGetValue(fieldName, out var originalName))
            {
                throw new InvalidOperationException(
               $"UXML '{uxml.name}' generates duplicate field '{fieldName}' from names " +
               $"'{originalName}' and '{elem.name}'. Rename one of the elements.");
            }

            seenFields.Add(fieldName, elem.name);

            if (!IsValidIdentifier(constantName))
            {
                throw new InvalidOperationException(
                    $"UXML '{uxml.name}' generates invalid constant '{constantName}' from element '{elem.name}'.");
            }
        }

        var className = uxml.name;

        if (!IsValidIdentifier(className))
        {
            throw new InvalidOperationException(
                $"UXML file name '{uxml.name}' is not a valid C# class name. Rename the UXML file.");
        }

    }
}
