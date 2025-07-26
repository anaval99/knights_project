using System.Linq;
using R3;

public class CombineState : IRxState
{
    public string Name { get; set; } = "";
    private IRxState[] states;

    public CombineState(params IRxState[] states)
    {
        this.Name = string.Join('>', states.Select(x => x.Name));
        this.states = states;
    }

    public Observable<int> Play()
    {
        return Observable.Concat(this.states.Select(x => x.Play()));
    }
}