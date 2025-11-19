using UnityEngine;

public abstract class Command
{
    public virtual void Execute(GameObject target)
    {
        
    }

    ~Command()
    {
        
    }
}
