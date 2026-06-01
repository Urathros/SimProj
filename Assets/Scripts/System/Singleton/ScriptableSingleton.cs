using UnityEngine;
using Unity.VisualScripting;


#if UNITY_EDITOR

using UnityEditor;
using System.IO;

#endif //!UNITY_EDITOR

public abstract class ScriptableSingleton<T> : ScriptableObject, ISingleton
    where T : ScriptableSingleton<T>
{
    private const string FolderName = "System/Singleton";

	private static T _instance = (T)default;

    private static bool _isInitialized = false;
    private static bool _isRegistered = false;

    private static string Name => typeof(T).Name;
    private static string Path => $"{FolderName}/{Name}";


    public static T Instance
	{
		get
        {
            EnsureRegistered();

            if (_instance != null) return _instance;

            LoadOrCreateInstance();

            return _instance;
        }
	}

    public static bool HasInstance => _instance != null;

    public bool IsInitialized => _instance == this;

    private static void LoadOrCreateInstance()
    {
        if (_isInitialized) return;

        _isInitialized = true;

        _instance = Resources.Load<T>(Path);

        if (HasInstance) return;

#if UNITY_EDITOR
        _instance = ScriptableSingletonEditorUtility.CreateAsset<T>(
            $"Assets/Resources/{Path}.asset"
        );
#else
        SingletonDiagnostics.MissingScriptableAsset<T>(ResourcePath);
#endif
    }

    private static void EnsureRegistered()
    {
        if (_isRegistered)
            return;

        _isRegistered = true;
        SingletonResetRegistry.Register(ResetStatics);
    }

    private static void ResetStatics()
    {
        _instance = null;
        _isInitialized = false;
        _isRegistered = false;
    }

    protected virtual void OnEnable()
    {
        EnsureRegistered();

        if (_instance == null)
            _instance = (T)this;
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        ScriptableSingletonEditorUtility.EnsureSingleAsset<T>();
    }
#endif
}

