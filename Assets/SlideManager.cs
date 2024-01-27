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
    // Rounding of timestamps (decimal places)
    readonly int ROUND = 3;


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


    // Function called at the beginning of an experiment for every participant (not trial)
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
    // Used for getting inputs 
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
            {;
                slideNo++;
                if (slideNo == 1)
                {
                    SetSlide(1);
                }
                else if (slideNo == 2)
                {
                    SetSlide(2);
                }

                // ExecuteSpaceEvent();
            }
        }
        if (slideNo == 2 && !pressed)
        {
            timePressed += Time.deltaTime;
        }
    }

    // Linked to Reset All button on slide 1 
    public void ResetAll() {
        PlayerPrefs.DeleteAll();
        trialNumber = 1;
        participantNumber = 1;
        trialNo_display.text = "Trial " + trialNumber;
        participantNo_display.text = "Participant " + participantNumber;
        writer.Close();
        CreateNewCSV();
    }

    // Called at the end of each trial (after the participant answers the MCQ question)
    // to start the next trial
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

        writer.Close();

        SceneManager.LoadScene(0); // The whole expt is one scene. Calling 0 calls the same scene which resets it 
    }

    // Called at the start of each participant to create the file to save the results
    public void CreateNewCSV() {
        string filePath = "Assets\\Results\\Participant_" + participantNumber + ".csv";
 
        writer = new StreamWriter(filePath); // Writer saves the communication betw program csv so u can write into the file until u close it 
 
        writer.WriteLine("Trial,Time pressed,Random Time,Actual Adv Time,MCQ");
    }

    public void WriteToCSV(string content) { // Specifying that this function is taking in a string, and ur calling it "content" 
        writer.WriteLine(content);
    }

    void ExecuteSpaceEvent()
    {
        if (slideNo < 2)
        {
            pressed = false;
            slideNo++;
        }

        if (slideNo == 1)
        {
            SetSlide(1);
        } else if (slideNo == 2)
        {
            pressed = true;
            SetSlide(2);
        } else if (slideNo == 3)
        {
            pressed = true;
        }
    }

    void ExecuteReturnEvent() { 
        if (slideNo == 2)
        {
            slideNo++;

            // Consolidates results into the format to be inserted into the results CSV.
            string content = trialNumber.ToString() +
                                ", " +
                                (pressed ? Math.Round(timePressed, ROUND).ToString() : "Not pressed") + // ? = Asks if it's pressed. if Yes, 1st thing, No, 2nd option aft comma
                                ", " +
                                Math.Round(randTime, ROUND).ToString() +
                                ", " +
                                Math.Round(slideAdvance, ROUND).ToString();

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
        curr_results += "," + opt; // The opts are ABCD, replaces "opt" w whichever is passed in when that specific function is called 

        // A += B ===> A = A + B 
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


    // Sets the display to the given slide number
    // Slide 0: Instructions for Participants slide
    // Slide 1: Slide to press button/foot pedal
    // Slide 2: Slide after pressing
    // Slide 3: Slide for the end of experiment
    // Slide 4: Slide with MCQ question for what just happened
    void SetSlide(int sNo)
    {
        // Debug.Log("SET SLIDE " + sNo);
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
