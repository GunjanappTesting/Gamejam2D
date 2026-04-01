using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquationSolverGameController : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float totalGameTime = 30f;
    [SerializeField] private int minStartValue = 1;
    [SerializeField] private int maxStartValue = 9;
    [SerializeField] private int minOperand = 1;
    [SerializeField] private int maxOperand = 9;
    [SerializeField] private int minOperationsPerEquation = 2;
    [SerializeField] private int maxOperationsPerEquation = 4;

    [Header("UI References")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text timerText;
    [SerializeField] private Text equationText;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private Text[] answerButtonTexts;
    [SerializeField] private GameObject endPopup;
    [SerializeField] private Text endTitleText;
    [SerializeField] private Text finalScoreText;
    [SerializeField] private Button restartButton;

    private readonly List<int> currentChoices = new List<int>(3);
    private readonly Dictionary<Button, Coroutine> buttonTapAnimations = new Dictionary<Button, Coroutine>();

    private AudioSource audioSource;
    private AudioClip tapClip;
    private AudioClip correctClip;
    private AudioClip wrongClip;
    private Font uiFont;
    private ParticleSystem correctAnswerParticles;
    private int correctAnswer;
    private int score;
    private float remainingTime;
    private bool isGameOver;

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        EnsureMainCamera();
        EnsureUi();
        EnsureAudio();
        EnsureCorrectAnswerParticles();
        ConfigureButtons();
        StartNewGame();
    }

    private void Update()
    {
        if (isGameOver)
        {
            return;
        }

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            UpdateHud();
            EndGame("Time Over");
            return;
        }

        UpdateHud();
    }

    public void RestartGame()
    {
        AnimateButtonTap(restartButton);
        PlaySound(tapClip, 0.8f);
        StartNewGame();
    }

    private void EnsureMainCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            mainCamera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
        }

        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        mainCamera.backgroundColor = new Color32(245, 247, 255, 255);
        mainCamera.orthographic = true;
        mainCamera.transform.position = new Vector3(0f, 0f, -10f);
    }

    private void EnsureUi()
    {
        if (scoreText != null && timerText != null && equationText != null && endPopup != null)
        {
            return;
        }

        uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        if (EventSystem.current == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("EquationSolverCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 1f;
        }

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        CreateStretchPanel("Background", canvasRect, new Color32(245, 247, 255, 255));
        CreatePanel("HeaderBand", canvasRect, new Color32(101, 119, 255, 255), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, 220f), Vector2.zero);

        CreateText("Title", canvasRect, "Equation Solver", 72, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(720f, 120f), new Vector2(0f, -100f));

        scoreText = CreateText("ScoreText", canvasRect, "Score: 0", 46, FontStyle.Bold, new Color32(31, 41, 55, 255), TextAnchor.MiddleLeft,
            new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(360f, 80f), new Vector2(50f, -270f));

        timerText = CreateText("TimerText", canvasRect, "Time: 30", 46, FontStyle.Bold, new Color32(220, 38, 38, 255), TextAnchor.MiddleRight,
            new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(360f, 80f), new Vector2(-50f, -270f));

        CreateText("InstructionText", canvasRect, "Select the correct answer", 48, FontStyle.Bold, new Color32(71, 85, 105, 255), TextAnchor.MiddleCenter,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(760f, 90f), new Vector2(0f, -420f));

        RectTransform equationCard = CreatePanel("EquationCard", canvasRect, Color.white, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(860f, 260f), new Vector2(0f, -620f));
        equationText = CreateText("EquationText", equationCard, "8 + 2 - 1", 78, FontStyle.Bold, new Color32(30, 41, 59, 255), TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(760f, 180f), Vector2.zero);

        answerButtons = new Button[3];
        answerButtonTexts = new Text[3];
        float[] buttonX = { -260f, 0f, 260f };
        for (int i = 0; i < 3; i++)
        {
            Button button = CreateButton("AnswerButton" + (i + 1), canvasRect, new Color32(79, 70, 229, 255), new Vector2(220f, 220f), new Vector2(buttonX[i], -1030f));
            Text label = CreateText("Label", button.GetComponent<RectTransform>(), "0", 64, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(180f, 120f), Vector2.zero);

            answerButtons[i] = button;
            answerButtonTexts[i] = label;
        }

        endPopup = CreateStretchPanel("EndPopup", canvasRect, new Color(0f, 0f, 0f, 0.55f)).gameObject;
        RectTransform popupCard = CreatePanel("PopupCard", endPopup.GetComponent<RectTransform>(), Color.white, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(760f, 520f), Vector2.zero);

        endTitleText = CreateText("EndTitle", popupCard, "Game Over", 70, FontStyle.Bold, new Color32(30, 41, 59, 255), TextAnchor.MiddleCenter,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(620f, 100f), new Vector2(0f, -120f));

        finalScoreText = CreateText("FinalScore", popupCard, "Score: 0", 56, FontStyle.Bold, new Color32(71, 85, 105, 255), TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(520f, 100f), new Vector2(0f, -10f));

        restartButton = CreateButton("RestartButton", popupCard, new Color32(16, 185, 129, 255), new Vector2(340f, 110f), new Vector2(0f, -405.5f));
        CreateText("RestartLabel", restartButton.GetComponent<RectTransform>(), "Play Again", 46, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(260f, 70f), Vector2.zero);

        endPopup.SetActive(false);
    }

    private void ConfigureButtons()
    {
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
        }

        if (answerButtons == null)
        {
            return;
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int capturedIndex = i;
            Button button = answerButtons[i];
            if (button == null)
            {
                continue;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnAnswerSelected(capturedIndex));
        }
    }

    private void StartNewGame()
    {
        score = 0;
        remainingTime = totalGameTime;
        isGameOver = false;

        if (endPopup != null)
        {
            endPopup.SetActive(false);
        }

        SetAnswerButtonsInteractable(true);
        GenerateNextEquation();
        UpdateHud();
    }

    private void GenerateNextEquation()
    {
        int currentValue = Random.Range(minStartValue, maxStartValue + 1);
        int operationCount = Random.Range(minOperationsPerEquation, maxOperationsPerEquation + 1);
        string equation = currentValue.ToString();

        for (int i = 0; i < operationCount; i++)
        {
            List<(char sign, int operand)> validMoves = new List<(char sign, int operand)>();

            for (int operand = minOperand; operand <= maxOperand; operand++)
            {
                int additionResult = currentValue + operand;
                if (additionResult >= 1 && additionResult <= 9)
                {
                    validMoves.Add(('+', operand));
                }

                int subtractionResult = currentValue - operand;
                if (subtractionResult >= 1 && subtractionResult <= 9)
                {
                    validMoves.Add(('-', operand));
                }
            }

            if (validMoves.Count == 0)
            {
                break;
            }

            (char sign, int operand) selectedMove = validMoves[Random.Range(0, validMoves.Count)];
            if (selectedMove.sign == '+')
            {
                currentValue += selectedMove.operand;
            }
            else
            {
                currentValue -= selectedMove.operand;
            }

            equation += " " + selectedMove.sign + " " + selectedMove.operand;
        }

        correctAnswer = currentValue;
        if (equationText != null)
        {
            equationText.text = equation;
        }

        BuildChoices();
        BindChoicesToButtons();
    }

    private void BuildChoices()
    {
        currentChoices.Clear();
        currentChoices.Add(correctAnswer);

        while (currentChoices.Count < 3)
        {
            int wrongAnswer = Random.Range(1, 10);
            if (!currentChoices.Contains(wrongAnswer))
            {
                currentChoices.Add(wrongAnswer);
            }
        }

        for (int i = 0; i < currentChoices.Count; i++)
        {
            int swapIndex = Random.Range(i, currentChoices.Count);
            int temp = currentChoices[i];
            currentChoices[i] = currentChoices[swapIndex];
            currentChoices[swapIndex] = temp;
        }
    }

    private void BindChoicesToButtons()
    {
        if (answerButtons == null)
        {
            return;
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            bool hasChoice = i < currentChoices.Count;
            if (answerButtons[i] != null)
            {
                answerButtons[i].gameObject.SetActive(hasChoice);
            }

            if (hasChoice && answerButtonTexts != null && i < answerButtonTexts.Length && answerButtonTexts[i] != null)
            {
                answerButtonTexts[i].text = currentChoices[i].ToString();
            }
        }
    }

    private void OnAnswerSelected(int answerIndex)
    {
        if (isGameOver || answerIndex < 0 || answerIndex >= currentChoices.Count)
        {
            return;
        }

        Button tappedButton = null;
        if (answerButtons != null && answerIndex < answerButtons.Length)
        {
            tappedButton = answerButtons[answerIndex];
            AnimateButtonTap(tappedButton);
        }

        PlaySound(tapClip, 0.7f);

        if (currentChoices[answerIndex] == correctAnswer)
        {
            score++;
            remainingTime = totalGameTime;
            PlaySound(correctClip, 0.9f);
            PlayCorrectAnswerParticles(tappedButton);
            GenerateNextEquation();
            UpdateHud();
            return;
        }

        PlaySound(wrongClip, 1f);
        EndGame("Wrong Answer");
    }

    private void EndGame(string title)
    {
        isGameOver = true;
        SetAnswerButtonsInteractable(false);

        if (endPopup != null)
        {
            endPopup.SetActive(true);
        }

        if (endTitleText != null)
        {
            endTitleText.text = title;
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Score: " + score;
        }
    }

    private void SetAnswerButtonsInteractable(bool interactable)
    {
        if (answerButtons == null)
        {
            return;
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] != null)
            {
                answerButtons[i].interactable = interactable;
            }
        }
    }

    private void UpdateHud()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.CeilToInt(remainingTime);
        }
    }

    private void EnsureAudio()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;

        if (tapClip == null)
        {
            tapClip = CreateToneClip("TapClip", 720f, 0.05f, 0.06f);
        }

        if (correctClip == null)
        {
            correctClip = CreateToneClip("CorrectClip", 980f, 0.12f, 0.08f);
        }

        if (wrongClip == null)
        {
            wrongClip = CreateToneClip("WrongClip", 260f, 0.2f, 0.1f);
        }
    }

    private void EnsureCorrectAnswerParticles()
    {
        if (correctAnswerParticles != null)
        {
            return;
        }

        GameObject particlesObject = new GameObject("CorrectAnswerParticles");
        particlesObject.transform.SetParent(transform, false);
        correctAnswerParticles = particlesObject.AddComponent<ParticleSystem>();

        var main = correctAnswerParticles.main;
        main.playOnAwake = false;
        main.loop = false;
        main.duration = 0.8f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.45f, 0.85f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(2.8f, 5.6f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.12f, 0.24f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, 360f);
        Gradient startGradient = new Gradient();
        startGradient.SetKeys(
            new[]
            {
                new GradientColorKey(new Color32(255, 214, 10, 255), 0f),
                new GradientColorKey(new Color32(56, 189, 248, 255), 0.35f),
                new GradientColorKey(new Color32(167, 139, 250, 255), 0.7f),
                new GradientColorKey(new Color32(34, 197, 94, 255), 1f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            });
        main.startColor = new ParticleSystem.MinMaxGradient(startGradient);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 0.35f;
        main.maxParticles = 60;

        var emission = correctAnswerParticles.emission;
        emission.enabled = true;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[]
        {
            new ParticleSystem.Burst(0f, 30)
        });

        var shape = correctAnswerParticles.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.15f;
        shape.arc = 360f;

        var colorOverLifetime = correctAnswerParticles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient colorGradient = new Gradient();
        colorGradient.SetKeys(
            new[]
            {
                new GradientColorKey(new Color32(255, 238, 88, 255), 0f),
                new GradientColorKey(new Color32(244, 114, 182, 255), 0.45f),
                new GradientColorKey(new Color32(59, 130, 246, 255), 1f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0.9f, 0.65f),
                new GradientAlphaKey(0f, 1f)
            });
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(colorGradient);

        var sizeOverLifetime = correctAnswerParticles.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(0.35f, 1.2f),
            new Keyframe(1f, 0f));
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        var rotationOverLifetime = correctAnswerParticles.rotationOverLifetime;
        rotationOverLifetime.enabled = true;
        rotationOverLifetime.z = new ParticleSystem.MinMaxCurve(-240f, 240f);

        var trails = correctAnswerParticles.trails;
        trails.enabled = true;
        trails.mode = ParticleSystemTrailMode.PerParticle;
        trails.dieWithParticles = true;
        trails.lifetime = 0.18f;
        trails.widthOverTrail = 0.35f;
        trails.colorOverLifetime = new ParticleSystem.MinMaxGradient(new Color32(255, 255, 255, 180));

        var renderer = correctAnswerParticles.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.sortMode = ParticleSystemSortMode.YoungestInFront;
        renderer.alignment = ParticleSystemRenderSpace.View;
    }

    private void PlayCorrectAnswerParticles(Button sourceButton)
    {
        if (correctAnswerParticles == null)
        {
            return;
        }

        Vector3 spawnPosition = new Vector3(0f, -2.6f, 0f);
        Camera mainCamera = Camera.main;

        if (sourceButton != null && mainCamera != null)
        {
            Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(null, sourceButton.transform.position);
            screenPoint.z = Mathf.Abs(mainCamera.transform.position.z);
            spawnPosition = mainCamera.ScreenToWorldPoint(screenPoint);
            spawnPosition.z = 0f;
        }

        correctAnswerParticles.transform.position = spawnPosition;
        correctAnswerParticles.Clear();
        correctAnswerParticles.Play();
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (audioSource == null || clip == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip, volume);
    }

    private void AnimateButtonTap(Button button)
    {
        if (button == null)
        {
            return;
        }

        if (buttonTapAnimations.TryGetValue(button, out Coroutine runningAnimation) && runningAnimation != null)
        {
            StopCoroutine(runningAnimation);
        }

        buttonTapAnimations[button] = StartCoroutine(PlayButtonTapAnimation(button.transform as RectTransform));
    }

    private IEnumerator PlayButtonTapAnimation(RectTransform target)
    {
        if (target == null)
        {
            yield break;
        }

        Vector3 startScale = Vector3.one;
        Vector3 pressedScale = Vector3.one * 0.88f;
        float duration = 0.08f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            target.localScale = Vector3.Lerp(startScale, pressedScale, progress);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            target.localScale = Vector3.Lerp(pressedScale, startScale, progress);
            yield return null;
        }

        target.localScale = startScale;
    }

    private AudioClip CreateToneClip(string clipName, float frequency, float duration, float amplitude)
    {
        int sampleRate = 44100;
        int sampleCount = Mathf.CeilToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float time = i / (float)sampleRate;
            float fade = Mathf.Clamp01(1f - (i / (float)sampleCount));
            samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * time) * amplitude * fade;
        }

        AudioClip clip = AudioClip.Create(clipName, sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private RectTransform CreateStretchPanel(string objectName, RectTransform parent, Color color)
    {
        GameObject panelObject = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        panelObject.transform.SetParent(parent, false);

        RectTransform rectTransform = panelObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Image image = panelObject.GetComponent<Image>();
        image.color = color;
        return rectTransform;
    }

    private RectTransform CreatePanel(string objectName, RectTransform parent, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 sizeDelta, Vector2 anchoredPosition)
    {
        GameObject panelObject = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        panelObject.transform.SetParent(parent, false);

        RectTransform rectTransform = panelObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = pivot;
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.anchoredPosition = anchoredPosition;

        Image image = panelObject.GetComponent<Image>();
        image.color = color;
        return rectTransform;
    }

    private Text CreateText(string objectName, RectTransform parent, string value, int fontSize, FontStyle fontStyle, Color color, TextAnchor alignment,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 sizeDelta, Vector2 anchoredPosition)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = pivot;
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.anchoredPosition = anchoredPosition;

        Text text = textObject.GetComponent<Text>();
        text.font = uiFont;
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.color = color;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return text;
    }

    private Button CreateButton(string objectName, RectTransform parent, Color color, Vector2 sizeDelta, Vector2 anchoredPosition)
    {
        GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 1f);
        rectTransform.anchorMax = new Vector2(0.5f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.anchoredPosition = anchoredPosition;

        Image image = buttonObject.GetComponent<Image>();
        image.color = color;

        Button button = buttonObject.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = color * 1.08f;
        colors.pressedColor = color * 0.9f;
        colors.selectedColor = color;
        colors.disabledColor = new Color(color.r, color.g, color.b, 0.45f);
        button.colors = colors;
        button.targetGraphic = image;
        return button;
    }
}
