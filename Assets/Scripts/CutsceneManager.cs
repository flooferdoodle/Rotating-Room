using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    //non code cutscene
    public GameObject background;
    public GameObject cutscenePanel;
    public Image cutsceneImage;
    public Button nextButton;
    public Button skipButton;

    //code cutscene
    public GameObject background2;
    public GameObject cutscenePanel2;
    public Image cutsceneImage2;
    public Button submitButton;
    public Button closeButton;
    public TMP_InputField codeInputField;
    public TMP_Text feedbackText;
    public Button nextButton2;
    

    public Sprite[] cutsceneImages;
    private int currentIndex = 0;
    private bool isPlaying = false;
    public bool autoPlay = false;
    public string requiredCode = "false";
    public bool requiresCode = false;
    public float autoPlayDelay = 2f;

    private GameObject spawned;

    private void Start()
    {
        cutscenePanel.SetActive(false);
        cutsceneImage.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);

        cutscenePanel2.SetActive(false);
        cutsceneImage2.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        codeInputField.gameObject.SetActive(false);
        feedbackText.gameObject.SetActive(false);

        nextButton.onClick.AddListener(NextImage);
        nextButton2.onClick.AddListener(NextImage);
        skipButton.onClick.AddListener(CloseCutscene);
        submitButton.onClick.AddListener(CheckCode);
        closeButton.onClick.AddListener(CloseCutscene);
    }

    private void Update()
    {
        if(currentIndex == cutsceneImages.Length-1 && spawned!=null)
        {
            spawned.SetActive(true);

        }
        
    }

    public void StartCutscene(Sprite[] images, bool shouldAutoPlay, bool needsCode, string neededCode, GameObject spawn)
    {
        if (isPlaying) return;

        

        cutsceneImages = images;
        autoPlay = shouldAutoPlay;
        requiresCode = needsCode;
        requiredCode = neededCode;
        spawned = spawn;
        currentIndex = 0;
        nextButton2.gameObject.SetActive(false);

        //Debug.Log("does it require code" + requiresCode);

        if (!requiresCode)
        {
            cutsceneImage.sprite = cutsceneImages[currentIndex];

            cutscenePanel.SetActive(true);
            background.SetActive(true);

            StartCoroutine(FadeInAndEnable());
        }

        else
        {
            cutsceneImage2.sprite = cutsceneImages[currentIndex];
            cutscenePanel2.SetActive(true);
            background2.SetActive(true);
            StartCoroutine(FadeInAndEnable());
        }

        if (autoPlay && !requiresCode)
        {
            StartCoroutine(AutoPlayCutscene());
        }
    }

    private IEnumerator FadeInAndEnable()
    {
        if (!requiresCode)
        {
            yield return StartCoroutine(background.GetComponent<PanelFader>().FadePanel(1f, 0.5f));
        }
        if (requiresCode)
        {
            yield return StartCoroutine(background2.GetComponent<PanelFader>().FadePanel(1f, 0.5f));


        }

        if (!requiresCode)
        {
            nextButton.gameObject.SetActive(!requiresCode);
            skipButton.gameObject.SetActive(true);
            cutsceneImage.gameObject.SetActive(true);
           


        }

        // Show the code input only if required
        if (requiresCode)
        {
            cutsceneImage2.gameObject.SetActive(true);
            submitButton.gameObject.SetActive(true);
            closeButton.gameObject.SetActive(true);
            codeInputField.gameObject.SetActive(true);
            feedbackText.gameObject.SetActive(true);

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
        //if (requiresCode) return; // Prevent skipping if code is required

        if (currentIndex < cutsceneImages.Length - 1)
        {
            currentIndex++;
            cutsceneImage.sprite = cutsceneImages[currentIndex];
            if (requiresCode)
            {
                cutsceneImage2.sprite = cutsceneImages[currentIndex];
            }
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
        

        closeButton.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);
        nextButton2.gameObject.SetActive(false);
        cutsceneImage2.gameObject.SetActive(false);
        codeInputField.gameObject.SetActive(false);
        feedbackText.gameObject.SetActive(false);




        if (!requiresCode)
        {
            yield return StartCoroutine(background.GetComponent<PanelFader>().FadePanel(0f, 0.5f));
        }
        if (requiresCode)
        {
            yield return StartCoroutine(background2.GetComponent<PanelFader>().FadePanel(0f, 0.5f));
        }

        cutscenePanel.SetActive(false);
        cutscenePanel2.SetActive(false);
        background.SetActive(false);
        background2.SetActive(false);
        isPlaying = false;
    }

    private void CheckCode()
    {
        if (codeInputField.text == requiredCode)
        {
            feedbackText.text = "Correct!";
            requiresCode = true;
            submitButton.gameObject.SetActive(false);
            nextButton2.gameObject.SetActive(true);
        }
        else
        {
            feedbackText.text = "Incorrect Code! Try again.";
        }
    }
}

