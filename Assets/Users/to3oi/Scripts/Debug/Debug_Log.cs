using System.Collections.Generic;
using TMPro;

public class Debug_Log : SingletonMonoBehaviour<Debug_Log>
{
    private static TextMeshProUGUI _out;
    private static int _index = 0;

    private List<string> message = new List<string>();

    public static void AddLog(string text)
    {
        Instance.UpdateLog(text);
    }

    private void UpdateLog(string text)
    {
        if (10 < _index)
        {
            message.RemoveAt(0);
        }

        message.Add(text + "\n");
        foreach (var m in message)
        {
            _out.text += m;
        }
        _index++;
    }
}