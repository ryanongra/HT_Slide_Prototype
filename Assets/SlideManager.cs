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
    readonly int NUM_TRIALS = 25;
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
    public GameObject slide_qn;

    int trialNumber = 1;
    public TextMeshProUGUI trialNo_display;

    int participantNumber = 1;
    public TextMeshProUGUI participantNo_display;

    int slideNo = 0;

    float timePressed = 0;
    float randTime = 0;
    float slideAdvance = 0;

    bool pressed = false;

    string curr_results;

    StreamWriter writer;
    
    void Start()
    {
        SetSlide(0);

        trialNumber = 1;
        trialNo_display.text = "Trial " + trialNumber;

        participantNumber = PlayerPrefs.GetInt("participant_no", 1);
        participantNo_display.text = "Participant " + participantNumber;

        randTime = RandomFloat(MIN_RAND_TIME, MAX_RAND_TIME);

        CreateNewCSV();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ExecuteSpaceEvent();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ExecuteReturnEvent();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ExecuteREvent();
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

        SetSlide(0);
        
        timePressed = 0;
        randTime = RandomFloat(MIN_RAND_TIME, MAX_RAND_TIME);
        slideAdvance = 0;

        pressed = false;
    }


    // Moves onto the next participant
    public void ResetAndRestart() {

        PlayerPrefs.SetInt("participant_no", participantNumber + 1);

        SceneManager.LoadScene(0);
    }

    public void CreateNewCSV() {
        string filePath = "Assets\\Results\\Participant_" + participantNumber + ".csv";
 
        writer = new StreamWriter(filePath);
 
        // File.AppendAllText(filePath, "Trial,Time pressed, Random Time, Actual Adv Time");
        writer.WriteLine("Trial,Time pressed,Random Time,Actual Adv Time,MCQ");

        // writer.Close();
    }

    public void WriteToCSV(string content) {
        writer.WriteLine(content);
    }

    void ExecuteSpaceEvent()
    {
        if (slideNo < 2)
        {
            pressed = true;
            slideNo++;
        }

        if (slideNo == 1)
        {
            pressed = false;
            SetSlide(1);
        } else if (slideNo == 2)
        {
            SetSlide(2);
        }
    }

    void ExecuteReturnEvent() { 
        if (slideNo == 2)
        {
            slideNo++;

            string content = trialNumber.ToString() +
                                ", " +
                                (pressed ? Math.Round(timePressed, 2).ToString() : "Not pressed") +
                                ", " +
                                Math.Round(randTime, 2).ToString() +
                                ", " +
                                Math.Round(slideAdvance, 2).ToString();

            SetSlide(4);

            curr_results = content;

        } 
    }

    void ExecuteREvent()
    {
        if (slideNo == 3)
        {
            ResetAndRestart();
        }
    }

    public void OnPress_A()
    {
        QnPress("A");
    }

    public void OnPress_B()
    {
        QnPress("B");
    }

    public void OnPress_C()
    {
        QnPress("C");
    }

    public void OnPress_D()
    {
        QnPress("D");
    }

    void QnPress(string opt)
    {
        curr_results += "," + opt;
        WriteToCSV(curr_results);
        CheckForEndOfExperiment();
    } 

    void CheckForEndOfExperiment() {
        if (trialNumber == NUM_TRIALS) {
            writer.Close();
            SetSlide(3);
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

    void SetSlide(int sNo)
    {
        Debug.Log("SET SLIDE " + sNo);
        slideNo = sNo;

        slide0.SetActive(false);
        slide1.SetActive(false);
        slide2.SetActive(false);
        slide_end.SetActive(false);
        slide_qn.SetActive(false);

        switch (sNo)
        {
            case 0: slide0.SetActive(true); break;
            case 1: slide1.SetActive(true); break;
            case 2: slide2.SetActive(true); break;
            case 3: slide_end.SetActive(true); break;
            case 4: slide_qn.SetActive(true); break;
        }
    }



}
