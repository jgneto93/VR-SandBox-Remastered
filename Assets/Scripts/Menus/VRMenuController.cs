using UnityEngine;
using UnityEngine.UI;

public class VRMenuController : MonoBehaviour
{
    public Button[] menuButtons; // Arraste os botões do menu aqui.
    public float angleLimit = 30.0f; // Ângulo de limite para selecionar um botão.

    private int selectedButtonIndex = 0;

    private void Update()
    {
        // Detecta o movimento do thumbstick.
        float horizontalInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;

        // Seleciona o próximo ou anterior botão com base no movimento do thumbstick.
        if (horizontalInput > angleLimit)
        {
            SelectNextButton();
        }
        else if (horizontalInput < -angleLimit)
        {
            SelectPreviousButton();
        }

        // Aciona um botão ao pressionar o gatilho.
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            ExecuteSelectedButtonAction();
        }
    }

    private void SelectNextButton()
    {
        // Desativa a seleção do botão atual.
        menuButtons[selectedButtonIndex].OnDeselect(null);

        // Avance para o próximo botão.
        selectedButtonIndex = (selectedButtonIndex + 1) % menuButtons.Length;

        // Ativa a seleção do novo botão.
        menuButtons[selectedButtonIndex].OnSelect(null);
    }

    private void SelectPreviousButton()
    {
        // Desativa a seleção do botão atual.
        menuButtons[selectedButtonIndex].OnDeselect(null);

        // Retroceda para o botão anterior.
        selectedButtonIndex = (selectedButtonIndex - 1 + menuButtons.Length) % menuButtons.Length;

        // Ativa a seleção do novo botão.
        menuButtons[selectedButtonIndex].OnSelect(null);
    }

    private void ExecuteSelectedButtonAction()
    {
        // Execute a ação associada ao botão selecionado.
        menuButtons[selectedButtonIndex].onClick.Invoke();
    }
}
