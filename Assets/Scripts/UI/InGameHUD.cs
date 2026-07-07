using System;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public partial class InGameHUD : MonoBehaviour, IReflectable
{
	private void Awake()
	{
		InitializeComponents();
	}
}
