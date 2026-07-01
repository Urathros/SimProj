using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class GameFlowManagerDebuggerWindow : EditorWindow
{
    private const string FlowRowLabelName = "flow-row";
    private const string FlowIdLabelName = "flow-id";
    private const string FlowTypeLabelName = "flow-type";
    private const string FlowValidLabelName = "flow-valid";

    /*************************************************************************/
    #region Fields
    /*************************************************************************/

    private static IDebugUI _target = null;

    private GameFlowManagerDebugSnapshot _snapshot;

    #endregion
    /*************************************************************************/

    /*************************************************************************/
    #region Callbacks
    /*************************************************************************/
    private VisualElement HandleFlowRowMake()
    {
        var row = new VisualElement();
        row.AddToClassList(FlowRowLabelName);

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
            _flowList.itemsSource = Array.Empty<GameFlowDebugInfo>();
            _flowList.Rebuild();
            return;
        }

        _snapshot = _target.CreateDebugSnapshot();

        if (_statusLabel == null) return;
        _statusLabel.text = _snapshot.IsDisposed ? "Status: Disposed" : "Status: Alive";

        _countLabel.text = $"Active Flows: {_snapshot.ActiveFlowCount}";

        _flowList.itemsSource = _snapshot.ActiveFlows.ToList();
        _flowList.Rebuild();
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
        InitializeComponents();

        _flowList.fixedItemHeight = 24;
        _flowList.makeItem = HandleFlowRowMake;
        _flowList.bindItem = HandleFlowRowBind;
        _flowList.selectionType = SelectionType.Single;

        rootVisualElement.Q<Button>(ElementName_RefreshButton).clicked += Refresh;

        Refresh();

        GameFlowManagerDebugHooks.UpdateDel += Refresh;
    }

    private void OnDestroy()
    {
        GameFlowManagerDebugHooks.UpdateDel -= Refresh;
    }
}
