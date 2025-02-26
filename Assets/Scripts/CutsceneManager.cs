using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public GameObject cutscenePanel; // UI Panel to hold images
    public Image cutsceneImage; // UI Image component to display images
    public Button nextButton;
    public Button skipButton;

    public Sprite[] cutsceneImages; // Array of images for the cutscene
    private int currentIndex = 0;
    private bool isPlaying = false;
    public bool autoPlay = false;
    public float autoPlayDelay = 2f; // Delay between images in autoplay mode

    private void Start()
    {
        // Hide UI elements initially
        cutscenePanel.SetActive(false);
        nextButton.onClick.AddListener(NextImage);
        skipButton.onClick.AddListener(CloseCutscene);
    }

    public void StartCutscene(Sprite[] images, bool shouldAutoPlay)
    {
        if (isPlaying) return;

        cutsceneImages = images;
        autoPlay = shouldAutoPlay;
        currentIndex = 0;
        cutscenePanel.SetActive(true);
        cutsceneImage.sprite = cutsceneImages[currentIndex];
        isPlaying = true;

        if (autoPlay)
        {
            StartCoroutine(AutoPlayCutscene());
        }
    }

    IEnumerator AutoPlayCutscene()
    {
        while (currentIndex < cutsceneImages.Length - 1)
        {
            yield return new WaitForSeconds(autoPlayDelay);
            NextImage();
        }
    }

    public void NextImage()
    {
        if (currentIndex < cutsceneImages.Length - 1)
        {
            currentIndex++;
            cutsceneImage.sprite = cutsceneImages[currentIndex];
        }
        else
        {
            CloseCutscene();
        }
    }

    public void CloseCutscene()
    {
        cutscenePanel.SetActive(false);
        isPlaying = false;
    }
}
