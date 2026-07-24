using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;


public class UICodeGenService
{

    private StringBuilder _builder = null;
    private UICodeGenIdentifierValidator _validator = null;

    public UICodeGenService()
    {
        _builder = new();
        _validator = new();
    }

    public string GetGuid(ScriptableObject asset)
    {
        string path = AssetDatabase.GetAssetPath(asset);
        return AssetDatabase.AssetPathToGUID(path);
    }

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

    private List<UxmlEventBinding> ExtractClickBindings(IEnumerable<VisualElement> visualElements)
    {
        var result = new List<UxmlEventBinding>();

        foreach (var elem in visualElements)
        {
            if (string.IsNullOrWhiteSpace(elem.name)) continue;

            if(elem is IUICodeGenClickable clickable && !string.IsNullOrWhiteSpace(clickable.CodeGenClick))
            {
                result.Add(new UxmlEventBinding { ElementName = elem.name, HandlerName = clickable.CodeGenClick });
            }
        }

        return result;
    }

    private List<UxmlDataBinding> ExtractDataBindings(IEnumerable<VisualElement> visualElements)
    {
        var result = new List<UxmlDataBinding>();

        foreach (var elem in visualElements)
        {
            if (string.IsNullOrWhiteSpace(elem.name)) continue;

            if(     elem is IUICodeGenBindable bindable 
                && !string.IsNullOrWhiteSpace(bindable.BindName)
                && bindable.BindMode != UICodeGenBindMode.None)
            {
                result.Add(new UxmlDataBinding
                {
                    ElementName = elem.name,
                    ElementType = elem.GetType(),
                    SourcePath = bindable.BindName,
                    TargetProperty = bindable.BindingTargetProperty,
                    BindMode = bindable.BindMode,
                    BindKind = UICodeGenBindKind.Value
                });
            }

            if (    elem is IUICodeGenItemsSource itemsSource
                 && !string.IsNullOrWhiteSpace(itemsSource.ItemsSourceBinding))
            {
                result.Add(new UxmlDataBinding
                {
                    ElementName = elem.name,
                    ElementType = elem.GetType(),
                    SourcePath = itemsSource.ItemsSourceBinding,
                    TargetProperty = nameof(itemsSource),
                    BindMode = UICodeGenBindMode.OneWay,
                    BindKind = UICodeGenBindKind.ItemsSource
                });
            }

            if (elem is IUICodeGenSelectionBindable selection)
            {
                if (!string.IsNullOrWhiteSpace(selection.SelectedItemBinding))
                {
                    result.Add(new UxmlDataBinding
                    {
                        ElementName = elem.name,
                        ElementType = elem.GetType(),
                        SourcePath = selection.SelectedItemBinding,
                        TargetProperty = "selectedItem",
                        BindMode = UICodeGenBindMode.TwoWay,
                        BindKind = UICodeGenBindKind.SelectedItem
                    });
                }

                if (!string.IsNullOrWhiteSpace(selection.SelectedIndexBinding))
                {
                    result.Add(new UxmlDataBinding
                    {
                        ElementName = elem.name,
                        ElementType = elem.GetType(),
                        SourcePath = selection.SelectedIndexBinding,
                        TargetProperty = "selectedIndex",
                        BindMode = UICodeGenBindMode.TwoWay,
                        BindKind = UICodeGenBindKind.SelectedIndex
                    });
                }
            }
        }

        return result;
    }

