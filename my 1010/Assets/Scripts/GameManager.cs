using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public enum CellState
{
    Empty,
    Fill,
    line
}
public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; set; }

    private const int CELL_SIZE = 10;
    private int[,] Cell = new int[CELL_SIZE, CELL_SIZE];

    [SerializeField] private AnimationCurve movementCurve;
    [SerializeField] private Transform[] blockSpawnPoints;
    [SerializeField] private Block[] blockObj;

    [SerializeField] private GameObject[] cellObject = new GameObject[100];
    [SerializeField] private Transform borderFront;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject dieText;

    private Color defaultCellColor;
    private int score = 0;
    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
            scoreText.text = $"{score}";
        }
    }
    void Awake() => Inst = this;
    private void Start()
    {
        LoadCells();
        SpawnBlock();
        defaultCellColor = cellObject[0].GetComponent<SpriteRenderer>().color;
    }

    #region �� ó��
    //���鳢�� ��Ҵ��� Ȯ��
    bool BlockCollision(int x, int y)
    {
        if (Cell[x, y] == (int)CellState.Fill) return true;
        return false;
    }

    //���� 10x10 ĭ�� �Ѿ���� Ȯ��
    bool OutCheck(int x, int y)
    {
        if (x < 0 || y < 0 || x >= CELL_SIZE || y >= CELL_SIZE) return true;
        return false;
    }

    //x,y ���� �����Ǵ� cellObject�� ã�Ƽ� ��ȯ��
    GameObject GetCell(int x, int y)
    {
        return cellObject[(y * 10) + x];
    }
    #endregion

    //���� ������ �� ó��
    public void PutPuzzle(Block block, Vector3[] shapeCell, Vector3 lastPos)
    {
        for (int i = 0; i < shapeCell.Length; i++)
        {
            Vector3 Pos = shapeCell[i] + lastPos;
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

        for (int i = 0; i < shapeCell.Length; i++)
        {
            Vector3 Pos = shapeCell[i] + lastPos;
            
            Cell[(int)Pos.x, (int)Pos.y] = (int)CellState.Fill;
            GetCell((int)Pos.x, (int)Pos.y).GetComponent<SpriteRenderer>().color =
                block.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        }
        block.DeleteBlock();
        ChekcLine(shapeCell.Length);
        CheckLogic();
    }

    void ChekcLine(int shapeCount)
    {
        int line = 0;
        //������ �ϼ������ üũ
        for (int y = 0; y < CELL_SIZE; y++)
        {
            int HorizontalCount = 0;
            int VerticalCount = 0;


            for (int x = 0; x < CELL_SIZE; x++)
            {
                if (Cell[x, y] == (int)CellState.Fill) HorizontalCount++;
                if (Cell[y, x] == (int)CellState.Fill) VerticalCount++;

                if (HorizontalCount == 10)
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

        //�ı�
        for (int x = 0; x < CELL_SIZE; x++)
        {
            for (int y = 0; y < CELL_SIZE; y++)
            {
                if (Cell[x, y] == (int)CellState.line)
                {
                    Cell[x, y] = (int)CellState.Empty;
                    StartCoroutine(BlockMove(GetCell(x, y), Vector3.zero, false, 0.2f, CellRecycling));
                }
            }
        }
        Score += line * 10 + shapeCount;
    }

    //������ ���� ��� ��ġ�Ǿ����� Ȯ��
    void CheckLogic()
    {
        int count = 0;
        int dieCheck = 0;
        for (int i = 0; i < blockSpawnPoints.Length; i++)
        {
            if (blockSpawnPoints[i].childCount != 0)
            {
                count++;
                if (PutUnable(blockSpawnPoints[i].GetComponentInChildren<Block>().shapePos)) dieCheck++;
            }
        }
        if (count == 0) SpawnBlock();
        else if (count == dieCheck) Die();
    }

    //��� ��ǥ�� �ش� ���� �� ��ǥ�� �ִ� �� ��ü������ Ȯ��
    bool PutUnable(Vector3[] shapePos)
    {
        int Check = 0;
        for (int y = 0; y < CELL_SIZE; y++)
        {
            for (int x = 0; x < CELL_SIZE; x++)
            {
                if (!UnableChecking(x, y, shapePos)) Check++;
            }
        }
        if (Check == 0) return true;
        else return false;
    }

    //���ڷ� ���� ��ǥ�� �ش� ���� �� �� �ִ� �� üũ
    bool UnableChecking(int x, int y, Vector3[] shapePos)
    {
        for (int i = 0; i < shapePos.Length; i++)
        {
            Vector3 SumPos = GetCell(x, y).transform.position + shapePos[i];

            if (OutCheck((int)SumPos.x, (int)SumPos.y)) return true;
            if (BlockCollision((int)SumPos.x, (int)SumPos.y)) return true;
        }
        return false;
    }

    //���� ���� ó��
    void Die()
    {
        print("Die");
        dieText.SetActive(true);
    }

    //�� ����
    void SpawnBlock()
    {
        for (int i = 0; i < blockSpawnPoints.Length; i++)
        {
            int Rand = Random.Range(0, blockObj.Length);

            Block block = Instantiate(blockObj[Rand],
                blockSpawnPoints[i].position + new Vector3(10, 0, 0), Quaternion.identity, blockSpawnPoints[i].transform);
            block.SetBlock();
            block.GetComponent<Block>().SetUP(blockSpawnPoints[i].position, 0.5f);
        }
        CheckLogic();
    }

    //Cell���� �迭�� ���������� ���
    void LoadCells()
    {
        int num = 0;
        foreach (Transform cell in borderFront)
        {
            cellObject[num] = cell.gameObject;
            num++;
        }
    }

    //�ε巯�� ������
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

    void CellRecycling(GameObject cell)
    {
        cell.transform.localScale = Vector3.one * 0.9f;
        cell.GetComponent<SpriteRenderer>().color = defaultCellColor;
    }
}
