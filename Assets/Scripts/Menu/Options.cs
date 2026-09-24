using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Options : MonoBehaviour
{

    [SerializeField] RectTransform panelFade;

    Color c;
    bool back;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        c = panelFade.gameObject.GetComponent<Image>().color;
    }

    // Update is called once per frame
    public void GetBack()
    {
        back = true;
        StartCoroutine(FadeOut());
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
        if (back) SceneManager.LoadScene("MenuPrincipal");
    }

}
