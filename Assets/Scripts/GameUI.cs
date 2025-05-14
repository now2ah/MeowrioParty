using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Button _rollDiceButton;
    [SerializeField] private VoidEventChannelSO _onRollButtonClicked;

    private void Awake()
    {
        _rollDiceButton.onClick.AddListener(() => { _onRollButtonClicked.RaiseEvent(); });
    }
}
