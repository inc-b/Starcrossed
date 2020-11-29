using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    public ShipController ship;
    public EmailHandler email;
    public bool freshStart;
    public IntroCutsceneHandler introCutscene;
    public QuestSystem questSystem;

    // Start is called before the first frame update
    void Start()
    {
        int saveDataPresent = PlayerPrefs.GetInt("SaveDataPresent");
        if (freshStart || saveDataPresent != 1) {
            NewGame();
            introCutscene.skipcutScene = false;
        } else {
            introCutscene.skipcutScene = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveGame() {
        PlayerPrefs.SetInt("SaveDataPresent", 1);
        float[] shipData = ship.SaveGame();
        for (int i = 0; i < 14; i++) {
            PlayerPrefs.SetFloat(i.ToString(), shipData[i]);
        }
        int currentEmail = email.SaveGame();
        int currentQuest = questSystem.SaveGame();
        PlayerPrefs.SetInt("CurrentEmail", currentEmail);
        PlayerPrefs.SetInt("CurrentQuest", currentQuest);
    }

    public void LoadGame() {
        float[] shipData = new float[14];

        for (int i = 0; i < 14; i++) {
            shipData[i] = PlayerPrefs.GetFloat(i.ToString());
        }
        ship.LoadGame(shipData);

        email.LoadGame(PlayerPrefs.GetInt("CurrentEmail"));
        questSystem.LoadGame(PlayerPrefs.GetInt("CurrentQuest"));
    }

    public void NewGame() {
        PlayerPrefs.DeleteAll();
    }
}
