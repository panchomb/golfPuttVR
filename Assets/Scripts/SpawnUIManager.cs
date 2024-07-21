using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpawnUIManager : MonoBehaviour
{
    public Button nuevoGreenButton;

    void Start()
    {
        if (nuevoGreenButton != null)
        {
            nuevoGreenButton.onClick.AddListener(OnNuevoGreenButtonClick);
        }
    }

    public void OnNuevoGreenButtonClick()
    {
        Debug.Log("[UI] clicked");
        SceneManager.LoadScene("PerlinNoise");
    }
}
