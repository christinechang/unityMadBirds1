using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    private Enemy[] _enemies;
    private static int _highestLevel = 2;
    private static int _nextLevelIndex = 1; // static is global shared variable all levels share.

    private void OnEnable()
    {
        _enemies = FindObjectsOfType<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Enemy enemy in _enemies)
        {
            if (enemy != null)
                return;
        }
        _nextLevelIndex++;
        if (_nextLevelIndex <= _highestLevel)
        {
            string nextLevelName = "Level" + _nextLevelIndex;
            Debug.Log("You killed all enemies.   NextLevel:" + nextLevelName);
            SceneManager.LoadScene(nextLevelName);
        }
    }
}
