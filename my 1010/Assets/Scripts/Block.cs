using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Vector3[] shapePos;
    private Vector3 blockPos;
    Coroutine sizeUp, sizeDown;

    public void SetBlock()
    {
        blockPos = transform.parent.position;
        shapePos = new Vector3[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
            shapePos[i] = transform.GetChild(i).localPosition;
    }

    //블럭 등장 애니메이션
    public void SetUP(Vector3 Position, float Time)
    {
        StartCoroutine(GameManager.Inst.BlockMove(this.gameObject, Position, true, Time));
    }

    #region 마우스 입력처리
    private void OnMouseDown()
    {
        if (sizeDown != null) StopCoroutine(sizeDown);
        sizeUp = StartCoroutine(GameManager.Inst.BlockMove(this.gameObject, Vector3.one, false, 0.2f));
        SortingOrder(10);
    }

    private void OnMouseDrag()
    {
        Vector3 front = new Vector3(0, 2, 10);
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + front;
    }

    private void OnMouseUp()
    {
        float x = Mathf.RoundToInt(transform.position.x);
        float y = Mathf.RoundToInt(transform.position.y);

        SortingOrder(1);
        transform.position = new Vector3(x, y, 0);

        GameManager.Inst.PutPuzzle(this, shapePos, (transform.parent.position + transform.localPosition));
    }
    #endregion

    //잘못된 위치에 놓았을 때 리턴
    public void BlockReturn()
    {
        StopCoroutine(sizeUp);
        sizeDown = StartCoroutine(GameManager.Inst.BlockMove(this.gameObject, Vector3.one * 0.5f, false, 0.2f));
        StartCoroutine(GameManager.Inst.BlockMove(this.gameObject, blockPos, true, 0.2f));
    }  
    
    //블럭삭제
    public void DeleteBlock()
    {
        transform.parent = null;
        Destroy(gameObject);
    }

    //드래그 중인 블럭 가장 위로
    private void SortingOrder(int order)
    {
        foreach(Transform child in transform)
        {
            child.GetComponent<SpriteRenderer>().sortingOrder = order;
        }
    }
}
