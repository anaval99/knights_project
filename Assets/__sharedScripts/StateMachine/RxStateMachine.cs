using R3;
using UnityEngine;

public class RxStateMachine : MonoBehaviour
{
    private BehaviorSubject<IRxState> CurrentStateObs = new BehaviorSubject<IRxState>(null);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.CurrentStateObs.Where(state => state != null)
            .Select(state =>
            {
                Debug.Log("Playing: " + state.Name);
                return state.Play();
            })
            .Switch()
            .Subscribe()
            .AddTo(this);
    }
    
    public void SetState(IRxState newState)
    {
        this.CurrentStateObs.OnNext(newState);
    }
}
