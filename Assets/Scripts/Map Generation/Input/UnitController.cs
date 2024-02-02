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

    private Vector2 _selectorStartPosition;
    private Vector2 _selectorEndPosition;

    private void Awake()
    {
        _selectedUnit = new List<Unit>();
        _selectorStartPosition = Vector2.zero;
        _selectorEndPosition = Vector2.zero;

        if (_unitDragSelectorRect == null) Debug.LogError("Selector box not assigned in UnitController");
    }

    private void AddUnitToSelected(Unit unit)
    {
        if (_selectedUnit.Contains(unit)) return;

        _selectedUnit.Add(unit);
        unit.SetSelectionVisual(true);
    }

    private void RemoveUnitFromSelected(Unit unit)
    {
        if(!_selectedUnit.Contains(unit)) return;

        _selectedUnit.Remove(unit);
        unit.SetSelectionVisual(false);
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

    private void AddCommandButton(Unit unit, Command command, GameObject parent)
    {
        RectTransform parentRectTransform;
        if (!parent.TryGetComponent<RectTransform>(out parentRectTransform)) { Debug.Log("Cant Get parent rect transform"); return; }

        GameObject buttonInstance = new GameObject();
        buttonInstance.transform.parent = parent.transform;
        RectTransform buttonRectTransform = buttonInstance.AddComponent<RectTransform>();
        Button button = buttonInstance.AddComponent<Button>();
        button.onClick.AddListener(delegate { unit.BlackBoard.ScheduleNormalCommand(command); });
        buttonRectTransform.sizeDelta = new Vector2(parentRectTransform.sizeDelta.x * 0.8f, parentRectTransform.sizeDelta.y * .8f);
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
                if (!hit.collider.gameObject.TryGetComponent<Unit>(out unit)) { ClearSelected(); return; }

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

            if (_selectedUnit.Count <= 0) return;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                IClickable clickableObject;
                if (!hit.collider.gameObject.TryGetComponent<IClickable>(out clickableObject)) return;

                Unit unit = _selectedUnit[0];
                _controlPanelParent.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
                AddCommandButton(unit, clickableObject.GetAvailableCommands(unit)[0], _controlPanelParent);
                //Debug.Log("Hit Clickable object got command " + clickableObject.GetAvailableCommands(_selectedUnit[0])[0]);
                _controlPanelParent.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
            }

        }
    }

    
}
