using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class EndingScreenCode : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    private VisualElement blackScreen;
    void Start()
    {
        blackScreen = uiDocument.rootVisualElement.Q<VisualElement>("BlackScreen");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            blackScreen.style.display = DisplayStyle.Flex;
            blackScreen.style.opacity = 1f;
        }
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
