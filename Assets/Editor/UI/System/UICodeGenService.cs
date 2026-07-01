using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

public class UICodeGenService
{
    public string GetGuid(ScriptableObject asset)
    {
        string path = AssetDatabase.GetAssetPath(asset);
        return AssetDatabase.AssetPathToGUID(path);
    }

    //private List<VisualElement> ExtractVisualElements(VisualTreeAsset uxml)
    //    => GetVisualElements(uxml.Instantiate());

    private static List<VisualElement> ExtractVisualElements(VisualElement root)
    {
        var result = new List<VisualElement>();
        var stack = new Stack<VisualElement>();

        foreach (VisualElement child in root.Children())
            stack.Push(child);

        while (stack.Count > 0)
        {
            VisualElement current = stack.Pop();
            result.Add(current);

            foreach (VisualElement child in current.Children())
                stack.Push(child);
        }

        return result;
    }

    private string SanitizeVariableName(string input)
    {
        StringBuilder result = new StringBuilder();
        bool upperNext = false;

        foreach (char c in input)
        {
            if (c == '-')
            {
                upperNext = true;
                continue;
            }

            if (upperNext)
            {
                result.Append(char.ToUpper(c));
                upperNext = false;
            }
            else
            {
                result.Append(c);
            }
        }

        return result.ToString();

    }

    private void GenerateInClassSpace(StringBuilder builder )
    {
        builder.AppendLine().AppendLine().AppendLine();
    }

    private void GenerateEditorHeader(StringBuilder builder, string name)
    {
        builder.AppendLine("using UnityEngine;")
               .AppendLine("using UnityEngine.UIElements;")
               .AppendLine("using UnityEditor;")
               .AppendLine()
               .AppendLine($"public partial class {name} : EditorWindow")
               .AppendLine("{");
    }

    private void GenerateEditorConstants(StringBuilder builder, VisualTreeAsset uxml, StyleSheet[] ussStyleSheets, IEnumerable<VisualElement> visualElements)
    {
        var pattern = @"Window|Editor";
        var uxmlName = Regex.Replace(uxml.name, pattern, "", RegexOptions.IgnoreCase);
        uxmlName = Regex.Replace(uxmlName, "(?<!^)([A-Z])", " $1");

        builder.AppendLine("\t/*************************************************************************/")
               .AppendLine("\t#region Constants")
               .AppendLine("\t/*************************************************************************/")
               .AppendLine($"\tprivate const string UxmlGUID = \"{GetGuid(uxml)}\";")
               .AppendLine($"\tprivate const string UxmlName = \"{uxmlName}\";");
        //foreach (var uss in ussStyleSheets)
        //{
        //    builder.AppendLine($"    private const string {char.ToUpper(uss.name[0])}{uss.name.Substring(1)}UssGUID = \"{GetGuid(uss)}\";");
        //}

        foreach (var elem in visualElements)
        {
            if (string.IsNullOrWhiteSpace(elem.name)) continue;
            var sanitizedName = SanitizeVariableName(elem.name);
            builder.AppendLine($"\tprivate const string {char.ToUpper(sanitizedName[0])}{sanitizedName.Substring(1)}Text = \"{elem.name}\";");
        }
        builder.AppendLine("\t#endregion");
        builder.AppendLine("\t/*************************************************************************/");
    }

    private void GenerateEditorFields( StringBuilder builder, IEnumerable<VisualElement> visualElements)
    {
        builder.AppendLine("\t/*************************************************************************/")
               .AppendLine("\t#region Fields")
               .AppendLine("\t/*************************************************************************/");

        foreach (var elem in visualElements)
        {
            if (string.IsNullOrWhiteSpace(elem.name)) continue;

            builder.Append("\tprivate ")
                   .Append(elem.GetType().Name)
                   .Append(" _")
                   .Append(SanitizeVariableName(elem.name))
                   .AppendLine(" = null;");
        }

        builder.AppendLine()
               .AppendLine("\t[SerializeField]")
               .AppendLine("\tprivate VisualTreeAsset _uxml = null;");

        builder.AppendLine("\t#endregion")
               .AppendLine("\t/*************************************************************************/");
    }

