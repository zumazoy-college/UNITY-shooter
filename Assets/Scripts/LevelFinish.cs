using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinish : MonoBehaviour
{
    public int nextLevelIndex = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Уровень пройден! Загрузка следующего...");
            SceneManager.LoadScene(nextLevelIndex);
        }
    }
}
