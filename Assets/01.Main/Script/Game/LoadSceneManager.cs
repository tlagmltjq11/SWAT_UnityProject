using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
public class LoadSceneManager : DonDestroy<LoadSceneManager>
{
    public enum eSceneState
    {
        None = -1,
        TitleScene,
        Stage1,
        Stage2,
        Stage3
    }

    eSceneState m_state = eSceneState.TitleScene;
    eSceneState m_loadState = eSceneState.None;
    string m_progressLabel;
    AsyncOperation m_loadSceneState;
    SpriteRenderer m_loadingBgSpr;

    public void SetState(eSceneState state)
    {
        m_state = state;
    }

    public eSceneState GetState()
    {
        return m_state;
    }

    public void LoadSceneAsync(eSceneState state)
    {
        //load를 하고 있다면
        if (m_loadState != eSceneState.None)
        {
            return;
        }

        m_loadState = state;
        ///m_loadingBgSpr.enabled = true;
        m_loadSceneState = SceneManager.LoadSceneAsync(state.ToString());
    }
    protected override void OnStart()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_loadSceneState != null && m_loadState != eSceneState.None)
        {
            if (m_loadSceneState.isDone)
            {
                //m_loadingBgSpr.enabled = false;
                m_loadSceneState = null;

                m_state = m_loadState;
                m_loadState = eSceneState.None;
                m_progressLabel = "100";
            }
            else
            {
                m_progressLabel = ((int)(m_loadSceneState.progress * 100)).ToString();
            }
        }
    }
}

