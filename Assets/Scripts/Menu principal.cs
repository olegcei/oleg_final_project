using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour
{
    [SerializeField] RectTransform panelFade;
    [SerializeField] TMP_Text text;
    [SerializeField] GameObject Contenedor1;

    Color c;
    bool final;
    bool startGame; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
 
        c = panelFade.gameObject.GetComponent<Image>().color;

    }

    public void MenuStart()
    {
        final = true;
        StartCoroutine(FadeOut());

    }

    public void GameStart()
    {
        final = true;
        StartCoroutine (FadeOut());
    }

    // Update is called once per frame
    IEnumerator FadeIn()
    {
        for (float Alfa = 1f; Alfa >= 0; Alfa -= 0.05f)
        {
            c.a = Alfa;
            panelFade.gameObject.GetComponent<Image>().color = c;
            yield return new WaitForSeconds(0.01f);
        }
        c.a = 0f;
        panelFade.gameObject.GetComponent<Image>().color = c;
        yield return new WaitForSeconds(0.1f);

    }

    IEnumerator FadeOut()
    {
        for (float Alfa = 0f; Alfa <= 1; Alfa += 0.05f)
        {
            c.a = Alfa;
            panelFade.gameObject.GetComponent<Image>().color = c;
            yield return new WaitForSeconds(0.01f);
        }
        c.a = 1f;
        panelFade.gameObject.GetComponent<Image>().color = c;
        yield return new WaitForSeconds(0.00001f);
        if (final) SceneManager.LoadScene("Options");
        if (startGame) SceneManager.LoadScene("level");

    }
}
