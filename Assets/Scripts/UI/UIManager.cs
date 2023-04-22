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
    [SerializeField] private StartView m_startView;
    [SerializeField] private LoginView m_loginView;
    [SerializeField] private AccountView m_accountView;
    [SerializeField] private MainView m_mainView;
    [SerializeField] private PauseView m_pauseView;
    [SerializeField] private OptionsView m_optionsView;
    [SerializeField] private LevelUpView m_levelUpView;
    [SerializeField] private DeathView m_deathView;

    [SerializeField] private SceneData m_titleScene;
    [SerializeField] private SceneData m_gameScene;
    [SerializeField] private SceneData m_endScene;

    private UIView m_previousView;

    private MenuController m_controller;
    public MenuController Controller => m_controller;

    public TitleView TitleView => m_titleView;
    public StartView StartView => m_startView;
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

    public void TestRun(bool _test)
    {
        TestManager.Instance.IsTesting = _test;
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

    public void SwitchToTitleScreen()
    {
        SwitchView(m_titleView);
    }

    public void SwitchToStartView()
    {
        SwitchView(m_startView);
    }

    public Selectable GetCurrentSelectable()
    {
        return m_currentSelectable;
    }

    public void SetCurrentSelectable(Selectable _newSelectable)
    {
        m_currentSelectable = _newSelectable;
    }

    public void BackToTitle()
    {
        if (LoginManager.Instance.IsLoggedIn(out _))
        {
            if (Player.Instance.TimeSurvived > SaveManager.Instance.SaveFile.bestTime)
            {
                SaveManager.Instance.SaveFile.bestTime = Player.Instance.TimeSurvived;
            }

            SaveManager.Instance.SaveGame(GoToTitleScreen);
            return;
        }

        GoToTitleScreen();
    }

    public void GoToTitleScreen()
    {
        Addressables.LoadAssetsAsync<Object>(new List<string> { m_titleScene.SceneName },
            _x => { }, Addressables.MergeMode.Union).Completed += LoadTitleScreen;
    }

    private void LoadTitleScreen(AsyncOperationHandle<IList<Object>> _object)
    {
        Addressables.LoadSceneAsync(m_titleScene.SceneName, LoadSceneMode.Additive).Completed += StartTitleScene;
    }

    private void StartTitleScene(AsyncOperationHandle<SceneInstance> _scene)
    {
        if (GameManager.Instance.currentScene != default)
        {
            SceneManager.UnloadSceneAsync(GameManager.Instance.currentScene).completed += ClearPools;
        }

        GameManager.Instance.currentScene = SceneManager.GetSceneByName(m_titleScene.SceneName);
        SwitchView(m_mainView);
        SwitchView(m_titleView);
        InputSystem.Instance.ClearHandlers();
        InputSystem.Instance.AddHandler(Controller);
        AudioManager.Instance.StartMusic();
        GameManager.Instance.UnpauseGame();
    }

    private void ClearPools(AsyncOperation _operation)
    {
        PoolManager.Instance.ResetPools();
    }
    
    public void GoToEndScreen()
    {
        AudioManager.Instance.StopMusic();
        Addressables.LoadAssetsAsync<Object>(new List<string> { m_endScene.SceneName },
            _x => { }, Addressables.MergeMode.Union).Completed += LoadEndScreen;
    }

    private void LoadEndScreen(AsyncOperationHandle<IList<Object>> _object)
    {
        Addressables.LoadSceneAsync(m_endScene.SceneName, LoadSceneMode.Additive).Completed += StartEndScene;
    }

    private void StartEndScene(AsyncOperationHandle<SceneInstance> _scene)
    {
        if (GameManager.Instance.currentScene != default)
        {
            SceneManager.UnloadSceneAsync(GameManager.Instance.currentScene).completed += ClearPools;
        }

        GameManager.Instance.currentScene = SceneManager.GetSceneByName(m_endScene.SceneName);
        InputSystem.Instance.ClearHandlers();
        GameManager.Instance.UnpauseGame();
    }

    public void GoToGameScreen()
    {
        Addressables.LoadAssetsAsync<Object>(new List<string> { m_gameScene.SceneName },
            _x => { }, Addressables.MergeMode.Union).Completed += LoadGameScene;
    }

    private void LoadGameScene(AsyncOperationHandle<IList<Object>> _object)
    {
        Addressables.LoadSceneAsync(m_gameScene.SceneName, LoadSceneMode.Additive).Completed += StartGameScene;
    }

    private void StartGameScene(AsyncOperationHandle<SceneInstance> _scene)
    {
        if (GameManager.Instance.currentScene != default)
        {
            SceneManager.UnloadSceneAsync(GameManager.Instance.currentScene);
        }

        GameManager.Instance.currentScene = SceneManager.GetSceneByName(m_gameScene.SceneName);
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
        ReturnToPreviousView();
    }

    public void OpenLoginAccountView()
    {
        if (LoginManager.Instance.IsLoggedIn(out var userName))
        {
            m_accountView.PreviousView = m_currentView;
            SwitchView(m_accountView);
        }
        else
        {
            m_loginView.PreviousView = m_currentView;
            SwitchView(m_loginView);
        }
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
        if (LoginManager.Instance.IsLoggedIn(out _))
        {
            if (Player.Instance.TimeSurvived > SaveManager.Instance.SaveFile.bestTime)
            {
                SaveManager.Instance.SaveFile.bestTime = Player.Instance.TimeSurvived;
            }

            SaveManager.Instance.SaveGame(Exit);
            return;
        }

        Exit();
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}