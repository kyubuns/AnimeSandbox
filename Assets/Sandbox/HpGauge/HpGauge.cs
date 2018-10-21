using System;
using AnimeRx;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Sandbox
{
    public class HpGauge : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private RectTransform blue;
        [SerializeField] private RectTransform red;

        public void Start()
        {
            var hp = slider
                .OnValueChangedAsObservable()
                .ToReadOnlyReactiveProperty(1.0f);

            hp.Subscribe(x => Debug.Log(x)).AddTo(gameObject);

            // blueはすぐに反映
            var blueValue = 1.0f;
            hp.Select(x => Anime.Play(blueValue, x, Easing.InExpo(TimeSpan.FromSeconds(0.15f))))
                .Switch()
                .Subscribe(x =>
                {
                    blueValue = x;
                    var p = blue.anchoredPosition;
                    p.x = Mathf.Lerp(-blue.rect.width, 0.0f, x);
                    blue.anchoredPosition = p;
                })
                .AddTo(blue.gameObject);

            // redは遅れて反映 / 回復の場合はblueと同じ速度
            var redValue = 1.0f;
            hp.Select(x =>
                {
                    if (x < redValue) return Anime.Play(redValue, x, Easing.InQuint(TimeSpan.FromSeconds(1.0f)));
                    return Anime.Play(blueValue, x, Easing.InExpo(TimeSpan.FromSeconds(0.15f)));
                })
                .Switch()
                .Subscribe(x =>
                {
                    redValue = x;
                    var p = red.anchoredPosition;
                    p.x = Mathf.Lerp(-red.rect.width, 0.0f, x);
                    red.anchoredPosition = p;
                })
                .AddTo(red.gameObject);
        }
    }
}
