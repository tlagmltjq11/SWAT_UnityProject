using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneManager : MonoBehaviour
{
    #region Static Field
    static string m_nextScene;
    #endregion

    #region Field
    [SerializeField]
    Image m_progressBar;
    #endregion

    #region Unity Methods
    private void Start()
    {
        StartCoroutine(LoadSceneProgress());
    }
    #endregion

    #region Static Methods
    public static void LoadScene(string sceneName)
    {
        m_nextScene = sceneName;
        SceneManager.LoadScene("Loading");
    }
    #endregion

    #region Coroutine
    IEnumerator LoadSceneProgress()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(m_nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;

        while(!op.isDone)
        {
            yield return null;

            if (op.progress < 0.9f)
            {
                m_progressBar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                m_progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);

                if(m_progressBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
    #endregion
}

