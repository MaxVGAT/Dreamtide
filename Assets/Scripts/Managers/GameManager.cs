using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveable
{
    public static GameManager instance;

    [Header("UI References")]
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject menuUI;

    [Header("Scene Management")]
    private bool isChangingScene = false;
    private Respawn_Type lastRespawnType = Respawn_Type.NonSpecific;
    private bool dataLoaded = false;
    private string lastScenePlayed;

    #region Initialization

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        Entity_Player.OnPlayerDeathFinished -= OnPlayerDeath;
        Entity_Player.OnPlayerDeathFinished += OnPlayerDeath;
    }

    private void OnDisable()
    {
        Entity_Player.OnPlayerDeathFinished -= OnPlayerDeath;
    }

    #endregion

    #region Scene Management

    public void ChangeScene(string sceneName, Respawn_Type respawnType)
    {
        if (isChangingScene || string.IsNullOrEmpty(sceneName))
            return;

        isChangingScene = true;
        lastRespawnType = respawnType;
        SaveProgress();
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        UI_Fade fade = FindFadeScreen();
        if (fade != null)
        {
            fade.DoFadeOut();
            while (fade.fadeEffectCo != null) yield return null;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
            yield return null;

        // Scene loaded, setup
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (instance != this) return;

        if (scene.name == "MainMenu")
            StartCoroutine(SetupMainMenu());
        else
            StartCoroutine(SetupGameScene());
    }

    private IEnumerator SetupMainMenu()
    {
        yield return null; // wait a frame

        isChangingScene = false;
        lastRespawnType = Respawn_Type.NonSpecific;

        inGameUI = GameObject.Find("InGameUI");
        menuUI = GameObject.Find("MenuUI");

        if (inGameUI != null) inGameUI.SetActive(false);
        if (menuUI != null) menuUI.SetActive(true);

        ShowHideSettings settings = FindFirstObjectByType<ShowHideSettings>();
        if (settings != null)
        {
            if (settings.mainMenuGroup == null) settings.AssignCanvasGroups();
            settings.HandleSettingsMainMenu();
        }

        SoundManager.instance?.StartBGM("MainMenu");

        UI_Fade fade = FindFadeScreen();
        if (fade != null)
        {
            fade.DoFadeIn();
            while (fade.fadeEffectCo != null) yield return null;
        }
    }

    private IEnumerator SetupGameScene()
    {
        while (!dataLoaded) yield return null;

        float timer = 0f;
        while ((Entity_Player.instance == null || Entity_Player.instance.GetComponent<Player_SkillManager>() == null) && timer < 5f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Entity_Player player = Entity_Player.instance;
        if (player == null)
        {
            Debug.LogError("[GameManager] Player not found in scene!");
            isChangingScene = false;
            yield break;
        }

        Vector3 target = player.transform.position;
        if (lastRespawnType != Respawn_Type.NonSpecific)
        {
            var wp = FindWaypoint(lastRespawnType);
            if (wp != null) target = wp.GetPositionAndSetTriggerFalse();
        }
        player.TeleportPlayer(target);

        yield return new WaitForSeconds(0.2f);

        UI ui = FindAnyObjectByType<UI>();
        if (ui != null)
        {
            bool menuWasOpen = ui.IsMenuOpen();
            if (!menuWasOpen) ui.ToggleUI();
            SaveManager.instance?.RefreshAndLoad();
            if (!menuWasOpen) ui.ToggleUI();
        }

        UI_Fade fade = FindFadeScreen();
        if (fade != null)
        {
            fade.DoFadeIn();
            while (fade.fadeEffectCo != null) yield return null;
        }

        isChangingScene = false;
    }

    private UI_Fade FindFadeScreen()
    {
        var fade = FindFirstObjectByType<UI_Fade>();
        if (fade == null)
        {
            GameObject go = GameObject.Find("FadeScreen");
            if (go != null) fade = go.GetComponent<UI_Fade>();
        }
        return fade;
    }

    #endregion

    #region Player Death

    private void OnPlayerDeath()
    {
        UI ui = FindFirstObjectByType<UI>();
        if (ui != null) ui.ToggleGameOverNoControls();
    }

    public void RespawnFromUI()
    {
        if (Entity_Player.instance == null) return;

        UI ui = FindFirstObjectByType<UI>();
        if (ui != null)
        {
            ui.ToggleGameOverNoControls();
            ui.ToggleUI();
        }

        SaveProgress();
        Debug.Log($"[Respawn] Player respawned at {Entity_Player.instance.transform.position}");
    }

    #endregion

    #region Save / Load

    public void SaveProgress()
    {
        var data = SaveManager.instance?.GetGameData();
        if (data == null) return;

        string scene = SceneManager.GetActiveScene().name;
        if (scene != "MainMenu") data.lastScenePlayed = scene;

        SaveManager.instance?.SaveGame();
    }

    public void LoadData(GameData data)
    {
        lastScenePlayed = string.IsNullOrEmpty(data.lastScenePlayed) ? "IntroScene" : data.lastScenePlayed;
        dataLoaded = true;
    }

    public void SaveData(ref GameData data)
    {
        string scene = SceneManager.GetActiveScene().name;
        if (scene == "MainMenu") return;
        data.lastScenePlayed = scene;
    }

    #endregion

    #region Waypoints

    private Object_Waypoint FindWaypoint(Respawn_Type type)
    {
        foreach (var wp in FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None))
        {
            if (wp.GetWaypointType() == type) return wp;
        }
        return null;
    }

    public Object_Waypoint GetWaypoint(Respawn_Type type) => FindWaypoint(type);

    #endregion

    #region Public Buttons

    public void GoMainMenuButton()
    {
        Debug.Log("[GameManager] GoMainMenuButton called - switching to MainMenu");
        ChangeScene("MainMenu", Respawn_Type.NonSpecific);
    }

    public void ContinuePlay()
    {
        string scene = SaveManager.instance?.GetGameData()?.lastScenePlayed ?? "IntroScene";
        ChangeScene(scene, Respawn_Type.NonSpecific);
    }

    public void RestartCurrentScene() => ChangeScene(SceneManager.GetActiveScene().name, Respawn_Type.NonSpecific);

    #endregion
}
