using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using YG;

public class GameScript : MonoBehaviour
{
    public Sprite[] windowBackgrounds;
    public QuestionList[] questions;
    public Text[] answersText;
    //public SpriteRenderer spriteWindow;
    public Image image;
    public Image imageBackground;
    public Button[] answersButtons = new Button[3];
    public GameObject windowQuestionPanel;
    public GameObject windowEndLevelPanel;
    public GameObject QuestionPanel;
    public Sprite[] TrueFalseIcons = new Sprite[2];
    public Image TrueFalseIcon;
    public Text TrueFalseText;
    [SerializeField] private Text qText;

    private ScoreHandler _scoreHandler;

    private int randomQuestion;
    private bool defaultColor, trueColor, falseColor;

    List<object> questionList;
    QuestionList currentQuestion;

    private void Awake()
    {
        _scoreHandler = GetComponent<ScoreHandler>();
    }

    private void Start()
    {
        windowEndLevelPanel.SetActive(false);
    }

    private void Update()
    {
        if (defaultColor)
            windowQuestionPanel.GetComponent<Image>().color =
                Color.Lerp(windowQuestionPanel.GetComponent<Image>().color, new Color(231 / 255.0f, 78 / 255.0f, 62 / 255.0f), 8 * Time.deltaTime);
        if (trueColor)
            windowQuestionPanel.GetComponent<Image>().color =
                Color.Lerp(windowQuestionPanel.GetComponent<Image>().color, new Color(80 / 255.0f, 226 / 255.0f, 29 / 255.0f), 8 * Time.deltaTime);
        if (falseColor)
            windowQuestionPanel.GetComponent<Image>().color =
                Color.Lerp(windowQuestionPanel.GetComponent<Image>().color, new Color(253 / 255.0f, 21 / 255.0f, 16 / 255.0f), 8 * Time.deltaTime);
    }

    public void OnClickPlay()
    {
        questionList = new List<object>(questions);
        QuestionGenerate();
        if (!windowQuestionPanel.GetComponent<Animator>().enabled)
        {
            windowQuestionPanel.GetComponent<Animator>().enabled = true;
        }
        else
        {
            windowQuestionPanel.GetComponent<Animator>().SetTrigger("ShowWinQuestion");
        }
    }

    private void QuestionGenerate()
    {
        if (questionList.Count > 0)
        {
            randomQuestion = Random.Range(0, questionList.Count);

            currentQuestion = questionList[randomQuestion] as QuestionList;
            //qText.gameObject.SetActive(true);
            qText.text = currentQuestion.question;
            //qText.gameObject.GetComponent<Animator>().SetTrigger("On");
            QuestionPanel.gameObject.GetComponent<Animator>().SetTrigger("On");

            image.GetComponent<Image>().sprite = currentQuestion.sprite;
            imageBackground.GetComponent<Image>().sprite = windowBackgrounds[Random.Range(0, windowBackgrounds.Length)];

            List<string> answers = new List<string>(currentQuestion.answers);

            for (int i = 0; i < currentQuestion.answers.Length; i++)
            {
                int random = Random.Range(0, answers.Count);

                answersText[i].text = answers[random];
                answers.RemoveAt(random);
            }

            StartCoroutine(IanimButtons());
        }
        else
        {
            _scoreHandler.ShowScoreEndLevel();
            _scoreHandler.InstallAndShowRecord();
            windowEndLevelPanel.SetActive(true);
            Debug.Log("EndGame");
        }
    }

    private IEnumerator IanimButtons()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < answersButtons.Length; i++)
            answersButtons[i].interactable = false;

        int a = 0;

        while (a < answersButtons.Length)
        {
            if (!answersButtons[a].gameObject.activeSelf)
                answersButtons[a].gameObject.SetActive(true);
            else
            {
                answersButtons[a].gameObject.GetComponent<Animator>().SetTrigger("On");
                //answersButtons[a].gameObject.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 2f, 0, 3f);
                Debug.Log("AnimOn");
            }
            a++;
            yield return new WaitForSeconds(1);
        }

        for (int i = 0; i < answersButtons.Length; i++)
            answersButtons[i].interactable = true;

        image.gameObject.SetActive(true);
        //QuestionPanel.gameObject.SetActive(true);

        yield break;
    }

    private IEnumerator TrueOrFalse(bool check)
    {
        defaultColor = false;
        for (int i = 0; i < answersButtons.Length; i++)
            answersButtons[i].interactable = false;

        yield return new WaitForSeconds(1);

        for (int i = 0; i < answersButtons.Length; i++)
            answersButtons[i].gameObject.GetComponent<Animator>().SetTrigger("Out"); //Отключение Кнопок
        //qText.gameObject.GetComponent<Animator>().enabled = true;
        //qText.gameObject.GetComponent<Animator>().SetTrigger("Out"); //Отключение Вопроса
        QuestionPanel.gameObject.GetComponent<Animator>().SetTrigger("Off");
        //qText.gameObject.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 2f, 0, 3f);

        image.gameObject.SetActive(false);
        //QuestionPanel.gameObject.SetActive(false);

        yield return new WaitForSeconds(1);

        if (!TrueFalseIcon.gameObject.activeSelf)
            TrueFalseIcon.gameObject.SetActive(true);
        else
        {
            TrueFalseIcon.gameObject.GetComponent<Animator>().SetTrigger("In"); //Отключение иконки
            Debug.Log("AnimOn");
        }

        YandexGame.FullscreenShow();

        if (check)
        {
            trueColor = true;
            TrueFalseIcon.sprite = TrueFalseIcons[0];
            TrueFalseText.text = "Верно";
            yield return new WaitForSeconds(1);

            _scoreHandler.IsTrueAnswer();

            TrueFalseIcon.gameObject.GetComponent<Animator>().SetTrigger("Out");
            questionList.RemoveAt(randomQuestion);
            QuestionGenerate();
            trueColor = false;
            defaultColor = true;

            if (questionList.Count <= 0)
            {
                _scoreHandler.ShowScoreEndLevel();
                _scoreHandler.InstallAndShowRecord();
                windowEndLevelPanel.SetActive(true);
            }

            yield break;
        }
        else
        {
            falseColor = true;
            TrueFalseIcon.sprite = TrueFalseIcons[1];
            TrueFalseText.text = "Неверно";
            yield return new WaitForSeconds(1);

            _scoreHandler.IsFalseAnswer();

            TrueFalseIcon.gameObject.GetComponent<Animator>().SetTrigger("Out");
            //windowQuestionPanel.GetComponent<Animator>().SetTrigger("offShowWinQuestion"); //Закончить игру при неправильном ответе
            questionList.RemoveAt(randomQuestion);
            QuestionGenerate();
            falseColor = false;
            defaultColor = true;


            if (questionList.Count <= 0)
            {
                _scoreHandler.ShowScoreEndLevel();
                _scoreHandler.InstallAndShowRecord();
                windowEndLevelPanel.SetActive(true);
            }

            //_scoreHandler.ShowScoreEndLevel();
            //_scoreHandler.InstallAndShowRecord();   //Закончить игру при неправильном ответе
            //windowEndLevelPanel.SetActive(true);

            yield break;
        }
    }

    public void AnswersBtn(int index)
    {
        if (answersText[index].text.ToString() == currentQuestion.answers[0])
        {
            StartCoroutine(TrueOrFalse(true));
            Debug.Log("Correct answer");
        }
        else
        {
            StartCoroutine(TrueOrFalse(false));
            Debug.Log("Fault answer");
        }
    }
}

[System.Serializable]
public class QuestionList
{
    public string question;
    public Sprite sprite;
    public string[] answers = new string[3];
}