    private string SanitizeVariableName(string input)
    {
        var result = new StringBuilder();
        var upperNext = false;

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

    private void GenerateInClassSpace()
    {
        _builder.AppendLine().AppendLine().AppendLine();
    }

    private void GenerateEditorGeneratedHeader(string name)
    {
        _builder.AppendLine("/****************************************************************************/")
                .AppendLine("// <auto-generated>")
                .AppendLine("// This file is autogenerated by UICodeGenService")
                .AppendLine($"// Unity Version:        {Application.unityVersion}")
                .AppendLine($"// .Net Runtime Version: {Environment.Version.ToString()}")
                .AppendLine($"// Framework Version:    {RuntimeInformation.FrameworkDescription}")
                .AppendLine("// ")
                .AppendLine("// Do not edit it manually")
                .AppendLine("// </auto-generated>")
                .AppendLine("/****************************************************************************/")
                .AppendLine("using System;")
                .AppendLine("using UnityEngine;")
                .AppendLine("using UnityEngine.UIElements;")
                .AppendLine("using UnityEditor;")
                .AppendLine("using System.Collections.Generic;")
                .AppendLine()
                .AppendLine($"public partial class {name} : EditorWindow")
                .AppendLine("{");
    }


    private void GenerateEditorHeader(string name)
    {
        _builder.AppendLine("using System;")
                .AppendLine("using UnityEngine;")
                .AppendLine("using UnityEngine.UIElements;")
                .AppendLine("using UnityEditor;")
                .AppendLine()
                .AppendLine($"public partial class {name} : EditorWindow")
                .AppendLine("{");
    }


    private void GenerateGameplayGeneratedHeader(string name)
    {
        _builder.AppendLine("/****************************************************************************/")
                .AppendLine("// <auto-generated>")
                .AppendLine("// This file is autogenerated by UICodeGenService")
                .AppendLine($"// Unity Version:        {Application.unityVersion}")
                .AppendLine($"// .Net Runtime Version: {Environment.Version.ToString()}")
                .AppendLine($"// Framework Version:    {RuntimeInformation.FrameworkDescription}")
                .AppendLine("// ")
                .AppendLine("// Do not edit it manually")
                .AppendLine("// </auto-generated>")
                .AppendLine("/****************************************************************************/")
                .AppendLine("using System;")
                .AppendLine("using UnityEngine;")
                .AppendLine("using UnityEngine.UIElements;")
                .AppendLine("using System.Collections.Generic;")
                .AppendLine()
                .AppendLine("[RequireComponent(typeof(UIDocument))]")
                .AppendLine($"public partial class {name} : MonoBehaviour")
                .AppendLine("{");
    }


    private void GenerateGameplayHeader(string name)
    {
        _builder.AppendLine("using System;")
                .AppendLine("using UnityEngine;")
                .AppendLine("using UnityEngine.UIElements;")
                .AppendLine()
                .AppendLine("[RequireComponent(typeof(UIDocument))]")
                .AppendLine($"public partial class {name} : MonoBehaviour")
                .AppendLine("{");
    }

    private void GenerateEditorConstants(VisualTreeAsset uxml, StyleSheet[] ussStyleSheets, IEnumerable<VisualElement> visualElements)
    {
        var pattern = @"Window|Editor";
        var uxmlName = Regex.Replace(uxml.name, pattern, "", RegexOptions.IgnoreCase);
        uxmlName = Regex.Replace(uxmlName, "(?<!^)([A-Z])", " $1");

        _builder.AppendLine("\t/*************************************************************************/")
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
            _builder.AppendLine($"\tprivate const string ElementName_{char.ToUpper(sanitizedName[0])}{sanitizedName.Substring(1)} = \"{elem.name}\";");
        }
        _builder.AppendLine("\t#endregion");
        _builder.AppendLine("\t/*************************************************************************/");
    }

    private void GenerateGameplayConstants(IEnumerable<VisualElement> visualElements)
    {
        _builder.AppendLine("\t/*************************************************************************/")
                .AppendLine("\t#region Constants")
                .AppendLine("\t/*************************************************************************/");

        foreach (var elem in visualElements)
        {
            if (string.IsNullOrWhiteSpace(elem.name)) continue;
            var sanitizedName = SanitizeVariableName(elem.name);
            _builder.AppendLine($"\tprivate const string ElementName_{char.ToUpper(sanitizedName[0])}{sanitizedName.Substring(1)} = \"{elem.name}\";");
        }
        _builder.AppendLine("\t#endregion");
        _builder.AppendLine("\t/*************************************************************************/");
    }

