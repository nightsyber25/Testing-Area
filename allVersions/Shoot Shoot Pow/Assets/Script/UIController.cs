using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public GameObject setupScreen;
    public GameObject optionScreen;
    public TMP_Text paperAmount;
    public TMP_Text rockAmount;
    public TMP_Text scissorAmount;
    public TMP_Text specialAmount;
    [SerializeField] TMP_Text timeCounter;

    private float currentTime = 0f;
    private float startingTime = 5f;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        setupScreen.SetActive(true);
        UpdateSetupScreen();
        currentTime = startingTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (setupScreen.activeInHierarchy)
        {
            currentTime -= 1 * Time.deltaTime;
            timeCounter.text = currentTime.ToString("0");
            if (currentTime < 0)
            {
                currentTime = 0;
                setupScreen.SetActive(false);
                DeckController.instance.spawnDeck();
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ShowHideOption();
        }
        
        if(optionScreen.activeInHierarchy && Cursor.lockState != CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ShowHideOption()
    {
        if(!optionScreen.activeInHierarchy)
        {
            optionScreen.SetActive(true);
        }
        else
        {
            optionScreen.SetActive(false);
        }
    }

    public void ReturnToMainMenu()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void UpdateSetupScreen()
    {
        scissorAmount.text = DeckController.instance.numOfCardInDeck[1].ToString();
        paperAmount.text = DeckController.instance.numOfCardInDeck[2].ToString();
        rockAmount.text = DeckController.instance.numOfCardInDeck[3].ToString();
        specialAmount.text = (DeckController.instance.numOfCardInDeck[4]+
        DeckController.instance.numOfCardInDeck[5]+
        DeckController.instance.numOfCardInDeck[6]+
        DeckController.instance.numOfCardInDeck[7]+
        DeckController.instance.numOfCardInDeck[8]).ToString();
    }
}
