using System.Linq;
using InGameAsset.Scripts.Managers;
using InGameAsset.Scripts.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class PlayerInputEvent : MonoBehaviour
{
    public UnityEvent OnShoot;
    public UnityEvent<Vector2> OnMoveBody;
    public UnityEvent OnMoveTurret;
    [SerializeField] Button _fireButton;
    [SerializeField] TurretShootFromBarrelWithOwnUpdate _barrelWithOwnUpdateFromPlayer;
    [SerializeField] PlayerInput _playerInput;
    [SerializeField] CanvasGroup _touchCanvasGroup;
    [SerializeField] LevelManager _levelManager;
    [SerializeField] AutoTargetAimerTurret _autoTargetAimerTurret;
    [SerializeField] BoolGameData DisablePlayerControll;
    [SerializeField] bool isGameRunning;
    void Start()
    {
        _playerInput ??= GetComponent<PlayerInput>();
        _levelManager = FindObjectOfType<LevelManager>();
        _playerInput.uiInputModule = FindObjectOfType<UIManager>().GetComponent<InputSystemUIInputModule>();
        _playerInput.camera = Camera.main;
        _levelManager.OnGameStateChanged.AddListener(HandleGameStateChange);
        _autoTargetAimerTurret ??= GetComponentInChildren<AutoTargetAimerTurret>();
        FindObjectOfType<PlayerManager>().OnPlayerActivated.AddListener(()=>
        {
            Debug.Log("DetechInput is trigger");
            if (_playerInput.currentControlScheme == "Touch")
            {
                _touchCanvasGroup.alpha = 1;
                _touchCanvasGroup.interactable = true;
            }
            else
            {
                _touchCanvasGroup.alpha = 0;
                _touchCanvasGroup.interactable = false; 
            }
        });
    }

    void OnEnable()
    {
        InitializeFindAllTouchInput();
        if (_playerInput.currentControlScheme == "Touch")
        {
            _touchCanvasGroup.alpha = 1;
            _touchCanvasGroup.interactable = true;
        }
        else if (_touchCanvasGroup != null)
        {
            _touchCanvasGroup.alpha = 0;
            _touchCanvasGroup.interactable = false; 
        }
    }

    void HandleGameStateChange(LevelManager.GameState currentState, LevelManager.GameState previousState)
    {
        if (previousState == LevelManager.GameState.PreGame && currentState == LevelManager.GameState.Running)
        {
            InitializeFindAllTouchInput();
            isGameRunning = true;
        }
        if (currentState == LevelManager.GameState.Running && previousState == LevelManager.GameState.Pause)
        {
            isGameRunning = true;
        }

        if (currentState == LevelManager.GameState.Pause && previousState == LevelManager.GameState.Running)
        {
            isGameRunning = false;
        }
    }

    void InitializeFindAllTouchInput()
    {
        _touchCanvasGroup = FindObjectsOfType<CanvasGroup>().FirstOrDefault(i => i.CompareTag("TouchUI"));
    }

    public void OnControlsChanged()
    {
        if (_playerInput == null) return;
        //Debug.LogWarning($" player current Control Scheme {_playerInput.currentControlScheme}");
        if (_touchCanvasGroup == null) return;
        if (_playerInput.currentControlScheme == "Touch")
        {
            _touchCanvasGroup.alpha = 1;
            _touchCanvasGroup.interactable = true;
        }
        else
        {
            _touchCanvasGroup.alpha = 0;
            _touchCanvasGroup.interactable = false; 
        }
    }

    public void OnFire(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            Debug.Log("fire is press");
            if( !DisablePlayerControll.RunTimeValue)
                OnShoot?.Invoke();
        }
    }

    public void OnPress(InputValue inputValue)
    {
        if (inputValue.isPressed)
            Debug.Log(" TapInteraction is Trigger");
    }

    public void OnLook(InputValue inputValue)
    {
        var movementVector = inputValue.Get<Vector2>();
        if (movementVector != Vector2.zero && !DisablePlayerControll.RunTimeValue)
            OnMoveTurret.Invoke();
    }

    public void OnMouseLook(InputValue inputValue)
    {
        Vector2 mousePosition = inputValue.Get<Vector2>();
        if (mousePosition != Vector2.zero && !DisablePlayerControll.RunTimeValue)
            OnMoveTurret.Invoke();
    }

    public void OnMove(InputValue inputValue)
    {
        var movementVector = inputValue.Get<Vector2>();
        if( !DisablePlayerControll.RunTimeValue)
            OnMoveBody?.Invoke(movementVector);
        else
        {
            OnMoveBody?.Invoke(Vector2.zero);
        }
    }

    public void OnExit(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            _levelManager.TogglePause();
        }
    }

    public void OnReturn(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            //if(DisablePlayerControll.RunTimeValue) 
                _levelManager.HandleReturnButtonTrigger();
            //else if(!DisablePlayerControll.RunTimeValue)
            //{
                _autoTargetAimerTurret.TriggerAutoManualTrigger();
            //}
        }
    }
    public void OnFire(InputAction.CallbackContext context)
    {
        Debug.LogError("Not suppose to run");
        if (context.ReadValueAsButton())
        {
            OnShoot?.Invoke();
        }
    }

    public void OnTurretMove(InputAction.CallbackContext context)
    {
        Debug.LogError("Not suppose to run");
        Vector3 mouseDir = context.ReadValue<Vector2>();
        if(!DisablePlayerControll.RunTimeValue)
            OnMoveTurret.Invoke();
    }

    public void OnBodyMove(InputAction.CallbackContext context)
    {
        Debug.LogError("Not suppose to run");
        var movementVector = context.ReadValue<Vector2>();
        OnMoveBody?.Invoke(movementVector);
    }
}