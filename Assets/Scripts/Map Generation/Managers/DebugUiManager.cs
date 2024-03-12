using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.PlayerLoop;

public class DebugUiManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _debugText;

    private Dictionary<int, string> _text;
    
    public static DebugUiManager Instance;

    private bool _RequiresRefresh = false;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        _text = new Dictionary<int, string>();
    }

    

    private void Refresh()
    {
        string text = "";
        foreach (KeyValuePair<int, string> pair in _text)
        {

            text += "[" + pair.Key.ToString() + "]" + " " + pair.Value + "\n";
        }

        _debugText.text = text;
        _RequiresRefresh = false;
    }

    public void AddDebugText(int id, string text)
    {
        if (!_text.ContainsKey(id)) _text.Add(id, text);
        else _text[id] = text;

        _RequiresRefresh = true;
    }

    private void Update()
    {
        if (_RequiresRefresh) Refresh();
        AddDebugText(GetHashCode(), "fps: " + (1f / Time.unscaledDeltaTime).ToString());
    }


}
