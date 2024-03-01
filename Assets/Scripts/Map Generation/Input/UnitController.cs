using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.UI.CanvasScaler;
using static PlasticPipe.PlasticProtocol.Messages.NegotiationCommand;
using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEditor;

public class UnitController : MonoBehaviour
{
    private List<Unit> _selectedUnit;

    private Vector2 _selectorStartPosition;
    private Vector2 _selectorEndPosition;

    private LayerMask _layerMask;
    private Texture _defaultLUT;
    private ColorLookup _colorLookupEffect;
    private bool _isUsingDrone;
    private float _keyLockTime = .4f;
    private float _sinceLastLock = 0f;
    private float _defaultLUTContribution;

    [SerializeField]
    private bool ALLOW_ENEMY_CONTROL;
    [SerializeField]
    private Texture _heatSignatureTex;
    [SerializeField]
    private GameObject _droneInstance;
    [SerializeField]
    private GameObject _cameraRig;
    [SerializeField]
    private Volume _volume;
    [SerializeField]
    private GameObject _revealAllInstance;


    private void Awake()
    {
        _selectedUnit = new List<Unit>();
        _selectorStartPosition = Vector2.zero;
        _selectorEndPosition = Vector2.zero;
        _layerMask = LayerMask.GetMask("Obstacle") | LayerMask.GetMask("Character");

        Assert.IsNotNull(_heatSignatureTex, "Heat signature LUT is not assigned");
        Assert.IsNotNull(_droneInstance, "Drone Instance is not assigned");
        Assert.IsNotNull(_cameraRig, "Camera rig is not assigned");
        Assert.IsNotNull(_volume, "volume is not assigned");
        Assert.IsNotNull(_revealAllInstance, "Reveal all instance not set is not assigned");

        _volume.profile.TryGet<ColorLookup>(out _colorLookupEffect);
        _defaultLUT = _colorLookupEffect.texture.value;
        _droneInstance.SetActive(false);
        _revealAllInstance.SetActive(false);
        _defaultLUTContribution = _colorLookupEffect.contribution.value;
    }

    private void AddUnitToSelected(Unit unit)
    {
        if (_selectedUnit.Contains(unit)) return;

        _selectedUnit.Add(unit);
        unit.SetSelectionVisual(true);
        UIManager.Instance.EnableUISlot(unit.GetHashCode());
    }

    private void ClearSelected()
    {
        foreach (Unit unit in _selectedUnit)
        {
            unit.SetSelectionVisual(false);
            UIManager.Instance.DisableUISlot(unit.GetHashCode());
        }

        _selectedUnit.Clear();
    }

    private void BoxSelect()
    {
        Rect selectionRect = new Rect();

        if (Input.mousePosition.x < _selectorStartPosition.x)
        {
            selectionRect.xMin = Input.mousePosition.x;
            selectionRect.xMax = _selectorStartPosition.x;
        }
        else
        {
            selectionRect.xMin = _selectorStartPosition.x;
            selectionRect.xMax = Input.mousePosition.x;
        }

        if (Input.mousePosition.y < _selectorStartPosition.y)
        {
            selectionRect.yMin = Input.mousePosition.y;
            selectionRect.yMax = _selectorStartPosition.y;
        }
        else
        {
            selectionRect.yMin = _selectorStartPosition.y;
            selectionRect.yMax = Input.mousePosition.y;
        }

        ETeam playerTeam = GameManager.Instance.PlayerTeam;

        for (int i = 0; i < UnitManager.Instance.GetTeamSize(playerTeam); i++)
        {
            Unit unit = UnitManager.Instance.GetUnitAtIndex(i, playerTeam);
            Vector2 unitScreenPos = Camera.main.WorldToScreenPoint(unit.gameObject.transform.position);

            if (selectionRect.Contains(unitScreenPos))
            {
                AddUnitToSelected(unit);
            }
        }

        if (ALLOW_ENEMY_CONTROL)
        {
            for (int i = 0; i < UnitManager.Instance.GetTeamSize(ETeam.Red); i++)
            {
                Unit unit = UnitManager.Instance.GetUnitAtIndex(i, ETeam.Red);
                Vector2 unitScreenPos = Camera.main.WorldToScreenPoint(unit.gameObject.transform.position);

                if (selectionRect.Contains(unitScreenPos))
                {
                    AddUnitToSelected(unit);

                }
            }
        }
    }


