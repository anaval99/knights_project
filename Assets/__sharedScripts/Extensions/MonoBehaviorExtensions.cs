using R3;
using Unity.VisualScripting;
using UnityEngine;

public static class MonoBehaviourExtensions
{
    public static CombatController GetCombatController(this MonoBehaviour mono)
    {
        var obj = GameObject.FindGameObjectWithTag("CombatController");
        if (obj != null)
        {
            var comp = obj.GetComponent<CombatController>();
            if (comp != null)
            {
                return comp;
            }
        }

        return null;
    }

    public static Observable<int> MoveTowards(this MonoBehaviour mono, Vector3 startingPosition, Vector3 targetPosition, float duration)
    {
        var completed = new Subject<bool>();
        float timeElapsed = 0;
        return Observable.EveryUpdate().Select(_ => 1)
            .Do(_ =>
            {
                if (timeElapsed < duration)
                {
                    var newPos = Vector3.Lerp(startingPosition, targetPosition, timeElapsed / duration);
                    mono.transform.position = newPos;
                }
                else
                {
                    completed.OnNext(true);
                    completed.OnCompleted();
                }
                timeElapsed += Time.deltaTime;
            })
            .TakeUntil(completed);
    }

    public static Observable<int> LookAtSmoothly(this MonoBehaviour mono, Vector3 targetWorldPosition, float duration)
    {
        Quaternion initialRotation = mono.transform.rotation;

        Vector3 flatTargetPosition = new Vector3(targetWorldPosition.x, mono.transform.position.y, targetWorldPosition.z);
        Vector3 directionToTarget = flatTargetPosition - mono.transform.position;

        if (directionToTarget.sqrMagnitude < 0.0001f)
        {
            return Observable.Return(1).Take(1);
        }

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

        float timeElapsed = 0;

        return Observable.EveryUpdate()
            .Select(_ => 1)
            .Do(_ =>
            {
                if (timeElapsed < duration)
                {
                    float t = timeElapsed / duration;
                    float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);

                    mono.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, easedT);
                    timeElapsed += Time.deltaTime;
                }
                else
                {
                    mono.transform.rotation = targetRotation;
                }
            })
            .TakeWhile(_ => timeElapsed < duration + float.Epsilon);
    }

    public static Observable<int> RotateSmoothly(this MonoBehaviour mono, Quaternion targetRotation, float duration)
    {
        Quaternion initialRotation = mono.transform.rotation;

        float timeElapsed = 0;

        return Observable.EveryUpdate()
            .Select(_ => 1)
            .Do(_ =>
            {
                if (timeElapsed < duration)
                {
                    float t = timeElapsed / duration;
                    float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);

                    mono.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, easedT);
                    timeElapsed += Time.deltaTime;
                }
                else
                {
                    mono.transform.rotation = targetRotation;
                }
            })
            .TakeWhile(_ => timeElapsed < duration + float.Epsilon);
    }
}