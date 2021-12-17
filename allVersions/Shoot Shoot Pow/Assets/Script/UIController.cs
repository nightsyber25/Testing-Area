using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public TMP_Text overHeatedMessage;
    public Slider weaponTempSlider;

    public GameObject setupScreen;
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
        currentTime = startingTime;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        timeCounter.text = currentTime.ToString("0");    
        if(currentTime < 0)
        {
            currentTime = 0;
            setupScreen.SetActive(false);
        }
    }
}
