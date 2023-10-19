using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public OVRInput.Button ativarMenu = OVRInput.Button.Start;
    public OVRInput.Button menuDrawn = OVRInput.Button.Start;
    public GameObject MenuButtons;
    public GameObject DrawnMenu;

    void Update()
    {
        if (OVRInput.GetDown(ativarMenu))
        {
            if (MenuButtons.activeSelf)
            {
                MenuButtons.SetActive(false);
            }
            else
            {
                MenuButtons.SetActive(true);
                DrawnMenu.SetActive(false); // Desativa o outro menu se estiver ativo
            }
        }

        if (OVRInput.GetDown(menuDrawn))
        {
            if (DrawnMenu.activeSelf)
            {
                DrawnMenu.SetActive(false);
            }
            else
            {
                DrawnMenu.SetActive(true);
                MenuButtons.SetActive(false); // Desativa o outro menu se estiver ativo
            }
        }
    }
}
