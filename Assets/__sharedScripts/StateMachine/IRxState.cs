using R3;
using UnityEngine;

public interface IRxState
{
    string Name { get;  set; }
    Observable<int> Play();
}
