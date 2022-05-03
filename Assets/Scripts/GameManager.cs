using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    protected PlayerData playerData;

    // Makes Play Game button on the main menu scene interactable/non-interactable based on if user made character selections
    private bool isInteractable = false;

    // Player selection variables
    private InputField inputCharName;
    private Image bowSprite;
    private Slider chosenDifficulty;
    private Text chosenDifficultyText;
    private TMP_Text saveConfirmation;

    // You can choose easy, medium or hard difficulty
    private string[] difficultyLevels = { "Easy", "Medium", "Hard" };
    
    // You can choose between these colors for your bow
    private Color[] colors = { Color.cyan, Color.blue, Color.green, Color.red, Color.yellow, Color.white, Color.magenta };
    
    // Used to keep track of which color option we are on when the user clicks the right/left button in player selection
    private int colorCount = 0;

    // Initially call the main menu scene function
    void Start()
    {
        MainMenuScene();

        // Keeps track of player selections and high score
        playerData = new PlayerData();
    }

    // Implements the singleton pattern
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // OnEnable, OnDisable, and OnLevelLoaded used to call our selected scene function when the scene is loaded
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoaded;
    }
    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "PlayGameScene")
        {
            PlayGameScene();
        }
        else if (scene.name == "PlayerSelectionScene")
        {
            PlayerSelectionScene();
        }
        else if (scene.name == "InstructionsScene")
        {
            InstructionsScene();
        }
        else if (scene.name == "MainMenuScene")
        {
            MainMenuScene();
        }
        else if (scene.name == "CreditsScene")
        {
            CreditsScene();
        }
    }

    // Load a certain scenes assets
    private void LoadSceneByNum(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }

    private void MainMenuScene()
    {
        var mainMenuPlayGameButton = GameObject.Find("Button_MainMenu_PlayGame").GetComponent<Button>();
        var mainMenuPlayerSelectionButton = GameObject.Find("Button_MainMenu_PlayerSelection").GetComponent<Button>();
        var mainMenuInstructionsButton = GameObject.Find("Button_MainMenu_Instructions").GetComponent<Button>();
        var mainMenuCreditsButton = GameObject.Find("Button_MainMenu_Credits").GetComponent<Button>();
        var mainMenuExitButton = GameObject.Find("Button_MainMenu_Exit").GetComponent<Button>();

        // Wait to see what button the user selects and load that buttons scene or exit if exit is pressed
        mainMenuPlayGameButton.onClick.AddListener(delegate { LoadSceneByNum(3); });
        mainMenuPlayerSelectionButton.onClick.AddListener(delegate { LoadSceneByNum(2); });
        mainMenuInstructionsButton.onClick.AddListener(delegate { LoadSceneByNum(1); });
        mainMenuCreditsButton.onClick.AddListener(delegate { LoadSceneByNum(4); });
        mainMenuExitButton.onClick.AddListener(delegate { OnExitGame(); });

        // If the user has saved his choices in the Player Selection scene, they can use the Play Game button
        mainMenuPlayGameButton.interactable = isInteractable;
    }

    private void PlayerSelectionScene()
    {
        // Wait for the user to press the back button and load the main menu scene when they do
        var selectionBackButton = GameObject.Find("Button_PlayerSelection_Back").GetComponent<Button>();
        selectionBackButton.onClick.AddListener(delegate { LoadSceneByNum(0); });

        // Wait for the user to press the Save Selections button when they are done selecting their choices
        var saveGame = GameObject.Find("Button_PlayerSelection_SaveSelections").GetComponent<Button>();
        saveConfirmation = GameObject.Find("Text_PlayerSelection_SaveConfirmation").GetComponent<TMP_Text>();
        saveGame.onClick.AddListener(delegate { OnSaveGame(); });
        saveGame.onClick.AddListener(delegate { SetSaveText(); });

        // Update our player name selection when the user types in a name and remember it between scenes
        inputCharName = GameObject.Find("InputField_PlayerSelection_Name").GetComponent<InputField>();
        inputCharName.text = playerData.characterName;
        inputCharName.onEndEdit.AddListener(delegate { OnEndEditName(); });

        // Update our players chosen difficulty level using our slider and display the difficulty in a text field (easy, medium, or hard)
        chosenDifficulty = GameObject.Find("Slider_PlayerSelection_Difficulty").GetComponent<Slider>();
        chosenDifficultyText = GameObject.Find("Text_PlayerSelection_Difficulty").GetComponent<Text>();
        chosenDifficultyText.text = difficultyLevels[playerData.difficulty];
        chosenDifficulty.value = playerData.difficulty;
        chosenDifficulty.onValueChanged.AddListener(delegate { OnChangeDifficultyLevel(); });

        // Update our players bow color selection when the user clicks the left/right button and remember it
        bowSprite = GameObject.Find("Image_PlayerSelection_Bow").GetComponent<Image>();
        bowSprite.color = colors[colorCount];
        var rightColor = GameObject.Find("Button_PlayerSelection_BowColor_Right").GetComponent<Button>();
        var leftColor = GameObject.Find("Button_PlayerSelection_BowColor_Left").GetComponent<Button>();
        rightColor.onClick.AddListener(delegate { SwitchColor(++colorCount); });
        leftColor.onClick.AddListener(delegate { SwitchColor(--colorCount); });
    }

    private void InstructionsScene()
    {
        // Wait for the user to press the back button and load the main menu scene when they do
        var instructionsBackButton = GameObject.Find("Button_Instructions_Back").GetComponent<Button>();
        instructionsBackButton.onClick.AddListener(delegate { LoadSceneByNum(0); });
    }

    private void CreditsScene()
    {
        // Wait for the user to press the back button and load the main menu scene when they do
        var creditsBackButton = GameObject.Find("Button_Credits_Back").GetComponent<Button>();
        creditsBackButton.onClick.AddListener(delegate { LoadSceneByNum(0); });
    }

    private void PlayGameScene()
    {
        // Set our timer difficulty variable in the Timer class based on the users chosen difficulty (60s, 120s, 180s)
        var timer = GameObject.Find("CountdownTimer").GetComponent(typeof(CountdownTimer)) as CountdownTimer;
        if (playerData.difficulty == 0)
        {
            timer.countdownTime = 180;
        }
        else if (playerData.difficulty == 1)
        {
            timer.countdownTime = 120;
        }
        else
        {
            timer.countdownTime = 10;
        }

        // Keep track of high scores so we can display the high scores even if they leave the scene and come back
        var playerScore = GameObject.Find("PlayerScore").GetComponent(typeof(PlayerScore)) as PlayerScore;
        playerScore.SetBestTime(playerData.bestTime);

        // Wait for the user to press the back button, if so set their high score in the player data object, then load main menu
        var playBackButton = GameObject.Find("Button_PlayGame_Back").GetComponent<Button>();
        playBackButton.onClick.AddListener(delegate { playerData.bestTime = playerScore.GetBestTime(); LoadSceneByNum(0); });

        // Change the bow color based on the user's choice
        var bow = GameObject.Find("Bow").GetComponent<Renderer>();
        bow.material.color = playerData.bowColor;

        // Check whether the user has pressed the retry button, if so set their high score in the player data object, then reload the game
        var retry = GameObject.Find("Button_PlayGame_GameOver_Retry").GetComponent<Button>();
        retry.onClick.AddListener(delegate { playerData.bestTime = playerScore.GetBestTime(); LoadSceneByNum(3); });

        // Check whether the user has pressed the retry button on winning screen, if so set their high score in the player data object, then reload the game
        var retryWinner = GameObject.Find("Button_PlayGame_WinningScreen_Retry").GetComponent<Button>();
        retryWinner.onClick.AddListener(delegate { playerData.bestTime = playerScore.GetBestTime(); LoadSceneByNum(3); });
    }

    private void OnEndEditName()
    {
        // Remember our players chosen character name between scenes
        playerData.characterName = inputCharName.text.ToString();
    }

    private void OnChangeDifficultyLevel()
    {
        // Update our difficulty text output for the user when they use the slider to say either easy, medium, or hard
        chosenDifficultyText.text = difficultyLevels[(int)chosenDifficulty.value];

        // Remember our players chosen difficulty level between scenes
        playerData.difficulty = (int)chosenDifficulty.value;
    }

    // Whenever the user switches the bow color with the left/right buttons, we grab the corresponding index in the colors array
    private void SwitchColor(int colorInt)
    {
        // Make sure we don't go under index 0 or over index 6 since we only have 7 different colors in our colors array
        if (colorInt < 0)
        {
            colorCount = colorInt = 6;
        }
        else if (colorInt > 6)
        {
            colorCount = colorInt = 0;
        }

        // Set our bow sprite color and remember it between scenes
        bowSprite.color = colors[colorInt];
        playerData.bowColor = colors[colorInt];
    }

    private void OnSaveGame()
    {
        // When the player's selections are saved, set this value to true so the user can press the play game button on main menu
        isInteractable = true;
    }

    private void SetSaveText()
    {
        saveConfirmation.text = playerData.characterName + ", your selections have been saved, click the back button and start playing!";
    }

    // If the user clicks the Exit button, exit the game, whether the user is using the unity editor or hes playing the .exe version
    private void OnExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
