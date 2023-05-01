using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) 
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource.volume <= 0)
            {
                audioSource.volume = 0.75f;
            }
            else
            {
                audioSource.volume = 0;
            }
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