    private void GenerateEditorFields( IEnumerable<VisualElement> visualElements)
    {
        _builder.AppendLine("\t/*************************************************************************/")
                .AppendLine("\t#region Fields")
                .AppendLine("\t/*************************************************************************/");

        foreach (var elem in visualElements)
        {
            if (string.IsNullOrWhiteSpace(elem.name)) continue;

            _builder.Append("\tprivate ")
                    .Append(elem.GetType().Name)
                    .Append(" _")
                    .Append(SanitizeVariableName(elem.name))
                    .AppendLine(" = null;");
        }

        _builder.AppendLine()
                .AppendLine("\tprivate object _dataContext = null;")
                .AppendLine("\tprivate readonly List<IDisposable> _dataBindings = new();")
                .AppendLine()
                .AppendLine("\t[SerializeField]")
                .AppendLine("\tprivate VisualTreeAsset _uxml = null;");

        _builder.AppendLine("\t#endregion")
                .AppendLine("\t/*************************************************************************/");
    }

    private void GenerateGameplayFields(IEnumerable<VisualElement> visualElements)
    {
        _builder.AppendLine("\t/*************************************************************************/")
                .AppendLine("\t#region Fields")
                .AppendLine("\t/*************************************************************************/");

        foreach (var elem in visualElements)
        {
            if (string.IsNullOrWhiteSpace(elem.name)) continue;

            _builder.Append("\tprivate ")
                    .Append(elem.GetType().Name)
                    .Append(" _")
                    .Append(SanitizeVariableName(elem.name))
                    .AppendLine(" = null;");
        }

        _builder.AppendLine()
                .AppendLine("\tprivate object _dataContext = null;")
                .AppendLine("\tprivate readonly List<IDisposable> _dataBindings = new();")
                .AppendLine()
                .AppendLine("\t[SerializeField]")
                .AppendLine("\tprivate UIAssetConfig _config = null;")
                .AppendLine()
                .AppendLine("\t[SerializeField]")
                .AppendLine("\tprivate PanelSettings _panelSettings = null;")
                .AppendLine()
                .AppendLine("\t[SerializeField]")
                .AppendLine("\tprivate VisualTreeAsset _uxml = null;")
                .AppendLine()
                .AppendLine("\t[SerializeField]")
                .AppendLine("\tprivate VisualElement _rootVisualElement = null;")
                .AppendLine()
                .AppendLine("\t[SerializeField]")
                .AppendLine("\tprivate UIDocument _uiDoc = null;");

        _builder.AppendLine("\t#endregion")
               .AppendLine("\t/*************************************************************************/");
    }

    private void GenerateProperties()
    {
        _builder.AppendLine("\t/*************************************************************************/")
                .AppendLine("\t#region Properties")
                .AppendLine("\t/*************************************************************************/")
                .AppendLine()
                .AppendLine("\tpublic object DataContext")
                .AppendLine("\t{")
                .AppendLine("\t\tget => _dataContext;")
                .AppendLine("\t\tset")
                .AppendLine("\t\t{")
                .AppendLine("\t\tif (ReferenceEquals(_dataContext, value)) return;")
                .AppendLine()
                .AppendLine("\t\tUnbindDataContext();")
                .AppendLine()
                .AppendLine("\t\t_dataContext = value;")
                .AppendLine()
                .AppendLine("\t\tBindDataContext();")
                .AppendLine("\t\t}")
                .AppendLine("\t}")
                .AppendLine()
                .AppendLine("\t#endregion")
                .AppendLine("\t/*************************************************************************/");
    }

    private void GenerateGeneratedClickableCallbacks(IEnumerable<UxmlEventBinding> bindings)
    {
        foreach (var binding in bindings)
        {
            _builder.AppendLine($"\tpartial void {binding.HandlerName}(ClickEvent e);");
        }
    }



    private void GenerateClickableCallbacks(IEnumerable<UxmlEventBinding> bindings)
    {
        foreach (var binding in bindings)
        {
            _builder.AppendLine($"\tpartial void {binding.HandlerName}(ClickEvent e)")
                    .AppendLine("\t{")
                    .AppendLine("\t}")
                    .AppendLine();

        }
    }

