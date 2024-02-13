using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.UI.CanvasScaler;
using static PlasticPipe.PlasticProtocol.Messages.NegotiationCommand;

public class UnitController : MonoBehaviour
{
    private List<Unit> _selectedUnit;

    private Vector2 _selectorStartPosition;
    private Vector2 _selectorEndPosition;

    [SerializeField]
    private bool ALLOW_ENEMY_CONTROL;

    private void Awake()
    {
        _selectedUnit = new List<Unit>();
        _selectorStartPosition = Vector2.zero;
        _selectorEndPosition = Vector2.zero;
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

        if (ALLOW_ENEMY_CONTROL)
        {
            for (int i = 0; i < UnitManager.Instance.GetTeamSize(Team.Red); i++)
            {
                Unit unit = UnitManager.Instance.GetUnitAtIndex(i, Team.Red);
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

        //foreach (Command command in commands)
        //{
        //    GameObject buttonInstance = UIManager.Instance.CreateCommandButton(command.ToUIString());
        //    Button button = buttonInstance.GetComponent<Button>();


        //    button.onClick.AddListener(() => command.Unit.ScheduleNormalCommand(command));


        //    commandButtons.Add(buttonInstance);
        //}

        return commandButtons;
    }

    

    private void Update()
    {
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

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                IClickable clickableObject;
                if (!hit.collider.gameObject.TryGetComponent<IClickable>(out clickableObject)) return;

                UIManager.Instance.OpenCommandMenu(GetAvailableCommands(clickableObject), Input.mousePosition);
            }

        }
    }

    
}
