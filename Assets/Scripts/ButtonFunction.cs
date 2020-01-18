using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunction : MonoBehaviour
{
    //menu transform
    public Transform menu;
    //controls transform
    public Transform controls;
    //story transform
    public Transform story;
    //bool for story or controls return
    bool fromControls;
    //game transform
    public Transform main;

    //sound, used for all menus
    public AudioSource clickSound;

    //goes to control screen
    public void Controls()
    {
        Click();
        fromControls = true;
        LevelManager.Instance.MoveCamera(menu, controls, false);
    }

    //returns from control screen
    public void Return()
    {
        Click();
        Transform used;
        if (fromControls) used = controls;
        else used = story;
        LevelManager.Instance.MoveCamera(used, menu, false);
    }

    //Starts game
    public void StartGame()
    {
        Click();
        LevelManager.Instance.MoveCamera(menu, main, true);
    }

    //goes to story screen
    public void Story()
    {
        Click();
        fromControls = false;
        LevelManager.Instance.MoveCamera(menu, story, false);
    }

    //restarts the game
    public void Restart()
    {
        Click();
        //reset timescale if needed
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    //Quits game
    public void Quit()
    {
        Click();
        Application.Quit();
    }

    //plays sound
    void Click()
    {
        if (clickSound) clickSound.Play();
    }
}
