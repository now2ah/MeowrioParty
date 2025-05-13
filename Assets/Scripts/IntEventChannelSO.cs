using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Events/Int Event Channel")]
public class IntEventChannelSO : ScriptableObject
{
    public event Action<int> OnEventRaised;
    public void RaiseEvent(int value)
    {
        OnEventRaised?.Invoke(value);
    }
}
