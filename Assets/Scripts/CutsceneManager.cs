using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public GameObject background;
    public GameObject cutscenePanel;
    public Image cutsceneImage;
    public Button nextButton;
    public Button skipButton;

    public Sprite[] cutsceneImages;
    private int currentIndex = 0;
    private bool isPlaying = false;
    public bool autoPlay = false;
    public float autoPlayDelay = 2f;

    // Code input UI elements
    public GameObject codePanel; // Panel containing input field and submit button
    public InputField codeInputField;
    public Button submitButton;
    public Text feedbackText;

    private string correctCode = "1775";
    public GameObject player;
    private Interactable thisObject;
    private bool requiresCode;

    private void Start()
    {
        cutscenePanel.SetActive(false);
        codePanel.SetActive(false);

        nextButton.onClick.AddListener(NextImage);
        skipButton.onClick.AddListener(CloseCutscene);
        submitButton.onClick.AddListener(CheckCode);
    }

    public void StartCutscene(Sprite[] images, bool shouldAutoPlay, bool needsCode = false, string requiredCode = "")
    {
        if (isPlaying) return;

        GameObject thisInteract = player.GetComponent<PickUp>().currentInteractable;
        Debug.Log("current Interactable" + thisInteract);
        thisObject = thisInteract.GetComponent<Interactable>();
        requiresCode = thisObject.needsCode;

        cutsceneImages = images;
        autoPlay = shouldAutoPlay;
        requiresCode = needsCode;
        correctCode = requiredCode;
        currentIndex = 0;
        Debug.Log("does it require code" + requiresCode);

        if (!requiresCode)
        {
            nextButton.gameObject.SetActive(true);
            cutsceneImage.sprite = cutsceneImages[currentIndex];

            cutscenePanel.SetActive(true);
            background.SetActive(true);

            StartCoroutine(FadeInAndEnable());
        }

        else
        {
            nextButton.gameObject.SetActive(false);
            codePanel.SetActive(true);
        }

        if (autoPlay && !requiresCode)
        {
            StartCoroutine(AutoPlayCutscene());
        }
    }

    private IEnumerator FadeInAndEnable()
    {
        yield return StartCoroutine(background.GetComponent<PanelFader>().FadePanel(1f, 2f));

        nextButton.gameObject.SetActive(!requiresCode);
        skipButton.gameObject.SetActive(true);
        cutsceneImage.gameObject.SetActive(true);

        // Show the code input only if required
        if (requiresCode)
        {
            codePanel.SetActive(true);
        }

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
        if (requiresCode) return; // Prevent skipping if code is required

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
        nextButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
        cutsceneImage.gameObject.SetActive(false);
        codePanel.SetActive(false);

        yield return StartCoroutine(background.GetComponent<PanelFader>().FadePanel(0f, 2f));

        cutscenePanel.SetActive(false);
        background.SetActive(false);
        isPlaying = false;
    }

    private void CheckCode()
    {
        if (codeInputField.text == correctCode)
        {
            feedbackText.text = "Correct!";
            codePanel.SetActive(false);
            requiresCode = false;
            nextButton.gameObject.SetActive(true); // Enable Next button once correct code is entered
        }
        else
        {
            feedbackText.text = "Incorrect Code! Try again.";
        }
    }
}

