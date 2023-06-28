using Mkey;
using System;
using System.Diagnostics;
using UnityEngine.UI;

[Serializable]
public struct Timer
{
    public float delay;
    public float value { get; set; }

    public event Action OnReset;

    public void Reset(bool invokeEvent = true)
    {
        value = 0;
        if (invokeEvent)
            OnReset?.Invoke();
    }

    public void SetValue(float newValue)
    {
       
        value = newValue;
    }

    public void Update(float dt)
    {
        value += dt;

        if (value > delay /*TODO: RE MOVE*/&& !CellLogic.Instance.isDragging)
        {
            Reset();
        }
    }
}
