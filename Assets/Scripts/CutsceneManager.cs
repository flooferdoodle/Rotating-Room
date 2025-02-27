using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public GameObject background;
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

        // Hide UI elements until the fade-in is done
        nextButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
        cutsceneImage.gameObject.SetActive(false);

        cutsceneImage.sprite = cutsceneImages[currentIndex];

        cutscenePanel.SetActive(true);
        background.SetActive(true);
        // Start fade-in and enable UI after it finishes
        StartCoroutine(FadeInAndEnable());

        if (autoPlay)
        {
            StartCoroutine(AutoPlayCutscene());
        }
    }


    private IEnumerator FadeInAndEnable()
    {
        //background.SetActive(true); // Ensure background is visible before fading in

        // Fade in background first
        yield return StartCoroutine(background.GetComponent<PanelFader>().FadePanel(1f, 2f));

        // Now enable UI elements after background fade-in is complete
        nextButton.gameObject.SetActive(true);
        skipButton.gameObject.SetActive(true);
        cutsceneImage.gameObject.SetActive(true);

        isPlaying = true;
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
        StartCoroutine(FadeOutAndDisable());
    }

    private IEnumerator FadeOutAndDisable()
    {
        // Hide UI before fade-out starts
        nextButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
        cutsceneImage.gameObject.SetActive(false);

        yield return StartCoroutine(background.GetComponent<PanelFader>().FadePanel(0f, 2f));

        // Disable everything AFTER the fade-out is done
        cutscenePanel.SetActive(false);
        background.SetActive(false);
        isPlaying = false;
    }


}
