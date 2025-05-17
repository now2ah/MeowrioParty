using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// COMMENT : 플레이어 이동과 관련된 로직만 처리하면 됨
// COMMENT : BoardManager가 가야할 Tile을 알려줌

public class Player : MonoBehaviour
{
    public Tile currentTile; // COMMENT: 게임 진행과 관련된 부분이므로 BoardManager가 갖고 있어야 함.

    [SerializeField] public Dice _dice;
    [SerializeField] private List<GameObject> _diceNumberObjects = new List<GameObject>(); // Comment: 아래 변수도 불필요

    private bool _isMoving = false;
    [SerializeField] private float moveSpeed = 5f;

    public bool IsMoving { get { return _isMoving; } }

    public int RollDice()
    {
        _dice.gameObject.SetActive(true);
        return _dice.Roll();
    }

    public void MoveTo(Tile nextTile)
    {
        if (!_isMoving)
        {
            StartCoroutine(MoveToSequenceCo(nextTile));
        }
    }

    private IEnumerator MoveToSequenceCo(Tile nextTile)
    {
        _isMoving = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = nextTile.transform.position;
        float distance = Vector3.Distance(startPos, endPos);
        float duration = distance / moveSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos; // 최종 위치 보정
        currentTile = nextTile;

        _isMoving = false;
    }

 
}
