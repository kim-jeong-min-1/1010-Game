using System.Collections;
using UnityEngine;

namespace JM.Tweening
{
    public static class Tweening
    {
        public static void JMMove(this Transform transform, Vector3 endPos, float time)
        {
            Tween tween = new Tween(transform, endPos, time);
            tween.DoTween();
        }
    }

    public class Tween
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
            TweenStarter.Instance.StartCoroutine(DOTweening(targetTransform, endPosition, duration));
        }

        private IEnumerator DOTweening(Transform target, Vector3 endPos, float time)
        {
            Vector3 startPos = target.position;
            float current = 0;
            float percent = 0;

            while (percent < 1)
            {
                current += Time.deltaTime;
                percent = current / time;

                target.position = Vector3.Lerp(startPos, endPos, percent);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    public class TweenStarter : MonoBehaviour
    {
        private static MonoBehaviour instance;
        public static MonoBehaviour Instance
        {
            get
            {
                if (!instance)
                {
                    instance = new GameObject("[JMTween]").AddComponent<TweenStarter>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }

        public new static Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return Instance.StartCoroutine(coroutine);
        }
    }
}
