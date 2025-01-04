using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class StepManager : MonoBehaviour
    {
        class Step
        {
            [SerializeField]
            public GameObject stepObject;

            [SerializeField]
            public string buttonText;
        }

        [SerializeField]
        public TextMeshProUGUI m_StepButtonTextField;
        [SerializeField]
        public Transform m_StepIndicator;
        [SerializeField]
        public GameObject m_StepObject;
        [SerializeField]
        public GameObject m_DotObject;
        
        List<Step> m_StepList = new List<Step>();
        List<GameObject> m_StepIndicatorList = new List<GameObject>();

        int m_CurrentStepIndex = 0;

        public void AddStep()
        {
            GameObject newStep = Instantiate(m_StepObject, transform);
            newStep.name = $"Card {m_StepList.Count + 1}";
            newStep.transform.SetSiblingIndex(m_StepIndicator.GetSiblingIndex()-1);
            GameObject newDot = Instantiate(m_DotObject, m_StepIndicator);
            newDot.name = $"Dot {m_StepList.Count + 1}";
            GameObject activeDot = newDot.transform.GetChild(0).gameObject;
            m_StepIndicatorList.Add(activeDot);
            if (m_StepList.Count > 0)
            {
                newStep.SetActive(false);
                activeDot.SetActive(false);
            }

            m_StepList.Add(new Step { stepObject = newStep, buttonText = $"Step {m_StepList.Count + 1}" });
        }

        public void Next()
        {
            m_StepList[m_CurrentStepIndex].stepObject.SetActive(false);
            m_StepIndicatorList[m_CurrentStepIndex].SetActive(false);
            m_CurrentStepIndex = (m_CurrentStepIndex + 1) % m_StepList.Count;
            m_StepList[m_CurrentStepIndex].stepObject.SetActive(true);
            m_StepIndicatorList[m_CurrentStepIndex].SetActive(true);
            m_StepButtonTextField.text = m_StepList[m_CurrentStepIndex].buttonText;
        }
    }
}