using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public struct TagMiunQA
{
    public string Question;
    public string Answer;
}

public class TagMiun : MonoBehaviour
{
    [SerializeField] private Transform _qAParent;
    [SerializeField] private List<GameObject> _qAGroupsList = new List<GameObject>();

    private List<TagMiunQA> _tagMiunQAsList = new List<TagMiunQA>();
    [SerializeField] private List<string> questions = new List<string>();
    [SerializeField] private List<string> answers = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        GetAllQAGroups(_qAParent);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Filling The List With All QAGroups
    private void GetAllQAGroups(Transform QAparent)
    {
        for (int a = 0; a < QAparent.childCount; a++)
        {
            Transform tempT = QAparent.GetChild(a);
            for (int b = 0; b < tempT.childCount; b++)
            {
                if (tempT.GetChild(b).CompareTag("QAGroup"))
                {
                    _qAGroupsList.Add(tempT.GetChild(b).gameObject);
                }
            }
        }
    }

    // Submit Button Method
    public void SubmitAnswers()
    {
        _tagMiunQAsList.Clear();
        foreach (GameObject QAGroup in _qAGroupsList)
        {
            // Adding The Question
            TagMiunQA tempStruct = new TagMiunQA();
            for (int i = 0; i < QAGroup.transform.childCount; i++)
            {
                if (QAGroup.transform.GetChild(i).name == "QuestionText")
                {
                    tempStruct.Question = QAGroup.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text;
                }
            }

            // Adding The Answers
            int counter0 = 0;
            for (int i = 0; i < QAGroup.transform.childCount; i++)
            {
                // If Answer Type Is "InputField"
                if (QAGroup.transform.GetChild(i).GetComponent<TMP_InputField>() != null && QAGroup.transform.GetChild(i).GetComponent<TMP_InputField>().text != "")
                {
                    if (counter0 == 0 && QAGroup.transform.childCount - 1 == i)
                    {
                        tempStruct.Answer += QAGroup.transform.GetChild(i).GetComponent<TMP_InputField>().text;
                    }
                    else if (QAGroup.transform.childCount - 1 == i)
                    {
                        tempStruct.Answer += QAGroup.transform.GetChild(i).GetComponent<TMP_InputField>().text + ".";
                    }
                    else
                    {
                        tempStruct.Answer += QAGroup.transform.GetChild(i).GetComponent<TMP_InputField>().text + ", ";
                    }
                    counter0++;
                }

                // If Answer Type Is "Radio"
                else if (QAGroup.transform.GetChild(i).GetComponent<ToggleGroup>() != null)
                {
                    Transform tempRadioParent = QAGroup.transform.GetChild(i).transform;
                    for (int y = 0; y < tempRadioParent.childCount; y++)
                    {
                        if (tempRadioParent.GetChild(y).GetComponent<Toggle>().isOn)
                        {
                            tempStruct.Answer = tempRadioParent.GetChild(y).GetChild(1).GetComponent<Text>().text;
                        }
                    }
                }

                // If Answer Type Is "Checkbox"
                else if (QAGroup.transform.GetChild(i).GetComponent<TMP_InputField>() == null && QAGroup.transform.GetChild(i).GetComponent<ToggleGroup>() == null)
                {
                    if (QAGroup.transform.GetChild(i).GetComponentInChildren<Toggle>() != null)
                    {
                        Transform tempCheckboxParent = QAGroup.transform.GetChild(i).transform;
                        for (int z = 0; z < tempCheckboxParent.childCount; z++)
                        {
                            if (tempCheckboxParent.GetChild(z).GetComponent<Toggle>().isOn)
                            {
                                if (counter0 == 0 && tempCheckboxParent.childCount - 1 == z)
                                {
                                    tempStruct.Answer += tempCheckboxParent.GetChild(z).GetChild(1).GetComponent<Text>().text;
                                }
                                else if (tempCheckboxParent.childCount - 1 == z)
                                {
                                    tempStruct.Answer += tempCheckboxParent.GetChild(z).GetChild(1).GetComponent<Text>().text + ".";
                                }
                                else
                                {
                                    tempStruct.Answer += tempCheckboxParent.GetChild(z).GetChild(1).GetComponent<Text>().text + ", ";
                                }
                                counter0++;
                            }
                        }
                    }
                }
            }


            // Adding The TempStruct To The List of QAs
            _tagMiunQAsList.Add(tempStruct);
        }


        questions.Clear();
        answers.Clear();
        // Updating Viewable Lists
        foreach (var item in _tagMiunQAsList)
        {
            questions.Add(item.Question);
            answers.Add(item.Answer);
        }
    }
}
