using System.Linq;
using R3;

public class CombineState : IRxState
{
    public string Name { get; set; } = "";
    public bool Merge { get; set; } = false;
    private IRxState[] states;

    public CombineState(params IRxState[] states)
    {
        this.Merge = false;
        this.Name = $"({string.Join(">", states.Select(x => x.Name))})";
        this.states = states;
    }

    public CombineState(bool merge, params IRxState[] states)
    {
        this.Merge = merge;
        this.Name = $"({string.Join("+", states.Select(x => x.Name))})";
        this.states = states;
    }    

    public Observable<int> Play()
    {
        return !this.Merge ? Observable.Concat(this.states.Select(x => x.Play()))
            : Observable.Merge(this.states.Select(x => x.Play()));
    }
}