using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.UI.CanvasScaler;

public class UnitController : MonoBehaviour
{
    private List<Unit> _selectedUnit;

    [SerializeField]
    private RectTransform _unitDragSelectorRect;

    [SerializeField]
    private GameObject _controlPanelParent;
    private RectTransform _controlParentRect;

    [SerializeField]
    private GameObject _buttonPrefab;

    private Vector2 _selectorStartPosition;
    private Vector2 _selectorEndPosition;

    private bool _isCommandMenuOpen;
    private const float BUTTON_PADDING = 20f;

    [SerializeField]
    private Vector2 _buttonSize;

    private void Awake()
    {
        _selectedUnit = new List<Unit>();
        _selectorStartPosition = Vector2.zero;
        _selectorEndPosition = Vector2.zero;
        _isCommandMenuOpen = false;

        if (_unitDragSelectorRect == null) Debug.LogError("Selector box not assigned in UnitController");
        _controlParentRect = _controlPanelParent.GetComponent<RectTransform>();
    }

    private void AddUnitToSelected(Unit unit)
    {
        if (_selectedUnit.Contains(unit)) return;

        _selectedUnit.Add(unit);
        unit.SetSelectionVisual(true);
    }

    private void ClearSelected()
    {
        foreach (Unit unit in _selectedUnit)
        {
            unit.SetSelectionVisual(false);
        }

        _selectedUnit.Clear();
    }

    private void UpdateSelectorBoxVisual()
    {
        Vector2 boxCentre = (_selectorStartPosition + _selectorEndPosition) / 2;
        Vector2 boxSize = new Vector2(Mathf.Abs(_selectorStartPosition.x - _selectorEndPosition.x),Mathf.Abs( _selectorStartPosition.y - _selectorEndPosition.y));

        _unitDragSelectorRect.position = boxCentre;
        _unitDragSelectorRect.sizeDelta = boxSize;

    }

    private void AddCommandButton(Unit unit, Command command, GameObject parent, Vector2 pos)
    {
        GameObject buttonInstance = Instantiate(_buttonPrefab, parent.transform);
        Button button = buttonInstance.GetComponent<Button>();
        RectTransform rectTransform = buttonInstance.GetComponent<RectTransform>();
        TMP_Text tmp = buttonInstance.GetComponentInChildren<TMP_Text>();
        buttonInstance.name = command.ToString() + " Button";


        rectTransform.position = pos;
        rectTransform.sizeDelta = _buttonSize;

        tmp.text = command.ToUIString();
        tmp.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmp.verticalAlignment = VerticalAlignmentOptions.Middle;
        tmp.fontSize = 12f;

        button.onClick.AddListener(() => unit.BlackBoard.ScheduleNormalCommand(command));
        button.onClick.AddListener(() => CloseCommandMenu());
    }

    private void OpenCommandMenu(List<Command> commands, Vector2 screenSpacePos)
    {
        if (_isCommandMenuOpen) return;

        _controlPanelParent.SetActive(true);
        _controlParentRect.sizeDelta = new Vector2(_buttonSize.x * 1.2f, (_buttonSize.y + BUTTON_PADDING) * commands.Count);
        _controlParentRect.position = screenSpacePos - new Vector2(0, _controlParentRect.sizeDelta.y / 2);

        for (int i = 0; i < commands.Count; i++)
        {
            Vector2 buttonPos = new Vector2(_controlParentRect.position.x , _controlParentRect.position.y - (i * _buttonSize.y));
            AddCommandButton(_selectedUnit[0], commands[i], _controlPanelParent, buttonPos);
        }

        _isCommandMenuOpen = true;
    }

    private void CloseCommandMenu()
    {

        if (!_isCommandMenuOpen) return;

        foreach(Transform child in _controlPanelParent.transform)
        {
            Destroy(child.gameObject);
        }

        _isCommandMenuOpen = false;
        _controlParentRect.sizeDelta = new Vector2(0f, 0f);
        _controlPanelParent.SetActive(false);
    }

    private void BoxSelect()
    {
        if (_unitDragSelectorRect.sizeDelta.magnitude <= float.Epsilon ) return;

        Rect selectionRect = new Rect();

        if (Input.mousePosition.x < _selectorStartPosition.x )
        {
            selectionRect.xMin = Input.mousePosition.x;
            selectionRect.xMax = _selectorStartPosition.x;
        }
        else
        {
            selectionRect.xMin = _selectorStartPosition.x;
            selectionRect.xMax = Input.mousePosition.x;
        }

        if (Input.mousePosition.y < _selectorStartPosition.y )
        {
            selectionRect.yMin = Input.mousePosition.y;
            selectionRect.yMax = _selectorStartPosition.y;
        }
        else
        {
            selectionRect.yMin = _selectorStartPosition.y;
            selectionRect.yMax = Input.mousePosition.y;
        }

        Team playerTeam = GameManager.Instance.PlayerTeam;

        for (int i = 0; i < UnitManager.Instance.GetTeamSize(playerTeam); i++)
        {
            Unit unit = UnitManager.Instance.GetUnitAtIndex(i, playerTeam);
            Vector2 unitScreenPos = Camera.main.WorldToScreenPoint(unit.gameObject.transform.position);
            
            if (selectionRect.Contains(unitScreenPos))
            {
                AddUnitToSelected(unit);
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _selectorStartPosition = Input.mousePosition;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray,out hit, Mathf.Infinity))
            {
                Unit unit;
                if (!hit.collider.gameObject.TryGetComponent<Unit>(out unit)) 
                { 
                    if (!_isCommandMenuOpen) ClearSelected();
                    return;
                }

                if (unit.BlackBoard.Team != GameManager.Instance.PlayerTeam) return;

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    AddUnitToSelected(unit);
                }
                else
                {
                    ClearSelected();
                    AddUnitToSelected(unit);
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            _selectorEndPosition = Input.mousePosition;
            UpdateSelectorBoxVisual();
        }

        if (Input.GetMouseButtonUp(0))
        {
            BoxSelect();
            _selectorStartPosition = Vector2.zero;
            _selectorEndPosition = Vector2.zero;
            UpdateSelectorBoxVisual();
        }

        if (Input.GetMouseButtonDown(1))
        {         
            CloseCommandMenu();

            if (_selectedUnit.Count <= 0) return;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                IClickable clickableObject;
                if (!hit.collider.gameObject.TryGetComponent<IClickable>(out clickableObject)) return;

                Unit unit = _selectedUnit[0];
                _controlPanelParent.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
                OpenCommandMenu(clickableObject.GetAvailableCommands(unit), Input.mousePosition);

            }

        }
    }

    
}
