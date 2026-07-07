using System;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public partial class InGameHUD : MonoBehaviour, IReflectable
{
	private void Awake()
	{
		InitializeComponents();

		var res = Screen.currentResolution;
		_panelSettings.referenceResolution = new(res.width, res.height);
	}
}
