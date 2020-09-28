using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    public Animator _animator;
    static public Animator animator;
    static int levelToLoad;

    private void Start()
    {
        animator = _animator;
    }

    static public void FadeToLevel(int levelIndex)
    {
        levelToLoad = levelIndex;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadSceneAsync(levelToLoad);
    }
}
