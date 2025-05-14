using System;
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

    private void OnEnable()
    {
        _onRollButtonClicked.OnEventRaised += RollDice;
    }

    private void OnDisable()
    {
        _onRollButtonClicked.OnEventRaised -= RollDice;
    }


    // 주사위를 굴려 값을 반환
    private void RollDice()
    {
        int value = UnityEngine.Random.Range(minValue, maxValue + 1);
        _onDiceRolled.RaiseEvent(value);
        Debug.Log(value);
    }
}
