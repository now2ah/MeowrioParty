using Unity.Netcode;
using UnityEngine;

public enum AnimationType
{
    Idle,
    Roll,
}

public class DiceController : NetworkBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlayAnimation(AnimationType type)
    {
        if (type == AnimationType.Idle)
        {

        }
        else if (type == AnimationType.Roll)
        {
            _animator.SetTrigger("RollTrigger");
        }
    }
}
