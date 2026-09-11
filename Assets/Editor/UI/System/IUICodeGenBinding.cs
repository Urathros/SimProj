using System;

public interface IUICodeGenBinding : IDisposable
{
    void RefreshTarget();
    void RefreshSource();
}
