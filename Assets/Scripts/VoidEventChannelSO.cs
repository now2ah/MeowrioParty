using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Events/Void Event Channel")]
public class VoidEventChannelSO : ScriptableObject
{
    public event Action OnEventRaised;
    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }
}
