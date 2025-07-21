using R3;
using UnityEngine;

public class FriendlyCamp : MonoBehaviour
{
    [SerializeField]
    private CombatController combatController;
    [SerializeField]
    private float patrolSpeed = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.combatController.CombatStateObs
            .Where(state => state != null)
            .Select(state =>
            {
                if (state.Phase == CombatPhase.Patrolling)
                {
                    return MoveCampForPatrol();
                }

                return Observable.Empty<int>();

            })
            .Switch()
            .Subscribe()
            .AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    Observable<int> MoveCampForPatrol()
    {
        return Observable.EveryUpdate()
            .Select(_ => 1)
            .Do(_ => this.transform.Translate(Vector3.right * patrolSpeed * Time.deltaTime));
    }
}
