using System;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour, ISingleton 
    where T : Singleton<T>
{

	private static T _instance = default;

	private static bool _isApplicationQuitting = false;
    private static bool _isRegistered = false;


    private static readonly object SyncRoot = new();
    protected static string Name => typeof(T).Name;

    public static T Instance
	{
		get 
		{
            EnsureRegistered();


            if (_isApplicationQuitting)
			{
                SingletonDiagnostics.RequestedWhileQuitting<T>();

                return null;
			}

            if (_instance != null) return _instance;

            lock (SyncRoot)
            {
                if (_instance != null) return _instance;

                _instance = FindFirstObjectByType<T>();

                if (_instance != null) return _instance;

                if (InitializeDel != null)
				{
					_instance = InitializeDel.Invoke();
                    if (_instance != null) return _instance;
                }

				var go = new GameObject(Name);
				_instance = go.AddComponent<T>();

                return _instance;
            }
		}
	}
    public static bool HasInstance => _instance != null;

    public bool IsInitialized => _instance == this;

    /// <summary>
    /// Optional custom creation logic.
    /// Example: load prefab from Resources, Addressables, bootstrap scene, etc.
    /// </summary>
    protected static Func<T> InitializeDel = () => default;



    private static void EnsureRegistered()
    {
        if (_isRegistered) return;

        _isRegistered = true;

        SingletonResetRegistry.Register(ResetStatics);
    }

    private static void ResetStatics()
    {
        if (_instance is IResettableSingleton resettable)
            resettable.ResetState();

        _instance = null;
        _isApplicationQuitting = false;
        InitializeDel = () => default;
    }


    protected virtual void Awake()
    {
        EnsureRegistered();

        if (_instance != null && !IsInitialized)
        {
            SingletonDiagnostics.DuplicateDestroyed<T>(gameObject);

            Destroy(gameObject);

            return;
        }

        _instance = (T)this;

        if (this is IPersistentSingleton) DontDestroyOnLoad(this);
    }

    protected virtual void OnApplicationQuit() => _isApplicationQuitting = true;

    protected virtual void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
}