    private void GenerateBindingContext()
    {
        _builder.AppendLine("\tprivate void BindDataContext()")
                .AppendLine("\t{")
                .AppendLine("\t\tif (_dataContext == null) return;")
                .AppendLine("\t\tInitializeGeneratedBindings();")
                .AppendLine("\t}")
                .AppendLine()
                .AppendLine("\tprivate void UnbindDataContext()")
                .AppendLine("\t{")
                .AppendLine("\t\tforeach (var binding in _dataBindings) binding.Dispose();")
                .AppendLine("\t\t_dataBindings.Clear();")
                .AppendLine("\t}");
    }

    private void GenerateLoadAssetsEditorFunction()
    {
        _builder.AppendLine("\tprivate void LoadAssets()")
                .AppendLine("\t{")
                .AppendLine("\t\tvar path = string.Empty;")
                .AppendLine("\t\tif (_uxml == null)")
                .AppendLine("\t\t{")
                .AppendLine("\t\t\tpath = AssetDatabase.GUIDToAssetPath(UxmlGUID);")
                .AppendLine("\t\t\t_uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);")
                .AppendLine("\t\t}")
                .AppendLine("\t}");

    }

    private void GenerateLoadAssetsGameplayFunction(string name)
    {
        _builder.AppendLine("\tprivate void LoadAssets()")
                .AppendLine("\t{")
                .AppendLine("\t\tif (_config == null)")
                .AppendLine($"\t\t\t_config = Resources.Load<UIAssetConfig>(\"Scriptables/{name}\");")
                .AppendLine()
                .AppendLine("\t\tif (_panelSettings == null && _config.HasPanelSettings)")
                .AppendLine("\t\t\t_panelSettings = _config.PanelSettings;")
                .AppendLine()
                .AppendLine("\t\tif (_panelSettings == null)")
                .AppendLine("\t\t\tthrow new InvalidOperationException(\"Unable to load Panel Settings.\");")
                .AppendLine()
                .AppendLine("\t\tif (_uxml == null && _config.HasUxml)")
                .AppendLine($"\t\t\t_uxml = _config.Uxml;")
                .AppendLine()
                .AppendLine("\t\tif (_uxml == null)")
                .AppendLine("\t\t\tthrow new InvalidOperationException(\"Unable to load UXML.\");")
                .AppendLine("\t}");
    }

    private string EscapeString(string value)
    {
        return value.Replace("\\", "\\\\")
                    .Replace("\"", "\\\"");
    }

    private void GenerateDataBindingInitialisationFunction(IEnumerable<UxmlDataBinding> bindings)
    {
        _builder.AppendLine("\tprivate void InitializeGeneratedBindings()")
                .AppendLine("\t{");

        foreach (var binding in bindings)
        {
            var fieldName = $"_{SanitizeVariableName(binding.ElementName)}";

            switch (binding.BindKind)
            {
                case UICodeGenBindKind.Value:
                    _builder.AppendLine( $"\t\t_dataBindings.Add(" +
                                         $"UICodeGenBindingFactory.Bind(" +
                                         $"_dataContext, " +
                                         $"\"{EscapeString(binding.SourcePath)}\", " +
                                         $"{fieldName}, " +
                                         $"\"{EscapeString(binding.TargetProperty)}\", " +
                                         $"UICodeGenBindMode.{binding.BindMode}));");
                    break;

                case UICodeGenBindKind.ItemsSource:
                    _builder.AppendLine( $"\t\t_dataBindings.Add(" +
                                         $"UICodeGenBindingFactory.BindItemsSource(" +
                                         $"_dataContext, " +
                                         $"\"{EscapeString(binding.SourcePath)}\", " +
                                         $"{fieldName}));");
                    break;

                case UICodeGenBindKind.SelectedItem:
                    _builder.AppendLine( $"\t\t_dataBindings.Add(" +
                                         $"UICodeGenBindingFactory.BindSelectedItem(" +
                                         $"_dataContext, " +
                                         $"\"{EscapeString(binding.SourcePath)}\", " +
                                         $"{fieldName}));");
                    break;

                case UICodeGenBindKind.SelectedIndex:
                    _builder.AppendLine( $"\t\t_dataBindings.Add(" +
                                         $"UICodeGenBindingFactory.BindSelectedIndex(" +
                                         $"_dataContext, " +
                                         $"\"{EscapeString(binding.SourcePath)}\", " +
                                         $"{fieldName}));");
                    break;

                case UICodeGenBindKind.None: //break
                default:
                    throw new ArgumentOutOfRangeException( nameof(binding.BindKind),
                                                           binding.BindKind,
                                                           "Unsupported data-binding kind.");
            }
        }

        _builder.AppendLine("\t}")
                .AppendLine();
    }

