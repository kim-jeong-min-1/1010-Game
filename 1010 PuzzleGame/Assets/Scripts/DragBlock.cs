using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragBlock : MonoBehaviour
{
    [SerializeField] private AnimationCurve moveCurve;
    [SerializeField] private AnimationCurve scaleCurve;

    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float returnDuration = 0.1f;

    [field:SerializeField]
    public Vector2Int BlockCount { get; private set; }

    Coroutine sizeUp;
    Coroutine sizeDown;
    public void Setup(Vector3 position)
    {
        StartCoroutine(BlockTo(position, moveDuration, true));
    }

    #region 마우스 입력처리
    private void OnMouseDown()
    {
        if(sizeDown != null) StopCoroutine(sizeDown);
        sizeUp = StartCoroutine(BlockTo(Vector3.one, returnDuration, false));
    }

    private void OnMouseDrag()
    {
        Vector3 Front = new Vector3(0, (BlockCount.y / 2) + 1, 10); 
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Front;
    }

    private void OnMouseUp()
    {
        StopCoroutine(sizeUp);
        sizeDown = StartCoroutine(BlockTo(Vector3.one * 0.5f, returnDuration, false));

        StartCoroutine(BlockTo(transform.parent.position, returnDuration, true));
    }
    #endregion

    private IEnumerator BlockTo(Vector3 endPos, float time, bool ismove)
    {
        Vector3 start;
        start = (ismove) ? transform.position : transform.localScale;

        float current = 0;
        float percent = 0;

        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            if(ismove) transform.position = Vector3.Lerp(start, endPos, moveCurve.Evaluate(percent));
            else transform.localScale = Vector3.Lerp(start, endPos, scaleCurve.Evaluate(percent));

            yield return null;
        }
    }
}