    private List<GameObject> GetAvailableCommands(IClickable clickableObject)
    {
        List<GameObject> commandButtons = new List<GameObject>();

        List<Command> commands;
        if (_selectedUnit.Count == 1) commands = clickableObject.GetAvailableCommands(_selectedUnit[0]);
        else commands = clickableObject.GetAvailableCommands(_selectedUnit);

        Dictionary<System.Type, List<Command>> commandByType = new Dictionary<System.Type, List<Command>>();

        foreach (Command command in commands)
        {
            //sequencer commands should be excluded from grouping
            if (command.GetType() == typeof(SequencerCommand))
            {
                GameObject buttonInstance = UIManager.Instance.CreateCommandButton(command.ToUIString());
                Button button = buttonInstance.GetComponent<Button>();
                button.onClick.AddListener(() => command.Unit.ScheduleNormalCommand(command));
                commandButtons.Add(buttonInstance);
                continue;
            }

            if (!commandByType.ContainsKey(command.GetType())) commandByType.Add(command.GetType(), new List<Command>());

            commandByType[command.GetType()].Add(command);
        }

        foreach (KeyValuePair<System.Type, List<Command>> item in commandByType)
        {
            GameObject buttonInstance = UIManager.Instance.CreateCommandButton(item.Value[0].ToUIString());
            Button button = buttonInstance.GetComponent<Button>();

            foreach (Command command in item.Value)
            {
                if (item.Key == typeof(StopCommand)) button.onClick.AddListener(() => command.Unit.ScheduleHighCommand(command));
                else button.onClick.AddListener(() => command.Unit.ScheduleNormalCommand(command));
            }

            commandButtons.Add(buttonInstance);
        }

        return commandButtons;
    }

    private void ToggleDroneView()
    {
        if (_isUsingDrone)
        {
            _isUsingDrone = false;
            
            _colorLookupEffect.texture.value = _defaultLUT;

            Cursor.visible = true;
            _colorLookupEffect.contribution.value = _defaultLUTContribution;
            _revealAllInstance.SetActive(false);
            _droneInstance.SetActive(false);
        }
        else
        {
            _isUsingDrone = true;

            _colorLookupEffect.texture.value = _heatSignatureTex;
            _colorLookupEffect.contribution.value = 1f;
            Cursor.visible = false;
            _revealAllInstance.SetActive(true);
            _droneInstance.SetActive(true);
        }
    }

    

    private void Update()
    {
        if (_sinceLastLock < _keyLockTime + 1f) _sinceLastLock += Time.deltaTime;

        if (Input.GetKey(KeyCode.B) && _sinceLastLock >= _keyLockTime)
        {
            _sinceLastLock = 0;
            ToggleDroneView();
        }

        if (_isUsingDrone) return;

        foreach(Unit selected in _selectedUnit)
        {
            string queue = selected.gameObject.name + " Queue [";

            foreach (Command command in selected.BlackBoard.CommandQueue)
            {
                queue += command.ToUIString() + ", ";
            }
            queue += "]";

            DebugUiManager.Instance.AddDebugText(selected.GetHashCode() + 1, queue);
        }

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
                    if (!UIManager.Instance.IsCommandMenuOpen) ClearSelected();
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
        }

        if (Input.GetMouseButtonUp(0))
        {
            BoxSelect();
            _selectorStartPosition = Vector2.zero;
            _selectorEndPosition = Vector2.zero;
        }

        if (Input.GetMouseButtonDown(1))
        {
            UIManager.Instance.CloseCommandMenu();
            if (_selectedUnit.Count <= 0) return;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerMask))
            {
                IClickable clickableObject;
                if (!hit.collider.gameObject.TryGetComponent<IClickable>(out clickableObject)) return;

                UIManager.Instance.OpenCommandMenu(GetAvailableCommands(clickableObject), Input.mousePosition);
            }

        }
    }

    
}