    private void GenerateRequireEditorMethod()
    {
        _builder.AppendLine("\tprivate T Require<T>(string name) where T : VisualElement")
                .AppendLine("\t{")
                .AppendLine("\t\tvar elem = rootVisualElement.Q<T>(name);")
                .AppendLine("\t\tif (elem == null)")
                .AppendLine("\t\t\tthrow new InvalidOperationException($\"Required UI element '{name}' of type Button was not found.\");")
                .AppendLine()
                .AppendLine("\t\treturn elem;")
                .AppendLine("\t}");
    }
    private void GenerateRequireGameplayMethod()
    {
        _builder.AppendLine("\tprivate T Require<T>(string name) where T : VisualElement")
                .AppendLine("\t{")
                .AppendLine("\t\tvar elem = _rootVisualElement.Q<T>(name);")
                .AppendLine("\t\tif (elem == null)")
                .AppendLine("\t\t\tthrow new InvalidOperationException($\"Required UI element '{name}' of type Button was not found.\");")
                .AppendLine()
                .AppendLine("\t\treturn elem;")
                .AppendLine("\t}");
    }

    private void GenerateInitializeEventsFunction(IEnumerable<UxmlEventBinding> bindings)
    {
        _builder.AppendLine("\tprivate void InitializeEvents()");
        _builder.AppendLine("\t{");

        foreach (var binding in bindings)
        {
            var fieldName = "_" + SanitizeVariableName(binding.ElementName);

            _builder.AppendLine($"\t\t{fieldName}.RegisterCallback<ClickEvent>({binding.HandlerName});");
        }

        _builder.AppendLine("\t}");
    }

    private void GenerateInitializeComponentsEditorFunction(IEnumerable<VisualElement> visualElements)
    {
        _builder.AppendLine("\tprotected void InitializeComponents()")
                .AppendLine("\t{")
                .AppendLine("\t\tLoadAssets();")
                .AppendLine()
                .AppendLine("\t\t_uxml.CloneTree(rootVisualElement);")
                .AppendLine();

        foreach (var elem in visualElements)
        {
            if (string.IsNullOrWhiteSpace(elem.name)) continue;

            var sanitizedName = SanitizeVariableName(elem.name);
            _builder.AppendLine($"\t\t_{SanitizeVariableName(elem.name)} = Require<{elem.GetType().Name}>(ElementName_{char.ToUpper(sanitizedName[0])}{sanitizedName.Substring(1)});");
        }


        _builder.AppendLine()
                .AppendLine("\t\tInitializeEvents();")
                .AppendLine("}");
    }

