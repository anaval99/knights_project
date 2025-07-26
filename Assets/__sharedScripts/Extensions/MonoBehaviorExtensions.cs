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
        var completed = new Subject<bool>();
        float timeElapsed = 0;
        return Observable.EveryUpdate().Select(_ => 1)
            .Do(_ =>
            {
                if (timeElapsed < duration)
                {

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
}