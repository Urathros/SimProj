using System;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public partial class InGameHUD : MonoBehaviour, IReflectable
{
	partial void HandleTimeXQuadrupleClick(ClickEvent e)
	{
	}

	partial void HandleTimeXDoubleClick(ClickEvent e)
	{
	}

	partial void HandleTimeXHalfClick(ClickEvent e)
	{
	}

	partial void HandleStartPauseClick(ClickEvent e)
	{
	}




	private void Awake()
	{
		InitializeComponents();
	}
}
