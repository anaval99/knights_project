using UnityEngine;

public static class ParserService
{
	public static string GetSeriesNumberFromGameObjectName(GameObject gameObject)
	{
		// e.g if GameObject name is "Object (0)", then the series number is "01"
		// e.g if GameObject name is "Object (1)", then the series number is "02"
		// e.g if GameObject name is "Object (09)", then the series number is "10"
		// e.g if GameObject name is "Object (10)", then the series number is "11"
		string goName = gameObject.name;
		if (goName.Contains("(") && goName.Contains(")"))
		{
			int startIndex = goName.IndexOf("(") + 1;
			int endIndex = goName.IndexOf(")");
			string extractedNum = goName.Substring(startIndex, endIndex - startIndex);
			int parsedNum = int.Parse(extractedNum) + 1; // 0 means hair1
			// ensure the number is two digits
			return parsedNum.ToString("D2");
		}
		else
		{
			Debug.LogError("GameObject name does not contain parentheses: " + goName);
			return null;
		}
	}
}
