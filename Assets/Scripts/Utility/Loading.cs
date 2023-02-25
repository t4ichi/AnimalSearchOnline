using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    AsyncOperation scene;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene(string Scenename = "Game")
    {
        scene = SceneManager.LoadSceneAsync(Scenename);
        scene.allowSceneActivation = false;

        while (!scene.isDone)
        {
            if (scene.progress >= 0.9f)
            {
                scene.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