    private void GenerateLoadAssetsFunction(StringBuilder builder)
    {
        builder.AppendLine("\tprivate void LoadAssets()")
               .AppendLine("\t{")
               .AppendLine("\t\tvar path = string.Empty;")
               .AppendLine("\t\tif (_uxml == null)")
               .AppendLine("\t\t{")
               .AppendLine("\t\t\tpath = AssetDatabase.GUIDToAssetPath(UxmlGUID);")
               .AppendLine("\t\t\t_uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);")
               .AppendLine("\t\t}")
               .AppendLine("\t}");

    }

    private void GenerateInitializeComponentsFunction(StringBuilder builder, IEnumerable<VisualElement> visualElements)
    {
        builder.AppendLine("\tprotected void InitializeComponents()")
               .AppendLine("\t{")
               .AppendLine("\t\tLoadAssets();")
               .AppendLine()
               .AppendLine("\t\t_uxml.CloneTree(rootVisualElement);")
               .AppendLine();

        foreach (var elem in visualElements)
        {
            if (string.IsNullOrWhiteSpace(elem.name)) continue;

            var sanitizedName = SanitizeVariableName(elem.name);
            builder.AppendLine($"\t\t_{SanitizeVariableName(elem.name)} = rootVisualElement.Q<{elem.GetType().Name}>({char.ToUpper(sanitizedName[0])}{sanitizedName.Substring(1)}Text) as {elem.GetType().Name};");
        }


        builder.AppendLine("}");
    }

    private void GenerateShowWindowFunction(StringBuilder builder, string name)
    {
        builder.AppendLine($"\t[MenuItem(\"Tools/{name}\")]")
               .AppendLine("\tpublic static void OpenWindow()")
               .AppendLine("\t{")
               .AppendLine($"\t\tvar wnd = GetWindow<{name}>();")
               .AppendLine("\t\twnd.titleContent = new(UxmlName);")
               .AppendLine("\t\twnd.Show();")
               .AppendLine("\t}");
    }
    
    public void GenerateCreateGUIFunction(StringBuilder builder)
    {
        builder.AppendLine("\tprivate void CreateGUI()")
               .AppendLine("\t{")
               .AppendLine("\t\tInitializeComponents();")
               .AppendLine("\t}");
    }

    private void WriteFile(StringBuilder builder, string fileName, string relativePath, string suffix)
    {
        if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(relativePath)) return;

        var fullName = $"{fileName}.{suffix}";
        var dirPath = $"{Application.dataPath}/{relativePath}/";
        var filePath = $"{dirPath}{fullName}";

        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

        File.WriteAllText(filePath, builder.ToString(), Encoding.UTF8);
    }

    public void GenerateEditorFile(VisualTreeAsset uxml)
    {

        //if (!uxmlField.value || uxmlField.GetType() != typeof(VisualTreeAsset)) return;

        //var uxml = uxmlField.value as VisualTreeAsset;
        var ussStyleSheets = uxml.stylesheets.ToArray();

        var visualElements = ExtractVisualElements(uxml.Instantiate());

        var builder = new StringBuilder();
        GenerateEditorHeader(builder, uxml.name);
        GenerateEditorConstants(builder, uxml, ussStyleSheets, visualElements);
        GenerateInClassSpace(builder);
        GenerateEditorFields(builder, visualElements);
        GenerateInClassSpace(builder);
        GenerateLoadAssetsFunction(builder);
        builder.AppendLine();
        GenerateShowWindowFunction(builder, uxml.name);
        builder.AppendLine();
        GenerateInitializeComponentsFunction(builder, visualElements);

        builder.AppendLine("}");

        WriteFile(builder, uxml.name, "Editor/UI/generated", "generated.cs");


        builder.Clear();


        GenerateEditorHeader(builder, uxml.name);
        GenerateCreateGUIFunction(builder);
        builder.AppendLine("}");

        if(!File.Exists($"{Application.dataPath}/Editor/UI/{uxml.name}.cs")) WriteFile(builder, uxml.name, "Editor/UI", ".cs");
    }
}
