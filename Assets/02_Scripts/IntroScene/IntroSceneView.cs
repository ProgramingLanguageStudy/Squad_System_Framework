using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroSceneView : MonoBehaviour
{
    [SerializeField] Button _playButton;

    public void Initialize()
    { 
        _playButton.onClick.AddListener(() => SceneManager.LoadScene("Play"));
    }
}
