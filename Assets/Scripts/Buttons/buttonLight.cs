using UnityEngine;

public class buttonLight : MonoBehaviour
{
    public Light lightToToggle;

    private bool isObjectActivated = false; // variável de controle

    private void OnEnable()
    {
        if (isObjectActivated)
        {
            // ativa a luz quando o objeto for adicionado na cena
            lightToToggle.enabled = true;
        }
    }

    private void Update()
    {
        if (isObjectActivated && OVRInput.GetDown(OVRInput.Button.One)) // verifica se o botão A foi pressionado
        {
            lightToToggle.enabled = !lightToToggle.enabled;
        }  
    }

}


