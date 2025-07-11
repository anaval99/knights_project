using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ListContainer : MonoBehaviour
{
  public void PopulateListItems<T>(List<T> childrenUI, int targetCount) where T : MonoBehaviour
  {
    if (childrenUI == null || childrenUI.Count == 0)
    {
      Debug.LogWarning("No ItemUI components assigned to ItemsContainer.");
      return;
    }

    while (childrenUI.Count < targetCount)
    {
      // add more items and then set parent to this.transform
      var listItem = Instantiate(childrenUI[0], this.transform);
      childrenUI.Add(listItem);
    }
  }

  public void RenderItems<T, D>(List<T> childrenUI, List<D> listData, int pageNumber, Action<T, D> renderFn) where T : MonoBehaviour
  {
    var pageSize = childrenUI.Count;
    var renderedData = listData.Skip(pageNumber * pageSize).Take(pageSize).ToList();
    for (int i = 0; i < childrenUI.Count; i++)
    {
      // check if renderedData has enough items
      if (i < renderedData.Count)
      {
        // Assuming T has a Render method that takes an item of type D
        renderFn(childrenUI[i], renderedData[i]);
      }
      else
      {
        // Clear the UI if no item is available
        renderFn(childrenUI[i], default);
      }
    }
    var totalPages = Mathf.CeilToInt((float)listData.Count / pageSize);
    Debug.Log($"Total pages: {totalPages}, Current page: {pageNumber}");
  }
}