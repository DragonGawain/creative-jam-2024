using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    static GameObject queue;

    GameObject MainMenu;

    GameObject SelectMenu;

    GameObject CreditMenu;

    static Dictionary<VariationType, Sprite> queueIcons = new();

    // Start is called before the first frame update
    void Awake()
    {
        MainMenu = GameObject.FindGameObjectWithTag("MainMenu");
        SelectMenu = GameObject.FindGameObjectWithTag("SelectMenu");
        CreditMenu = GameObject.FindGameObjectWithTag("CreditMenu");

        SelectMenu.SetActive(false);
        CreditMenu.SetActive(false);

        queue = GameObject.FindWithTag("Queue");
        Sprite[] icons = Resources.LoadAll<Sprite>("queueIcons");
        foreach (Sprite icon in icons)
        {
            switch (icon.name)
            {
                case "wind_u":
                    queueIcons.Add(VariationType.WIND_UP, icon);
                    break;
                case "wind_d":
                    queueIcons.Add(VariationType.WIND_DOWN, icon);
                    break;
                case "wind_l":
                    queueIcons.Add(VariationType.WIND_LEFT, icon);
                    break;
                case "wind_r":
                    queueIcons.Add(VariationType.WIND_RIGHT, icon);
                    break;
                case "wind_base":
                    queueIcons.Add(VariationType.WIND, icon);
                    break;
                case "move_u":
                    queueIcons.Add(VariationType.MOVE_UP, icon);
                    break;
                case "move_d":
                    queueIcons.Add(VariationType.MOVE_DOWN, icon);
                    break;
                case "move_l":
                    queueIcons.Add(VariationType.MOVE_LEFT, icon);
                    break;
                case "move_r":
                    queueIcons.Add(VariationType.MOVE_RIGHT, icon);
                    break;
                case "mimic_head":
                    queueIcons.Add(VariationType.MIMIC, icon);
                    break;
                case "mimic_base":
                    queueIcons.Add(VariationType.MIMIC_BODY, icon);
                    break;
                case "boots":
                    queueIcons.Add(VariationType.BOOTS, icon);
                    break;
            }
        }
    }

    public void OnStartButton()
    {
        MainMenu.SetActive(false);
        SelectMenu.SetActive(true);
    }

    public void OnSelectButton(int levelNb)
    {
        SceneManager.LoadScene("SampleScene");
        StartCoroutine(WaitForJankToPass(levelNb));
        // GameManager.StartNewLevel(levelNb);
    }

    public void OnSelectButtonDEBUG(int levelNb)
    {
        // SceneManager.LoadScene("SampleScene");
        GameManager.StartNewLevel(levelNb);
    }
    public void OnCreditsButton()
    {
        MainMenu.SetActive(false);
        CreditMenu.SetActive(true);
    }

    public void OnBackButton()
    {
        CreditMenu.SetActive(false);
        SelectMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update() { }

    public static void UpdateQueueVisuals(VariationType[] items)
    {
        // set to 6 for now cause that's the length of the queue.
        // Can be changed later :shrug:
        for (int i = 0; i < items.Length; i++)
        {
            queue.transform.GetChild(5 - i).GetComponent<Image>().sprite = queueIcons[items[i]];
        }
    }

    IEnumerator WaitForJankToPass(int levelnb)
    {
        yield return new WaitForSeconds(1);
        GameManager.StartNewLevel(levelnb);
    }
}
