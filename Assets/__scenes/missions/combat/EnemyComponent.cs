using Animancer;
using UnityEngine;

[RequireComponent(typeof(RxStateMachine))]
public class EnemyComponent : MonoBehaviour
{
    [SerializeField]
    private AnimancerComponent animancer;
    [SerializeField]
    private AnimationClip idleAnimation;

    private RxStateMachine rxStateMachine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rxStateMachine = GetComponent<RxStateMachine>();
        this.PlayIdleAnimation();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void PlayIdleAnimation()
    {
        if (animancer == null || idleAnimation == null)
        {
            Debug.LogError("Animancer or Idle Animation is not set.");
            return;
        }
        var idleState = new EnemyIdleState(
            animancer,
            idleAnimation);
        rxStateMachine.SetState(idleState);
    }
}
