using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class CommandIcon : MonoBehaviour
{
    private Image _commandIcon;
    [SerializeField]
    TMP_Text _commandName;

    public Image Icon { get => _commandIcon; }
    public TMP_Text CommandName { get => _commandName; }

    private void Awake()
    {
        _commandIcon = GetComponent<Image>();
    }

}
