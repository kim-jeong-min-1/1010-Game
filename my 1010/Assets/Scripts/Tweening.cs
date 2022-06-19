using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JM.Tweening
{
    public static class Tweening
    {
        public static void DoMove(this Transform transform, Vector3 endPos, float time)
        {
            
        }
    }

    public class Tween : MonoBehaviour
    {
        AnimationCurve moveCurve;

        private Transform targetTransform;
        private Vector3 endPosition;
        private float duration;

        public Tween(Transform transform, Vector3 vector, float duration)
        {
            this.targetTransform = transform;
            this.endPosition = vector;
            this.duration = duration;
        }

        public void DoTween()
        {

            StartCoroutine(DOTweening(targetTransform, endPosition, duration));
        }

        private IEnumerator DOTweening(Transform target, Vector3 endPos, float time)
        {
            Vector3 startPos = target.position;
            float current = 0;
            float percent = 0;

            while(percent < 1)
            {
                current += Time.deltaTime;
                percent = current / time;

                target.position = Vector3.Lerp(startPos, endPos, moveCurve.Evaluate(percent));
                yield return null;
            }
        }
    }
}
