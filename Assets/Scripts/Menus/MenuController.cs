using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    // Variável para armazenar a opção selecionada pelo usuário.
    private string selectedOption;

    public void OnButtonAClick()
    {
        // Lógica para o botão A.
        Debug.Log("Botão A foi pressionado.");
        selectedOption = "A"; // Ou qualquer outra lógica para definir a resposta selecionada.
    }

    public void OnButtonBClick()
    {
        // Lógica para o botão B.
        Debug.Log("Botão B foi pressionado.");
        selectedOption = "B"; // Ou qualquer outra lógica para definir a resposta selecionada.
    }

    public void OnButtonCClick()
    {
        // Lógica para o botão C.
        Debug.Log("Botão C foi pressionado.");
        selectedOption = "C"; // Ou qualquer outra lógica para definir a resposta selecionada.
    }

    public void OnButtonDClick()
    {
        // Lógica para o botão D.
        Debug.Log("Botão D foi pressionado.");
        selectedOption = "D"; // Ou qualquer outra lógica para definir a resposta selecionada.
    }

    public void OnButtonABClick()
    {
        // Lógica para o botão A-B.
        Debug.Log("Botão A-B foi pressionado.");
        selectedOption = "A-B"; // Ou qualquer outra lógica para definir a resposta selecionada.
    }

    public void OnButtonBCClick()
    {
        // Lógica para o botão B-C.
        Debug.Log("Botão B-C foi pressionado.");
        selectedOption = "B-C"; // Ou qualquer outra lógica para definir a resposta selecionada.
    }

    public void OnButtonCDClick()
    {
        // Lógica para o botão C-D.
        Debug.Log("Botão C-D foi pressionado.");
        selectedOption = "C-D"; // Ou qualquer outra lógica para definir a resposta selecionada.
    }

    public void OnButtonDEClick()
    {
        // Lógica para o botão D-E.
        Debug.Log("Botão D-E foi pressionado.");
        selectedOption = "D-E"; // Ou qualquer outra lógica para definir a resposta selecionada.
    }

    // Método para retornar a resposta selecionada pelo usuário.
    public string GetSelectedOption()
    {
        return selectedOption;
    }
}