    private void GenerateInitializeComponentsGameplayFunction(IEnumerable<VisualElement> visualElements)
    {
        _builder.AppendLine("\tprotected void InitializeComponents()")
               .AppendLine("\t{")
               .AppendLine("\t\tLoadAssets();")
               .AppendLine()
               .AppendLine("\t\tif(_uiDoc == null) _uiDoc = GetComponent<UIDocument>();")
               .AppendLine()
               .AppendLine("\t\t_rootVisualElement = _uiDoc.rootVisualElement;")
               .AppendLine("\t\t_uiDoc.panelSettings = _panelSettings;")
               .AppendLine("\t\t_uiDoc.visualTreeAsset = _uxml;")
               .AppendLine("\t\t_uxml.CloneTree(_rootVisualElement);")
               .AppendLine();

        foreach (var elem in visualElements)
        {
            if (string.IsNullOrWhiteSpace(elem.name)) continue;

            var sanitizedName = SanitizeVariableName(elem.name);
            _builder.AppendLine($"\t\t_{SanitizeVariableName(elem.name)} = Require<{elem.GetType().Name}>(ElementName_{char.ToUpper(sanitizedName[0])}{sanitizedName.Substring(1)});");

        }

        _builder.AppendLine()
                .AppendLine("\t\tInitializeEvents();")
                .AppendLine("}");
    }

    private void GenerateShowWindowFunction(string name)
    {
        _builder.AppendLine($"\t[MenuItem(\"Tools/{name}\")]")
                .AppendLine("\tpublic static void OpenWindow()")
                .AppendLine("\t{")
                .AppendLine($"\t\tvar wnd = GetWindow<{name}>();")
                .AppendLine("\t\twnd.titleContent = new(UxmlName);")
                .AppendLine("\t\twnd.Show();")
                .AppendLine("\t}");
    }
    
    public void GenerateCreateGUIFunction()
    {
        _builder.AppendLine("\tprivate void CreateGUI()")
                .AppendLine("\t{")
                .AppendLine("\t\tInitializeComponents();")
                .AppendLine("\t}");
    }

    public void GenerateAwakeFunction()
    {
        _builder.AppendLine("\tprivate void Awake()")
                .AppendLine("\t{")
                .AppendLine("\t\tInitializeComponents();")
                .AppendLine("\t}");
    }

    private static void EnsureFolderExists(string folderPath)
    {
        var folders = folderPath.Split('/');
        var currentFolder = folders[0]; // Assets

        for (var i = 1; i < folders.Length; i++)
        {
            var nextFolder = $"{currentFolder}/{folders[i]}";

            if (!AssetDatabase.IsValidFolder(nextFolder))
            {
                AssetDatabase.CreateFolder(currentFolder, folders[i]);
            }

            currentFolder = nextFolder;
        }
    }


    private void WriteFile(string fileName, string relativePath, string suffix)
    {
        if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(relativePath)) return;

        var fullName = $"{fileName}.{suffix}";
        var dirPath = $"{Application.dataPath}/{relativePath}/";
        var filePath = $"{dirPath}{fullName}";

        if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

