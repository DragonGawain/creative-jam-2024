using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    static GameObject queue;

    GameObject MainMenu;

    GameObject SelectMenu;

    GameObject CreditMenu;

    static int queueSize = 6;

    static Dictionary<VariationType, Sprite> queueIcons = new();

    static GameObject W1;
    static GameObject W2;
    static GameObject W3;

    // Start is called before the first frame update
    void Awake()
    {
        MainMenu = GameObject.FindGameObjectWithTag("MainMenu");
        SelectMenu = GameObject.FindGameObjectWithTag("SelectMenu");
        CreditMenu = GameObject.FindGameObjectWithTag("CreditMenu");

        // SelectMenu.SetActive(false);
        // CreditMenu.SetActive(false);

    }

    public void OnStartButton()
    {
        MainMenu.SetActive(false);
        SelectMenu.SetActive(true);
    }

    public void OnSelectButton(int levelNb)
    {
        SceneManager.LoadScene("MasterScene");
        StartCoroutine(WaitForJankToPass(levelNb));
        initialize();
        // GameManager.StartNewLevel(levelNb);
    }

    public static void IncrementWind(int n)
    {
        switch (n)
        {
            case 1:
                W1.SetActive(true);
                W2.SetActive(false);
                W3.SetActive(false);
                break;
            case 2:
                W1.SetActive(false);
                W2.SetActive(true);
                W3.SetActive(false);
                break;
            case 3:
                W1.SetActive(false);
                W2.SetActive(false);
                W3.SetActive(true);
                break;
            default:
        }
    }

    static void initialize()
    {
        queueIcons.Clear();
        
        queue = GameObject.FindWithTag("Queue");
        W1 = queue.transform.parent.GetChild(3).gameObject;
        W2 = queue.transform.parent.GetChild(4).gameObject;
        W3 = queue.transform.parent.GetChild(5).gameObject;
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
        if (queue == null)
            initialize();
        // set to 6 for now cause that's the length of the queue.
        // Can be changed later :shrug:
        for (int i = 0; i < items.Length; i++)
        {
            queue.transform.GetChild((queueSize - 1) - i).GetComponent<Image>().sprite = queueIcons[
                items[i]
            ];
        }
    }

    IEnumerator WaitForJankToPass(int levelnb)
    {
        yield return new WaitForSeconds(1);
        GameManager.StartNewLevel(levelnb);
    }

    public static void SetQueueSize(int n)
    {
        queueSize = n;
        // also change the UI to display the correct number of queue slots
    }
}
