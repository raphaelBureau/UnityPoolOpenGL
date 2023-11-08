using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GameManager))]

public class UIControls : MonoBehaviour
{
    string player1Name = "Joueur 1";
    string player2Name = "Joueur 2";
    [DllImport("__Internal")] private static extern bool IsMobile();

    [SerializeField] TextMeshProUGUI Message;
    [SerializeField] GameObject player1Display;
    [SerializeField] GameObject player2Display;
    [SerializeField] GameObject BackgroundScene;
    [SerializeField] GameObject SettingsMenu;
    [SerializeField] Image CamControlBackground;
    [SerializeField] Image SettingsToggleBackground;
    [SerializeField] Image SettingsMenuBackground;
    [SerializeField] Image StrengthToggleBackground;

    Color red = new Color(1, 0.1f, 0.1f, 0.5f);
    Color defaultColor = new Color(0.6f, 0.6f, 0.6f, 0.5f);
    Color focusedColor = new Color(0.6f, 0.6f, 0.6f, 1f);
    const float fade = 0.5f;
    const float solid = 1f;

    GameManager GM;

    public bool InUI { get; private set; }
    public bool StrengthMode { get; private set; }
    void Start()
    {
        GM = GetComponent<GameManager>();
        SettingsMenu.SetActive(false);
        CamControlBackground.color = defaultColor;
        SettingsToggleBackground.color = defaultColor;
        SettingsMenuBackground.color = defaultColor;
        InUI = false;

        StrengthToggleBackground.gameObject.SetActive(IsMobile());
    }

    public void EndGame()
    {
        if (GM.PlayerTurn)
        {
            if (GM.Player2Points == 7)
            {
                //win
                Message.text = player2Name + " a gagné la partie";
            }
            else
            {
                //loose
                Message.text = player1Name + " a gagné la partie, " + player2Name + " a fait tomber la boule 8 avant les boules 9-15";
            }
        }
        else
        {
            if (GM.Player1Points == 7)
            {
                //win
                Message.text = player1Name + " a gagné la partie";
            }
            else
            {
                //loose
                Message.text = player2Name + " a gagné la partie, " + player1Name + " a fait tomber la boule 8 avant les boules 1-7";
            }
        }
    }
    public void UpdateProfiles()
    {

        if (!GM.PlayerTurn)
        {

            player1Display.GetComponent<Image>().color = new Color(0.8f, 0.6f, 0.6f, 0.7f);
        }
        else
        {
            player1Display.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f, 0.5f);
        }
        TextMeshProUGUI[] text1 = player1Display.GetComponentsInChildren<TextMeshProUGUI>(); //lenght 3, 0= nom, 1= score, 2 = balltype

        text1[0].text = player1Name;

        text1[1].text = "Score: " + GM.Player1Points;

        if (GM.FirstBallFell)
        {
            text1[2].text = GM.Player1Solid ? "Solide" : "Rayé";
        }

        if (GM.PlayerTurn)
        {
            player2Display.GetComponent<Image>().color = new Color(0.8f, 0.6f, 0.6f, 0.7f);

        }
        else
        {
            player2Display.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f, 0.5f);
        }
        TextMeshProUGUI[] text2 = player2Display.GetComponentsInChildren<TextMeshProUGUI>(); //lenght 3, 0= nom, 1= score, 2 = balltype

        text2[0].text = player2Name;

        text2[1].text = "Score: " + GM.Player2Points;

        if (GM.FirstBallFell)
        {
            text2[2].text = GM.Player1Solid ? "Rayé" : "Solide";
        }
    }
    public void ToggleBackground(bool active)
    {
        BackgroundScene.SetActive(!active);
    }
    public void MouseCamEnter()
    {
        Color iconColor = CamControlBackground.color;
        iconColor.a = solid;
        CamControlBackground.color = iconColor;
        InUI = true;
    }
    public void MouseCamExit()
    {
        Color iconColor = CamControlBackground.color;
        iconColor.a = fade;
        CamControlBackground.color = iconColor;
        InUI = false;
    }
    public void ToggleSettingsMenu()
    {
        SettingsMenu.SetActive(!SettingsMenu.activeSelf);
        Color color = SettingsMenu.activeSelf ? red : defaultColor;
        color.a = SettingsToggleBackground.color.a;
        SettingsToggleBackground.color = color;
    }
    public void MouseSettingsEnter()
    {
        Color iconColor = SettingsToggleBackground.color;
        iconColor.a = solid;
        SettingsToggleBackground.color = iconColor;
        InUI = true;
    }
    public void MouseSettingsExit()
    {
        Color iconColor = SettingsToggleBackground.color;
        iconColor.a = fade;
        SettingsToggleBackground.color = iconColor;
        InUI = false;
    }
    public void MouseMenuEnter()
    {
        Color iconColor = SettingsMenuBackground.color;
        iconColor.a = solid - 0.1f;
        SettingsMenuBackground.color = iconColor;
        InUI = true;
    }
    public void MouseMenuExit()
    {
        Color iconColor = SettingsMenuBackground.color;
        iconColor.a = fade;
        SettingsMenuBackground.color = iconColor;
        InUI = false;
    }
    public void SetSens(float sliderValue)
    {
        const float maxSens = 2f;
        const float minSens = 0.1f;
        float sens = sliderValue * maxSens + minSens;
        GM.UpdateSens(sens);
    }
    public void MouseStrengthEnter()
    {
        Color iconColor = StrengthToggleBackground.color;
        iconColor.a = solid - 0.1f;
        StrengthToggleBackground.color = iconColor;
        InUI = true;
    }
    public void MouseStrengthExit()
    {
        Color iconColor = StrengthToggleBackground.color;
        iconColor.a = fade;
        StrengthToggleBackground.color = iconColor;
        InUI = false;
    }
    public void MouseStrengthToggle()
    {
        StrengthMode = !StrengthMode;
        Color color = StrengthMode ? red : defaultColor;
        color.a = StrengthToggleBackground.color.a;
        StrengthToggleBackground.color = color;
    }
}
