using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject touchFeedbackUI;
    [SerializeField, Min(0f)] private float feedbackDuration = 0.5f;

    private Coroutine feedbackRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (touchFeedbackUI != null)
        {
            touchFeedbackUI.SetActive(false);
        }
    }

    private void OnEnable()
    {
        PlayerTouchable.Touched += OnTouched;
    }

    private void OnDisable()
    {
        PlayerTouchable.Touched -= OnTouched;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnTouched(PlayerTouchable touchable)
    {
        if (touchFeedbackUI == null)
        {
            return;
        }

        if (feedbackRoutine != null)
        {
            StopCoroutine(feedbackRoutine);
        }

        feedbackRoutine = StartCoroutine(ShowFeedback());
    }

    private IEnumerator ShowFeedback()
    {
        touchFeedbackUI.SetActive(true);
        yield return new WaitForSeconds(feedbackDuration);
        touchFeedbackUI.SetActive(false);
        feedbackRoutine = null;
    }
}
