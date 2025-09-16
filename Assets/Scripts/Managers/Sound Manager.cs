using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioDatabaseSO audioDatabase;

    private void Start()
    {
        audioDatabase.Get("button_click");

    }
}
