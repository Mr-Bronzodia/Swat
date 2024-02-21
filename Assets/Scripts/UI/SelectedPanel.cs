using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectedPanel : MonoBehaviour
{
    [SerializeField]
    private Slider _healthSlider;
    [SerializeField]
    private TMP_Text _ammoCountText;
    [SerializeField]
    private GameObject _commandQueueParent;

    public Slider HealthSlider { get => _healthSlider; }
    public TMP_Text AmmoCountText { get => _ammoCountText; }
    public GameObject CommandQueueParent { get => _commandQueueParent; }

}
