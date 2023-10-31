using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;
using Math = System.Math;

public class SlideManager : MonoBehaviour
{
    //////////////////////////////////////////////
    /// EXPERIMENTER VARIABLES
    //////////////////////////////////////////////
    // The number of trials for each experiment/participant
    readonly int NUM_TRIALS = 5;
    // The minimum time before the slide might automatically advance
    readonly float MIN_RAND_TIME = 0.7f;
    // The maximum time the slide will take to automatically advance
    readonly float MAX_RAND_TIME = 2f;


    //////////////////////////////////////////////
    /// PROGRAM VARIABLES (DO NOT MODIFY)
    //////////////////////////////////////////////
    public GameObject slide0;
    public GameObject slide1;
    public GameObject slide2;
    public GameObject slide_end;
    public GameObject slideResult;

    public TextMeshProUGUI timePressed_result;
    public TextMeshProUGUI randomTime_result;
    public TextMeshProUGUI actualAdv_result;

    int trialNumber = 1;
    public TextMeshProUGUI trialNo_display;

    int participantNumber = 1;
    public TextMeshProUGUI participantNo_display;

    int slideNo = 0;

    float timePressed = 0;
    float randTime = 0;
    float slideAdvance = 0;

    bool pressed = false;

    
    void Start()
    {
        slide0.SetActive(true);
        slide1.SetActive(false);
        slide2.SetActive(false);
        slideResult.SetActive(false);

        trialNumber = PlayerPrefs.GetInt("trial_no", 1);
        trialNo_display.text = "Trial " + trialNumber;

        participantNumber = PlayerPrefs.GetInt("participant_no", 1);
        participantNo_display.text = "Participant " + participantNumber;

        randTime = RandomFloat(MIN_RAND_TIME, MAX_RAND_TIME);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            pressed = true;
            ExecuteSpaceEvent();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ExecuteReturnEvent();
        }
        if (slideNo == 1)
        {
            timePressed += Time.deltaTime;
            slideAdvance += Time.deltaTime;

            if (slideAdvance >= randTime)
            {
                ExecuteSpaceEvent();
            }
        }
        if (slideNo == 2 && !pressed)
        {
            timePressed += Time.deltaTime;
        }
    }

    public void Restart()
    {
        PlayerPrefs.SetInt("participant_no", participantNumber);
        PlayerPrefs.SetInt("trial_no", trialNumber + 1);
        SceneManager.LoadScene(0);
    }

    public void ResetAndRestart() {
        trialNumber = 1;
        trialNo_display.text = "Trial " + trialNumber;
        PlayerPrefs.SetInt("trial_no", trialNumber);
        PlayerPrefs.SetInt("participant_no", participantNumber + 1);

        CreateNewCSV();

        SceneManager.LoadScene(0);
    }

    public void CreateNewCSV() {
        string filePath = "Results/Assets/Participant_" + participantNumber;
 
        StreamWriter writer = new StreamWriter(filePath);
 
        writer.WriteLine("Trial,Time pressed, Random Time, Actual Adv Time");
    }

    void ExecuteSpaceEvent()
    {
        if (slideNo < 2)
        {
            slideNo++;
        }
        if (slideNo == 1)
        {
            pressed = false;
            slide0.SetActive(false);
            slide1.SetActive(true);
            slide2.SetActive(false);
        } else if (slideNo == 2)
        {
            slide0.SetActive(false);
            slide1.SetActive(false);
            slide2.SetActive(true);
        }
    }

    void ExecuteReturnEvent() { 
        if (slideNo == 2)
        {
            slideNo++;

            timePressed_result.text = pressed ? Math.Round(timePressed, 2).ToString() : "Not pressed";
            randomTime_result.text = Math.Round(randTime, 2).ToString();
            actualAdv_result.text = Math.Round(slideAdvance, 2).ToString();

            CheckForEndOfExperiment();

            // slide0.SetActive(false);
            // slide1.SetActive(false);
            // slide2.SetActive(false);

            
            // slideResult.SetActive(true);
        } else if (slideNo == 3) {
            ResetAndRestart();
        }
    }

    void CheckForEndOfExperiment() {
        if (trialNumber == NUM_TRIALS) {
            slide_end.SetActive(true);
        } else {
            Restart();
        }
    }


    public float RandomFloat(float min, float max)
    {
        Random random = new Random();
        Debug.Log(random.NextDouble().ToString());
        return (float)(random.NextDouble() * (max - min) + min);
    }



}
