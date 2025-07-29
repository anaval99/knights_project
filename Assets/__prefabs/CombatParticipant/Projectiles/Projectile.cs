using R3;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    float Duration = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Observable<int> Fire(CombatParticipant Target, GameObject Template)
    {
        this.transform.LookAt(Target.transform);
        return this.MoveTowards(this.transform.position, Target.Collider.bounds.center, this.Duration)
            .Do(onNext: _ => {}, onCompleted: _ =>
            {
                GameObject.Destroy(Template);
                GameObject.Destroy(this.gameObject);
                Debug.Log(Template.name + " has reached target");
            });
    }
}
