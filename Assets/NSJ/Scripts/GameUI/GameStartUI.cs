using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameUIs
{
    public class GameStartUI : BaseUI
    {
        [SerializeField] float _duration;

        private GameObject _gooseUI => GetUI("GooseBackGround");
        private GameObject _duckUI => GetUI("DuckBackGround");

        private Image _playerImage => GetUI<Image>("PlayerBody");

        private void Awake()
        {
            Bind();
        }

        public void SetActive(bool value)
        {
            GetUI("GameStartUI").SetActive(value);
            if (value == true)
            {
                StartCoroutine(DurationRoutine());
            }
        }

        /// <summary>
        /// 거위 화면 또는 오리 화면 띄우기
        /// 플레이어 색상 또한 지정
        /// </summary>
        public void SetUI(PlayerType type, Color color)
        {
            _gooseUI.SetActive(type == PlayerType.Goose);
            _duckUI.SetActive(type == PlayerType.Duck);

            _playerImage.color = color;
        }

        /// <summary>
        /// 지속시간동안만 나타남
        /// </summary>
        IEnumerator DurationRoutine()
        {
            yield return _duration.GetDelay();
            GetUI("GameStartUI").SetActive(false);
        }
    }
}

