using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using UnityEngine.UI;

public class PagedList<T>
{
    public List<T> Data;
    public int CurrentPage;
    public int MaxPage;
}

public class ListPager : MonoBehaviour
{
    [SerializeField]
    Button PrevButton;
    [SerializeField]
    Button NextButton;
    [SerializeField]
    TMPro.TextMeshProUGUI Text;

    private int currentPage = 0;
    private int maxPage = -1;
    private BehaviorSubject<bool> emitterObs = new(true);

    void Start()
    {
        this.PrevButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (this.currentPage <= 0)
                {
                    return;
                }
                this.currentPage--;
                this.emitterObs.OnNext(true);
            }).AddTo(this);
        this.NextButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (this.currentPage >= this.maxPage)
                {
                    return;
                }
                this.currentPage++;
                this.emitterObs.OnNext(true);
            }).AddTo(this);            
    }

    public Observable<PagedList<T>> Feed<T>(Observable<List<T>> source, int itemsPerPage)
    {
        return Observable
            .CombineLatest(this.emitterObs, source, (_, data) => data)
            .Where(data => data != null)
            .Select(data =>
            {
                this.maxPage = (int)Math.Ceiling((double)data.Count / itemsPerPage) - 1;
                this.currentPage = Math.Min(this.currentPage, this.maxPage);

                var startIndex = this.currentPage * itemsPerPage;
                var displayedData = data.Skip(startIndex).Take(itemsPerPage);
                this.Text.SetText($"Page {this.currentPage + 1}/{this.maxPage + 1}");
                return new PagedList<T>
                {
                    Data = displayedData.ToList(),
                    CurrentPage = this.currentPage,
                    MaxPage = this.maxPage,
                };
            });
    }
}