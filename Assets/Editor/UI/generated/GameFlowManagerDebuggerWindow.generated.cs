using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public partial class GameFlowManagerDebuggerWindow : EditorWindow
{
	/*************************************************************************/
	#region Constants
	/*************************************************************************/
	private const string UxmlGUID = "2c2616c995aa0cd4b8716f7f415ccb60";
	private const string UxmlName = "Game Flow Manager Debugger";
	private const string RootText = "root";
	private const string RefreshButtonText = "refresh-button";
	private const string FlowListText = "flow-list";
	private const string CountLabelText = "count-label";
	private const string StatusLabelText = "status-label";
	private const string TitleText = "title";
	#endregion
	/*************************************************************************/



	/*************************************************************************/
	#region Fields
	/*************************************************************************/
	private VisualElement _root = null;
	private Button _refreshButton = null;
	private ListView _flowList = null;
	private Label _countLabel = null;
	private Label _statusLabel = null;
	private Label _title = null;

	[SerializeField]
	private VisualTreeAsset _uxml = null;
	#endregion
	/*************************************************************************/



	private void LoadAssets()
	{
		var path = string.Empty;
		if (_uxml == null)
		{
			path = AssetDatabase.GUIDToAssetPath(UxmlGUID);
			_uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
		}
	}

	[MenuItem("Tools/GameFlowManagerDebuggerWindow")]
	public static void OpenWindow()
	{
		var wnd = GetWindow<GameFlowManagerDebuggerWindow>();
		wnd.titleContent = new(UxmlName);
		wnd.Show();
	}

	protected void InitializeComponents()
	{
		LoadAssets();

		_uxml.CloneTree(rootVisualElement);

		_root = rootVisualElement.Q<VisualElement>(RootText) as VisualElement;
		_refreshButton = rootVisualElement.Q<Button>(RefreshButtonText) as Button;
		_flowList = rootVisualElement.Q<ListView>(FlowListText) as ListView;
		_countLabel = rootVisualElement.Q<Label>(CountLabelText) as Label;
		_statusLabel = rootVisualElement.Q<Label>(StatusLabelText) as Label;
		_title = rootVisualElement.Q<Label>(TitleText) as Label;
}
}
