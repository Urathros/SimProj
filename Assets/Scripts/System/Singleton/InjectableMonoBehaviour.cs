using UnityEngine;

public abstract class InjectableMonoBehaviour : MonoBehaviour
{
    protected virtual void Awake()
    {
        SingletonInjector.Inject(this);
    }
}
