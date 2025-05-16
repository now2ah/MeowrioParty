using System;
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

    private Queue<Tile> _moveQueue = new Queue<Tile>();
    private bool isMoving = false;
    [SerializeField] float moveSpeed = 5f;

    private void Update()
    {
        if (_moveQueue.Count > 0 && !isMoving) //움직이고 있지 않고 큐가 차면
        {
            isMoving = true;
            StartCoroutine(MoveTileQueue());
        }
    }
    public int RollDice()
    {
        return _dice.Roll();
    }

    public void GetMoveQueue(Queue<Tile> tileQueue)
    {
        _moveQueue.Clear();
        _moveQueue = tileQueue;//타일 큐를 받아와서
    }

    private IEnumerator MoveTileQueue()
    {
        //int _diceValueUI = _dice.DiceValue-1;
        //_diceNumberObjects[_diceValueUI].SetActive(true);
        for (int i = 0; i < _moveQueue.Count; i++)
        {
            Tile nextTile = _moveQueue.Dequeue();

/*            int current = nextTile.tileIndex - i;
            int next = nextTile.tileIndex - i - 1;
            if (current < _diceNumberObjects.Count && next >= 0)
            {
                if (_diceNumberObjects[current] != null)
                    _diceNumberObjects[current].SetActive(false);
                if (_diceNumberObjects[next] != null)
                    _diceNumberObjects[next].SetActive(true);
            }
*/
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
            yield return new WaitForSeconds(0.1f);

        }
        isMoving = false;
        _dice.gameObject.SetActive(false);
    }
        

    // comment: 아래 로직은 필히 없어져야 함.
    private IEnumerator MoveCoroutine(Tile nextTile)
    {
        transform.position = nextTile.transform.position;
        currentTile = nextTile;
        yield return new WaitForSeconds(1f);
    }
}
