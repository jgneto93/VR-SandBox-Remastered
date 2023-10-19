using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuPrincipalManager : MonoBehaviour
{
    [SerializeField] private string nomeDoLevelDeJogo;
    [SerializeField] private string nomeDoLevel;
    [SerializeField] private string nomeDoLevelDeTutorial;
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private Pen pen;
    public GameObject Spw;
    public GameObject penDraw;
    public GameObject terrain1;
    public GameObject terrain2;
    public GameObject terrain3;
    public GameObject terrain4;
    public GameObject currentDrawing;

    private bool treesOn = false;
    private bool monitorsOn = false;

    public void Iniciar()
    {
        SceneManager.LoadScene(nomeDoLevelDeJogo);
    }

    public void Tutorial()
    {
        SceneManager.LoadScene(nomeDoLevelDeTutorial);
    }

    public void Voltar()
    {
        SceneManager.LoadScene(nomeDoLevel);
    }

    public void SairJogo()
    {
        Debug.Log("Sair do Jogo");
        Application.Quit();
    }

    public void Show()
    {
        Spw.SetActive(!Spw.activeSelf);
    }

    public void Terreno1()
    {
        terrain1.SetActive(!terrain1.activeSelf);
    }

    public void Terreno2()
    {
        terrain2.SetActive(!terrain2.activeSelf);
    }

    public void Terreno3()
    {
        terrain3.SetActive(!terrain3.activeSelf);
    }

    public void Terreno4()
    {
        terrain4.SetActive(!terrain4.activeSelf);
    }

    public void ClearDrawing()
    {
        if (currentDrawing != null)
        {
            Destroy(currentDrawing);
        }
    }

    public void DrawingPen()
    {
        penDraw.SetActive(!penDraw.activeSelf);
    }

    public void Monitors()
    {
        monitorsOn = !monitorsOn;
        GameObject[] monitorObjects = GameObject.FindGameObjectsWithTag("monitor");
        foreach(GameObject monitorObject in monitorObjects)
        {
            monitorObject.SetActive(monitorsOn);
            monitorObject.tag = monitorsOn ? "monitor" : "inactive";
        }
    }
}
