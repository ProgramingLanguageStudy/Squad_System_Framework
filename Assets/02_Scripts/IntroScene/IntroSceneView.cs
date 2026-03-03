using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroSceneView : MonoBehaviour
{
    [SerializeField] Button _playButton;

    public void Initialize()
    {
        GameManager.Instance.SaveManager.Load();
        _playButton.onClick.AddListener(() => SceneManager.LoadScene("Play"));
    }
}
