using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameUIs
{
    public class ReportUI :BaseUI
    {
        [SerializeField] private float _duration;


        private Image _reporter => GetUI<Image>("PlayerHeadImage");
        private Image _corpse => GetUI<Image>("PlayerCorpseImage");
        private void Awake()
        {
            Bind();
        }

        public void SetActive(bool value)
        {
            GetUI("CorpseReportUI").SetActive(value);
            if (value == true)
            {
                StartCoroutine(DurationRoutine());
            }
        }

        /// <summary>
        /// 색 지정
        /// </summary>
        /// <param name="reporterColor"></param>
        /// <param name="corpseColor"></param>
        public void SetColor(Color reporterColor, Color corpseColor)
        {
            _reporter.color = reporterColor;
            _corpse.color = corpseColor;
        }

        /// <summary>
        /// 지속시간동안만 나타남
        /// </summary>
        IEnumerator DurationRoutine()
        {
            yield return _duration.GetDelay();
            GetUI("CorpseReportUI").SetActive(false);
        }
    }
}

