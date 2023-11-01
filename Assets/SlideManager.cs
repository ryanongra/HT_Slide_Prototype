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


    StreamWriter writer;
    
    void Start()
    {
        slide0.SetActive(true);
        slide1.SetActive(false);
        slide2.SetActive(false);
        slideResult.SetActive(false);

        trialNumber = 1;
        trialNo_display.text = "Trial " + trialNumber;

        participantNumber = PlayerPrefs.GetInt("participant_no", 1);
        participantNo_display.text = "Participant " + participantNumber;

        slideNo = 0;

        randTime = RandomFloat(MIN_RAND_TIME, MAX_RAND_TIME);

        CreateNewCSV();

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

    public void ResetAll() {
        PlayerPrefs.DeleteAll();
        trialNumber = 1;
        participantNumber = 1;
        trialNo_display.text = "Trial " + trialNumber;
        participantNo_display.text = "Participant " + participantNumber;
        CreateNewCSV();
    }

    public void NextTrial()
    {
        trialNumber++;
        trialNo_display.text = "Trial " + trialNumber;

        slide0.SetActive(true);
        slide1.SetActive(false);
        slide2.SetActive(false);
        slideResult.SetActive(false);

        slideNo = 0;
        
        timePressed = 0;
        randTime = RandomFloat(MIN_RAND_TIME, MAX_RAND_TIME);
        slideAdvance = 0;

        pressed = false;
    }


    // Moves onto the next participant
    public void ResetAndRestart() {

        writer.Close();

        PlayerPrefs.SetInt("participant_no", participantNumber + 1);

        SceneManager.LoadScene(0);
    }

    public void CreateNewCSV() {
        string filePath = "Assets\\Results\\Participant_" + participantNumber + ".csv";
 
        writer = new StreamWriter(filePath);
 
        // File.AppendAllText(filePath, "Trial,Time pressed, Random Time, Actual Adv Time");
        writer.WriteLine("Trial,Time pressed,Random Time,Actual Adv Time");

        // writer.Close();
    }

    public void WriteToCSV(string content) {
        writer.WriteLine(content);
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
            slide_end.SetActive(false);
        } else if (slideNo == 2)
        {
            slide0.SetActive(false);
            slide1.SetActive(false);
            slide2.SetActive(true);
            slide_end.SetActive(false);
        }
    }

    void ExecuteReturnEvent() { 
        if (slideNo == 2)
        {
            slideNo++;

            timePressed_result.text = pressed ? Math.Round(timePressed, 2).ToString() : "Not pressed";
            randomTime_result.text = Math.Round(randTime, 2).ToString();
            actualAdv_result.text = Math.Round(slideAdvance, 2).ToString();

            string content = trialNumber.ToString() +
                                ", " +
                                (pressed ? Math.Round(timePressed, 2).ToString() : "Not pressed") +
                                ", " +
                                Math.Round(randTime, 2).ToString() +
                                ", " +
                                Math.Round(slideAdvance, 2).ToString();

            Debug.Log(content);

            WriteToCSV(content);

            CheckForEndOfExperiment();


        } else if (slideNo == 3) {
            ResetAndRestart();
        }
    }

    void CheckForEndOfExperiment() {
        if (trialNumber == NUM_TRIALS) {
            slide0.SetActive(false);
            slide1.SetActive(false);
            slide2.SetActive(false);
            slide_end.SetActive(true);

            slideNo = 3;
        } else {
            NextTrial();
        }
    }


    public float RandomFloat(float min, float max)
    {
        Random random = new Random();
        Debug.Log(random.NextDouble().ToString());
        return (float)(random.NextDouble() * (max - min) + min);
    }



}
