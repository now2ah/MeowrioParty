using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [SerializeField]
    private IntEventChannelSO _onDiceRolled;
    [SerializeField]
    private VoidEventChannelSO _onRollButtonClicked;

    [SerializeField]
    private int minValue = 1;
    [SerializeField]
    private int maxValue = 6;

    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        //gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _onRollButtonClicked.OnEventRaised += RollDice;
    }

    private void OnDisable()
    {
        _onRollButtonClicked.OnEventRaised -= RollDice;
    }

    public void MoveTo(GameObject obj)
    {
        float offset = 5.0f;
        transform.position = obj.transform.position + new Vector3(0f, offset, 0f);
    }

    public void PlayDiceAnimation(Action callback)
    {
        _animator.SetTrigger("RollTrigger");
        StartCoroutine(RollAnimationCoroutine(callback));
    }

    IEnumerator RollAnimationCoroutine(Action callback)
    {
        yield return new WaitForSeconds(0.1f);
        float animationTime = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationTime);
        gameObject.SetActive(false);
        callback.Invoke();
        Debug.Log("dice number 출력!");
    }

    // 주사위를 굴려 값을 반환
    private void RollDice()
    {
        
        int value = UnityEngine.Random.Range(minValue, maxValue + 1);
        _onDiceRolled.RaiseEvent(value);
        Debug.Log(value);
    }
}
