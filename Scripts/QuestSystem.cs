using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestSystem : MonoBehaviour
{
    public Text questText;
    int currentQuest = 0;
    public string[] quests;
    public GameObject questLabel;
    public GameObject questLog;

    // Start is called before the first frame update
    void Start()
    {
        questText.text = "Press +/- to change map zoom";
    }

    public int SaveGame () {
        return currentQuest;
    }

    public void LoadGame(int setQuest) {
        currentQuest = setQuest;
    }

    // Update is called once per frame
    void Update()
    {
        questText.text = quests[currentQuest];
    }

    public void AdvanceQuest() {
        currentQuest++;
        if (currentQuest > quests.Length) {
            currentQuest = quests.Length;
        }
        PlayerPrefs.SetInt("CurrentQuest", currentQuest);
    }

    public void ZoomQuestDone() {
        if (currentQuest == 0) {
            AdvanceQuest();
        }
    }

    public void MoonQuestDone() {
        if (currentQuest == 1) {
            AdvanceQuest();
        }
    }

    public void SpaceStationQuestDone() {
        if (currentQuest == 2) {
            AdvanceQuest();
        }
    }

    public void BuySellQuestDone() {
        if(currentQuest == 3) {
            AdvanceQuest();
        }
    }

    public void DisplayQuests() {
        questLabel.SetActive(true);
        questLog.SetActive(true);
    }
}
