using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            m_controller = GetComponent<MenuController>();
            return;
        }

        Destroy(gameObject);
    }

    private Selectable m_currentSelectable;
    private UIView m_currentView;

    [SerializeField] private bool m_isTitleScreen;

    [Header("Views")] [SerializeField] private TitleView m_titleView;
    [SerializeField] private LoginView m_loginView;
    [SerializeField] private AccountView m_accountView;
    [SerializeField] private MainView m_mainView;
    [SerializeField] private PauseView m_pauseView;
    [SerializeField] private OptionsView m_optionsView;
    [SerializeField] private LevelUpView m_levelUpView;
    [SerializeField] private DeathView m_deathView;

    [SerializeField] private SceneData m_titleScene;
    [SerializeField] private SceneData m_gameScene;

    [SerializeField] private string titleSceneName;
    [SerializeField] private string gameSceneName;

    private UIView m_previousView;

    private MenuController m_controller;
    public MenuController Controller => m_controller;

    public TitleView TitleView => m_titleView;
    public LoginView LoginView => m_loginView;
    public AccountView AccountView => m_accountView;
    public MainView MainView => m_mainView;
    public PauseView PauseView => m_pauseView;
    public OptionsView OptionsView => m_optionsView;
    public LevelUpView LevelUpView => m_levelUpView;
    public DeathView DeathView => m_deathView;

    public UIView PreviousView
    {
        get => m_previousView;
        set => m_previousView = value;
    }

    public bool IsTitleScreen
    {
        get => m_isTitleScreen;
        set => m_isTitleScreen = value;
    }

    private void Start()
    {
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForEndOfFrame();
        SwitchView(m_titleView);
        GoToTitleScreen();

        // if (m_isTitleScreen)
        // {
        //     SwitchView(m_titleView);
        //     InputSystem.Instance.AddHandler(Controller);
        // }
        // else
        // {
        //     SwitchView(m_mainView);
        //     InputSystem.Instance.AddHandler(Player.Instance.Controller);
        // }
    }

    public void SwitchView(UIView _newView = null, bool _additive = false)
    {
        if (_newView != null)
        {
            m_previousView = _newView.PreviousView;
        }

        if (!_additive)
        {
            if (m_currentView)
            {
                m_currentView.OnHide();
            }
        }

        m_currentView = _newView;
        if (m_currentView != null)
        {
            m_currentView.OnShow();
        }
    }

    public Selectable GetCurrentSelectable()
    {
        return m_currentSelectable;
    }

    public void SetCurrentSelectable(Selectable _newSelectable)
    {
        m_currentSelectable = _newSelectable;
    }

    public void GoToTitleScreen()
    {
        Addressables.LoadAssetsAsync<Object>(new List<string> { "TitleScene" },
            _x => { }, Addressables.MergeMode.Union).Completed += LoadTitleScreen;
    }

    private void LoadTitleScreen(AsyncOperationHandle<IList<Object>> _object)
    {
        Addressables.LoadSceneAsync(titleSceneName, LoadSceneMode.Additive).Completed += StartTitleScene;
    }
    
    private void StartTitleScene(AsyncOperationHandle<SceneInstance> _scene)
    {
        if (GameManager.Instance.currentScene != default)
        {
            SceneManager.UnloadSceneAsync(GameManager.Instance.currentScene).completed += ClearPools;
        }
        
        GameManager.Instance.currentScene = SceneManager.GetSceneByName(titleSceneName);
        SwitchView(m_mainView);
        SwitchView(m_titleView);
        InputSystem.Instance.ClearHandlers();
        InputSystem.Instance.AddHandler(Controller);
        GameManager.Instance.UnpauseGame();
    }

    private void ClearPools(AsyncOperation _operation)
    {
        PoolManager.Instance.ResetPools();
    }

    public void GoToGameScreen()
    {
        Addressables.LoadAssetsAsync<Object>(new List<string> { "GameScene" },
            _x => { }, Addressables.MergeMode.Union).Completed += LoadGameScene;
    }

    private void LoadGameScene(AsyncOperationHandle<IList<Object>> _object)
    {
        Addressables.LoadSceneAsync(gameSceneName, LoadSceneMode.Additive).Completed += StartGameScene;
    }

    private void StartGameScene(AsyncOperationHandle<SceneInstance> _scene)
    {
        if (GameManager.Instance.currentScene != default)
        {
            SceneManager.UnloadSceneAsync(GameManager.Instance.currentScene);
        }
        
        GameManager.Instance.currentScene = SceneManager.GetSceneByName(gameSceneName);
        SwitchView(m_mainView);
        InputSystem.Instance.ClearHandlers();
        InputSystem.Instance.AddHandler(Player.Instance.Controller);
    }

    public void TogglePauseView()
    {
        if (m_currentView == m_levelUpView || m_currentView == m_deathView) return;

        GameManager.Instance.PauseGame();
        if (m_currentView == m_mainView)
        {
            m_pauseView.PreviousView = m_currentView;
            SwitchView(m_pauseView, true);
        }
        else
        {
            m_currentView.CurrentSelectable = null;
            SwitchView(m_mainView);
        }
    }

    public void ToggleLevelUpView()
    {
        GameManager.Instance.PauseGame();
        if (m_currentView == m_mainView)
        {
            m_levelUpView.PreviousView = m_currentView;
            SwitchView(m_levelUpView, true);
        }
        else
        {
            m_currentView.PreviousView = null;
            SwitchView(m_mainView);
        }
    }

    public void ToggleDeathView()
    {
        if (m_currentView == m_mainView)
        {
            m_deathView.PreviousView = m_currentView;
            SwitchView(m_deathView, true);
        }
        else
        {
            m_currentView.PreviousView = null;
            SwitchView(m_mainView);
        }
    }

    public void OpenOptionsView()
    {
        m_optionsView.PreviousView = m_currentView;
        SwitchView(m_optionsView);
    }

    public void ReturnFromOptionsView()
    {
        m_optionsView.SaveOptions();
        ReturnToPreviousView();
    }

    public void OpenLoginView()
    {
        m_loginView.PreviousView = m_currentView;
        SwitchView(m_loginView);
    }

    public void ReturnToPreviousView()
    {
        m_currentView.CurrentSelectable = null;
        SwitchView(m_previousView);
    }

    public void UpdateExperience()
    {
        m_mainView.UpdateExperience();
    }

    public void ReturnToDesktop()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}