using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Master : MonoBehaviour
{
    private GameObject _lostUi;
    private GameObject _wonUi;
    private Button _retryBtn;

    public bool GameActive { get; private set; }
    public bool IsLastLevel = false;

    void Start()
    {
        GameActive = true;
        _lostUi = GameObject.Find("GameOver");
        _wonUi = GameObject.Find("GameWon");

        var curLevel = int.Parse(new string(new[] {SceneManager.GetActiveScene().name.Last()}));
        var nextLevel = curLevel + 1;
        var nextLevelName = "level" + nextLevel;

        _lostUi.transform.FindChild("Retry").GetComponent<Button>().onClick.AddListener(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });

        if (!IsLastLevel)
        {
            _wonUi.transform.FindChild("Next").GetComponent<Button>().onClick.AddListener(() =>
            {
                SceneManager.LoadScene(nextLevelName);
            });
        }

        _lostUi.SetActive(false);
        _wonUi.SetActive(false);
    }

    public void PlayerLost()
    {
        GameActive = false;
        _lostUi.SetActive(true);
        _lostUi.transform.FindChild("Poseichan").GetComponent<Peek>().enabled = true;
    }

    public void PlayerWon()
    {
        if (IsLastLevel)
        {
            _wonUi.transform.FindChild("Next").gameObject.SetActive(false);
        }

        _wonUi.SetActive(true);
        _wonUi.transform.FindChild("Poseichan").GetComponent<Peek>().enabled = true;
    }
}
