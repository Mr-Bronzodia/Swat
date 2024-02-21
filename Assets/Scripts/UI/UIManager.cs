using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Assertions;
//using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField]
    private RectTransform _unitDragSelectorRect;

    [SerializeField]
    private GameObject _buttonPrefab;

    [SerializeField]
    private Vector2 _buttonSize;

    [SerializeField]
    public GameObject _controlPanelParent;

    [SerializeField]
    private GameObject _selectedPanelPrefab;

    [SerializeField]
    private GameObject _commandIconPrefab;

    [SerializeField]
    private Canvas _mainCanvas;

    private RectTransform _controlParentRect;

    private Dictionary<int, SelectedPanel> _unitUISlots;
    private int _enabledSlotsCount = 0;

    public bool IsCommandMenuOpen {  get; private set; }

    private const float BUTTON_PADDING = 10f;
    private const float ENABLED_PADDING = 150f;

    private Vector2 _selectorStartPosition;
    private Vector2 _selectorEndPosition;

    private Vector2 _lastOpenPosition;

    private Vector2 _selectedRestPosition;

    // Start is called before the first frame update
    void Start()
    {
        _controlParentRect = _controlPanelParent.GetComponent<RectTransform>();
        _lastOpenPosition = Vector2.zero;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            _unitUISlots = new Dictionary<int, SelectedPanel>();

        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _selectorStartPosition = Input.mousePosition;
            //CloseCommandMenu();
        }

        if (Input.GetMouseButton(0))
        {
            _selectorEndPosition = Input.mousePosition;
            UpdateSelectorBoxVisual();
        }

        if (Input.GetMouseButtonUp(0))
        {
            _selectorStartPosition = Vector2.zero;
            _selectorEndPosition = Vector2.zero;
            UpdateSelectorBoxVisual();
        }

        if (!IsCommandMenuOpen) return;

        if (Vector2.Distance(_lastOpenPosition, Input.mousePosition) > 200f) CloseCommandMenu();
    }

    private void UpdateSelectorBoxVisual()
    {
        Vector2 boxCentre = (_selectorStartPosition + _selectorEndPosition) / 2;
        Vector2 boxSize = new Vector2(Mathf.Abs(_selectorStartPosition.x - _selectorEndPosition.x), Mathf.Abs(_selectorStartPosition.y - _selectorEndPosition.y));

        _unitDragSelectorRect.position = boxCentre;
        _unitDragSelectorRect.sizeDelta = boxSize;

    }

    public void EnableUISlot(int ID)
    {
        if (!_unitUISlots.ContainsKey(ID)) return;

        GameObject selectedInstance = _unitUISlots[ID].gameObject;

        RectTransform rectTransform = selectedInstance.GetComponent<RectTransform>();
        rectTransform.position = new Vector2(rectTransform.position.x, rectTransform.position.y + (_enabledSlotsCount * ENABLED_PADDING));
        _enabledSlotsCount++;

        selectedInstance.SetActive(true);
    }

    public void DisableUISlot(int ID)
    {
        if (!_unitUISlots.ContainsKey(ID)) return;

        GameObject selectedInstance = _unitUISlots[ID].gameObject;

        RectTransform rectTransform = selectedInstance.GetComponent<RectTransform>();
        rectTransform.position = _selectedRestPosition;
        _enabledSlotsCount--;

        selectedInstance.SetActive(false);
    }

    public void RegisterUISlot(int ID)
    {
        if (_unitUISlots.ContainsKey(ID)) return;

        GameObject selectionPanelInstance = Instantiate(_selectedPanelPrefab, _mainCanvas.transform);
        selectionPanelInstance.SetActive(false);
        _selectedRestPosition = selectionPanelInstance.GetComponent<RectTransform>().position;
        SelectedPanel selectedPanel = selectionPanelInstance.GetComponent<SelectedPanel>();


        _unitUISlots.Add(ID, selectedPanel);
    }

    public void UpdateUIHealth(int ID, float currentHealth, float maxHealth)
    {
        if (!_unitUISlots.ContainsKey(ID)) return;

        _unitUISlots[ID].HealthSlider.value = currentHealth / maxHealth;
    }

    public void UpdateAmmoUIAmmoCount(int ID, float maxAmmo, float currentAmmo)
    {
        if (!_unitUISlots.ContainsKey(ID)) return;

        _unitUISlots[ID].AmmoCountText.text = currentAmmo + "/" + maxAmmo;
    }

    public GameObject CreateCommandButton(string buttonText)
    {
        GameObject buttonInstance = Instantiate(_buttonPrefab, _controlPanelParent.transform);
        Button button = buttonInstance.GetComponent<Button>();
        RectTransform rectTransform = buttonInstance.GetComponent<RectTransform>();
        TMP_Text tmp = buttonInstance.GetComponentInChildren<TMP_Text>();
        buttonInstance.name = buttonText + " Button";


        rectTransform.position = _controlParentRect.position;
        rectTransform.sizeDelta = _buttonSize;

        tmp.text = buttonText;
        tmp.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmp.verticalAlignment = VerticalAlignmentOptions.Middle;
        tmp.fontSize = 12f;

        button.onClick.AddListener(() => CloseCommandMenu());

        return buttonInstance;
    }

    public void OpenCommandMenu(List<GameObject> buttons, Vector2 screenSpacePos)
    {
        if (IsCommandMenuOpen) return;
        if (buttons.Count == 0) return;

        _controlPanelParent.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
        _controlPanelParent.SetActive(true);
        _controlParentRect.sizeDelta = new Vector2(_buttonSize.x * 1.2f, (_buttonSize.y + BUTTON_PADDING) * buttons.Count);
        _controlParentRect.position = screenSpacePos - new Vector2(0, _controlParentRect.sizeDelta.y / 2);

        _lastOpenPosition = Input.mousePosition;
        IsCommandMenuOpen = true;
    }

    public void CloseCommandMenu()
    {
        if (!IsCommandMenuOpen) return;

        foreach (Transform child in _controlPanelParent.transform)
        {
            Destroy(child.gameObject);
        }

        IsCommandMenuOpen = false;
        _controlParentRect.sizeDelta = new Vector2(0f, 0f);
        _controlPanelParent.SetActive(false);
    }
}
