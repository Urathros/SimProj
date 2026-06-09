using System;

public interface IFlow : IDisposable
{
    bool Tick();

    void Complete();

    void Apply();
}
