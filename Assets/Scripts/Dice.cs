using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [SerializeField] private int minValue = 1;
    [SerializeField] private int maxValue = 6;

    private int _diceValue;

    private Animator _animator;

    public int DiceValue { get { return _diceValue; } }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // // 주사위를 굴려 값을 반환
    // public int Roll()
    // {
    //     _diceValue = UnityEngine.Random.Range(minValue, maxValue + 1);
    //     Debug.Log(_diceValue);
    //     return _diceValue;
    // }

    //[Rpc(SendTo.Everyone)]
    public void PlayDiceAnimationClient(int diceValue) //일단 값 받아와봄봄
    {
        StartCoroutine(RollCoroutine());
    }

    IEnumerator RollCoroutine(Action callback = null)
    {
        _diceValue = UnityEngine.Random.Range(minValue, maxValue + 1);
        _animator.SetTrigger("RollTrigger");
        yield return new WaitForSeconds(0.1f);
        float animationTime = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationTime);
        callback?.Invoke();
        gameObject.SetActive(false);
    }
}
