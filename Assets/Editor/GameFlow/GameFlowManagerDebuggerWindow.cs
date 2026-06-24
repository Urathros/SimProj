using Codice.Client.Common.GameUI;
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
public class GameFlowManagerDebuggerWindow : EditorWindow
{
    private const string FlowIdLabelName = "flow-id";
    private const string FlowTypeLabelName = "flow-type";
    private const string FlowValidLabelName = "flow-valid";

    /*************************************************************************/
    #region Fields
    /*************************************************************************/

    private static IDebugUI _target = null;

    private Label _statusLabel = null;
    private Label _countLabel = null;
    private ListView _flowListView = null;

    private GameFlowManagerDebugSnapshot _snapshot;

    [SerializeField]
    private VisualTreeAsset _uxml = null;

    [SerializeField]
    private StyleSheet _uss = null;
    #endregion
    /*************************************************************************/

    /*************************************************************************/
    #region Callbacks
    /*************************************************************************/
    private VisualElement HandleFlowRowMake()
    {
        var row = new VisualElement();
        row.AddToClassList("flow-row");

        var id = new Label { name = FlowIdLabelName };
        id.AddToClassList(FlowIdLabelName);

        var type = new Label { name = FlowTypeLabelName };
        type.AddToClassList(FlowTypeLabelName);

        var valid = new Label { name = FlowValidLabelName };
        valid.AddToClassList(FlowValidLabelName);

        row.Add(id);
        row.Add(type);
        row.Add(valid);

        return row;
    }

    private void HandleFlowRowBind(VisualElement element, int index)
    {
        var flow = _snapshot.ActiveFlows[index];

        element.Q<Label>(FlowIdLabelName).text = flow.FlowId;
        element.Q<Label>(FlowTypeLabelName).text = flow.FlowType;
        element.Q<Label>(FlowValidLabelName).text = flow.IsValid ? "Valid" : "Invalid";
    }



    private static void HandleDebugStateChanged() 
        => GetWindow<GameFlowManagerDebuggerWindow>().Refresh();


    #endregion
    /*************************************************************************/

    private void Refresh()
    {
        if (_target == null)
        {
            _statusLabel.text = "Status: No GameFlowManager registered.";
            _countLabel.text = "Active Flows: 0";
            _flowListView.itemsSource = Array.Empty<GameFlowDebugInfo>();
            _flowListView.Rebuild();
            return;
        }

        _snapshot = _target.CreateDebugSnapshot();

        if (_statusLabel == null) return;
        _statusLabel.text = _snapshot.IsDisposed ? "Status: Disposed" : "Status: Alive";

        _countLabel.text = $"Active Flows: {_snapshot.ActiveFlowCount}";

        _flowListView.itemsSource = _snapshot.ActiveFlows.ToList();
        _flowListView.Rebuild();
    }

    [MenuItem("Tools/Game Flow Debugger")]
    public static void OpenWindow()
    {
        var wnd = GetWindow<GameFlowManagerDebuggerWindow>();
        wnd.titleContent = new("Game Flow Debugger");
        wnd.Show();
    }

    public static void SetTarget(IDebugUI target)
    {
        if (_target != null)
        {
            _target.DebugStateChangedDel -= HandleDebugStateChanged;
        }

        _target = target;

        if (_target != null)
        {
            _target.DebugStateChangedDel += HandleDebugStateChanged;
        }
        GetWindow<GameFlowManagerDebuggerWindow>().Refresh();
    }

    private void CreateGUI()
    {
        //rootVisualElement.style.paddingLeft = 12;
        //rootVisualElement.style.paddingRight = 12;
        //rootVisualElement.style.paddingTop = 12;

        //var title = new Label("Game Flow Manager");
        //title.style.unityFontStyleAndWeight = FontStyle.Bold;
        //title.style.fontSize = 18;
        //rootVisualElement.Add(title);

        //_statusLabel = new();
        //_countLabel = new();

        //rootVisualElement.Add(_statusLabel);
        //rootVisualElement.Add(_countLabel);

        //_flowListView = new ListView 
        //{
        //    fixedItemHeight = 24,
        //    makeItem = HandleFlowRowMake,
        //    bindItem = HandleFlowRowBind,
        //    selectionType = SelectionType.Single
        //};

        //_flowListView.style.flexGrow = 1;
        //_flowListView.style.marginTop = 10;

        //rootVisualElement.Add(_flowListView);

        //var refreshButton = new Button(Refresh)
        //{
        //    text = "Refresh"
        //};
        //rootVisualElement.Add(refreshButton);
        _uxml.CloneTree(rootVisualElement);
        rootVisualElement.styleSheets.Add(_uss);

        _statusLabel = rootVisualElement.Q<Label>("status-label");
        _countLabel = rootVisualElement.Q<Label>("count-label");
        _flowListView = rootVisualElement.Q<ListView>("flow-list");

        _flowListView.fixedItemHeight = 24;
        _flowListView.makeItem = HandleFlowRowMake;
        _flowListView.bindItem = HandleFlowRowBind;
        _flowListView.selectionType = SelectionType.Single;

        rootVisualElement.Q<Button>("refresh-button").clicked += Refresh;

        Refresh();

        GameFlowManagerDebugHooks.UpdateDel += Refresh;
    }

    private void OnDestroy()
    {
        GameFlowManagerDebugHooks.UpdateDel -= Refresh;
    }
    //private void OnInspectorUpdate() => Refresh();
}

#endif