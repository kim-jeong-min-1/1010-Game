using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using JM.Tweening;

public enum CellState
{
    Empty,
    Fill,
    line
}
public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; set; }
    const int CELL_SIZE = 10;

    [SerializeField] private AnimationCurve movementCurve;
    [SerializeField] Transform[] BlockSpawnPoints;
    [SerializeField] GameObject[] BlockObj;
    public int[,] Cell = new int[CELL_SIZE, CELL_SIZE];

    [SerializeField] private Transform BorderFront;
    [SerializeField] private GameObject[] cellObject = new GameObject[100];
    [SerializeField] private Text ScoreText;
    [SerializeField] private GameObject DieText;

    private Color CellColor;
    private int score = 0;
    public int Score
    {
        get {
            return score;
        }
        set {
            score = value;
            ScoreText.text = $"{score}";
        }
    } 
    void Awake()
    {
        transform.DoMove(new Vector3(0, 0, 1), 1f);
        Inst = this;
        SpawnBlock();
        LoadCells();
        CellColor = cellObject[0].GetComponent<SpriteRenderer>().color;
    }

    #region 블럭 처리
    //블럭들끼리 닿았는지 확인
    bool BlockCollision(int x, int y)
    {
        if (Cell[x, y] == (int)CellState.Fill) return true;
        return false;
    }

    //블럭이 10x10 칸을 넘어갔는지 확인
    bool OutCheck(int x, int y)
    {
        if (x < 0 || y < 0 || x >= CELL_SIZE || y >= CELL_SIZE) return true;
        return false;
    }

    //x,y 값에 대응되는 cellObject를 찾아서 반환함
    GameObject GetCell(int x, int y)
    {
        return cellObject[(y * 10) + x];
    }
    #endregion

    //블럭을 놓았을 때 처리
    public void PutPuzzle(Block block, Vector3[] ShapeCell, Vector3 lastPos)
    {
        for (int i = 0; i < ShapeCell.Length; i++)
        {
            Vector3 Pos = ShapeCell[i] + lastPos;
            if (OutCheck((int)Pos.x, (int)Pos.y))
            {
                block.BlockReturn();
                return;
            }
            if (BlockCollision((int)Pos.x, (int)Pos.y))
            {
                block.BlockReturn(); 
                return;
            }
        }

        for (int i = 0; i < ShapeCell.Length; i++)
        {
            Vector3 Pos = ShapeCell[i] + lastPos;
            print("채워짐");
            Cell[(int)Pos.x, (int)Pos.y] = (int)CellState.Fill;
            GetCell((int)Pos.x, (int)Pos.y).GetComponent<SpriteRenderer>().color =
                block.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        }
        block.DeleteBlock();
        ChekcLine(ShapeCell.Length);
        Invoke("CheckLogic", 0.25f);
    }

    void ChekcLine(int ShapeCount)
    {
        int line = 0;
        //한줄이 완성됬는지 체크
        for (int y = 0; y < CELL_SIZE; y++)
        {
            int HorizontalCount = 0;
            int VerticalCount = 0;

            for (int x = 0; x < CELL_SIZE; x++)
            {
                if (Cell[x, y] == (int)CellState.Fill) HorizontalCount++;
                if (Cell[y, x] == (int)CellState.Fill) VerticalCount++;

                if(HorizontalCount == 10)
                {
                    line++;
                    for (int i = 0; i < CELL_SIZE; i++) Cell[i, y] = (int)CellState.line;
                }

                if (VerticalCount == 10)
                {
                    line++;
                    for (int i = 0; i < CELL_SIZE; i++) Cell[y, i] = (int)CellState.line;
                }
            }
        }

        //파괴
        for(int x = 0; x < CELL_SIZE; x++)
        {
            for (int y = 0; y < CELL_SIZE; y++)
            {
                if(Cell[x, y] == (int)CellState.line)
                {
                    Cell[x, y] = (int)CellState.Empty;
                    StartCoroutine(BlockMove(GetCell(x, y), Vector3.zero, false, 0.2f, CellRecycling));
                }
            }
        }
        Score += line * 10 + ShapeCount; 
    }

    //스폰된 블럭이 모두 배치되었는지 확인
    void CheckLogic()
    {
        int count = 0;
        int dieCheck = 0;
        for (int i = 0; i < BlockSpawnPoints.Length; i++)
        {
            if (BlockSpawnPoints[i].childCount != 0)
            {
                count++;
                if (PutUnable(BlockSpawnPoints[i].GetComponentInChildren<Block>().ShapePos)) dieCheck++;   
            }
        }
        if (count == 0) SpawnBlock();
        else if (count == dieCheck) Die();
    }

    //모든 좌표중 해당 블럭이 들어갈 좌표가 있는 지 전체적으로 확인
    bool PutUnable(Vector3[] ShapePos)
    {
        int Check = 0;
        for (int y = 0; y < CELL_SIZE; y++)
        {
            for (int x = 0; x < CELL_SIZE; x++)
            {
                if (!UnableChecking(x, y, ShapePos)) Check++;
            }
        }
        if (Check == 0) return true;
        else return false;
    }

    //인자로 받은 좌표에 해당 블럭이 들어갈 수 있는 지 체크
    bool UnableChecking(int x, int y, Vector3[] ShapePos)
    {
        for (int i = 0; i < ShapePos.Length; i++)
        {
            Vector3 SumPos = GetCell(x, y).transform.position + ShapePos[i];

            if (OutCheck((int)SumPos.x, (int)SumPos.y)) return true;
            if (BlockCollision((int)SumPos.x, (int)SumPos.y)) return true;
        }
        return false;
    }

    //게임 오버 처리
    void Die()
    {
        print("Die");
        DieText.SetActive(true);
    }

    //블럭 스폰
    void SpawnBlock()
    {
        for (int i = 0; i < BlockSpawnPoints.Length; i++)
        {
            int Rand = Random.Range(0, BlockObj.Length);

            GameObject clone = Instantiate(BlockObj[Rand],
                BlockSpawnPoints[i].position + new Vector3(10, 0, 0), Quaternion.identity, BlockSpawnPoints[i].transform);

            clone.GetComponent<Block>().SetUP(BlockSpawnPoints[i].position, 0.5f);
        }

        Invoke("CheckLogic", 0.05f);
    }

    //Cell들을 배열에 순차적으로 담기
    void LoadCells()
    {
        int num = 0;
        foreach(Transform cell in BorderFront)
        {
            cellObject[num] = cell.gameObject;
            num++;
        }
    }

    //부드러운 움직임
    public IEnumerator BlockMove(GameObject obj, Vector3 endPos, bool isMove, float time, Action<GameObject> CallBack = null)
    {
        Vector3 startPos;
        startPos = (isMove) ? obj.transform.position : obj.transform.localScale; 

        float current = 0;
        float percent = 0;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            if (isMove) obj.transform.position = Vector3.Lerp(startPos, endPos, movementCurve.Evaluate(percent));
            else obj.transform.localScale = Vector3.Lerp(startPos, endPos, movementCurve.Evaluate(percent));

            yield return null;
        }
        CallBack?.Invoke(obj);
    }

    void CellRecycling(GameObject Cell)
    {
        Cell.transform.localScale = Vector3.one * 0.9f;
        Cell.GetComponent<SpriteRenderer>().color = CellColor;
    }
}