        File.WriteAllText(filePath, _builder.ToString(), Encoding.UTF8);
    }

    public void GenerateEditorFile(VisualTreeAsset uxml)
    {

        //if (!uxmlField.value || uxmlField.GetType() != typeof(VisualTreeAsset)) return;

        //var uxml = uxmlField.value as VisualTreeAsset;
        var ussStyleSheets = uxml.stylesheets.ToArray();

        var root = uxml.Instantiate();
        var visualElements = ExtractVisualElements(root);
        var clickBindings = ExtractClickBindings(visualElements);
        var dataBindings = ExtractDataBindings(visualElements);

        _validator.Validate(uxml, visualElements);

        _builder.Clear();
        GenerateEditorGeneratedHeader(uxml.name);
        GenerateEditorConstants(uxml, ussStyleSheets, visualElements);
        GenerateInClassSpace();
        GenerateEditorFields(visualElements);
        GenerateInClassSpace();
        GenerateProperties();
        GenerateInClassSpace();
        GenerateGeneratedClickableCallbacks(clickBindings);
        GenerateInClassSpace();
        GenerateBindingContext();
        GenerateInClassSpace();
        GenerateLoadAssetsEditorFunction();
        _builder.AppendLine();
        GenerateDataBindingInitialisationFunction(dataBindings);
        _builder.AppendLine();
        GenerateShowWindowFunction(uxml.name);
        _builder.AppendLine();
        GenerateRequireEditorMethod();
        _builder.AppendLine();
        GenerateInitializeEventsFunction(clickBindings);
        _builder.AppendLine();
        GenerateInitializeComponentsEditorFunction(visualElements);

        _builder.AppendLine("}");

        WriteFile( uxml.name, "Editor/UI/generated", "generated.cs");


        _builder.Clear();


        if (!File.Exists($"{Application.dataPath}/Editor/Scriptables/{uxml.name}.asset"))
        {

            var scriptableFolderPath = $"Assets/Editor/Scriptables/";


            EnsureFolderExists(scriptableFolderPath);


            var assetConfig = ScriptableObject.CreateInstance<UIAssetConfig>();
            assetConfig.Uxml = uxml;
            assetConfig.PanelSettings = Resources.Load<PanelSettings>("UI/Panel Settings/RuntimePanelSettings");

            var scriptableAssetPath = $"{scriptableFolderPath}{uxml.name}.asset";


            AssetDatabase.CreateAsset(assetConfig, scriptableAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = assetConfig;

            Debug.Log($"Created: {scriptableAssetPath}");
        }

        if (File.Exists($"{Application.dataPath}/Editor/UI/{uxml.name}.cs")) return;

        GenerateEditorHeader(uxml.name);
        GenerateClickableCallbacks(clickBindings);
        GenerateInClassSpace();
        GenerateCreateGUIFunction();
        _builder.AppendLine("}");

        WriteFile(uxml.name, "Editor/UI", "cs");

        _builder.Clear();
    }


    public void GenerateGameplayFile(VisualTreeAsset uxml)
    {
        var ussStyleSheets = uxml.stylesheets.ToArray();

        var root = uxml.Instantiate();
        var visualElements = ExtractVisualElements(root);
        var clickBindings = ExtractClickBindings(visualElements);
        var dataBindings = ExtractDataBindings(visualElements);

        _validator.Validate(uxml, visualElements);

        _builder.Clear();
        GenerateGameplayGeneratedHeader(uxml.name);
        GenerateGameplayConstants(visualElements);
        GenerateInClassSpace();
        GenerateGameplayFields(visualElements);
        GenerateInClassSpace();
        GenerateProperties();
        GenerateInClassSpace();
        GenerateGeneratedClickableCallbacks(clickBindings);
        GenerateInClassSpace();
        GenerateBindingContext();
        GenerateInClassSpace();
        GenerateLoadAssetsGameplayFunction(uxml.name);
        _builder.AppendLine();
        GenerateDataBindingInitialisationFunction(dataBindings);
        _builder.AppendLine();
        GenerateRequireGameplayMethod();
        _builder.AppendLine();
        GenerateInitializeEventsFunction(clickBindings);
        _builder.AppendLine();
        GenerateInitializeComponentsGameplayFunction(visualElements);

        _builder.AppendLine("}");

        WriteFile(uxml.name, "Scripts/UI/generated", "generated.cs");

        _builder.Clear();

        if (!File.Exists($"{Application.dataPath}/Resources/Scriptables/{uxml.name}.asset"))
        {

            var scriptableFolderPath = $"Assets/Resources/Scriptables/";


            EnsureFolderExists(scriptableFolderPath);


            var assetConfig = ScriptableObject.CreateInstance<UIAssetConfig>();
            assetConfig.Uxml = uxml;
            assetConfig.PanelSettings = Resources.Load<PanelSettings>("UI/Panel Settings/RuntimePanelSettings");
             
            var scriptableAssetPath = $"{scriptableFolderPath}{uxml.name}.asset";


            AssetDatabase.CreateAsset(assetConfig, scriptableAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = assetConfig;

            Debug.Log($"Created: {scriptableAssetPath}");
        }

        if (File.Exists($"{Application.dataPath}/Scripts/UI/{uxml.name}.cs")) return;

        GenerateGameplayHeader(uxml.name);
        GenerateClickableCallbacks(clickBindings);
        GenerateInClassSpace();
        GenerateAwakeFunction();
        _builder.AppendLine("}");

        WriteFile(uxml.name, "Scripts/UI", "cs");

        _builder.Clear();
    }
}
