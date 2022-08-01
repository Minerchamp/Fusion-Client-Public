using System;
using UnityEngine;
using UnityEngine.UI;

namespace FusionClient.API.QM
{
    public class QMSlider
    {
        protected GameObject slider;
        protected GameObject label;
        protected Slider sliderComp;
        protected Text text;
        protected float minValue;
        protected float maxValue;
        protected float currentValue;

        public QMSlider(QMNestedButton location, float posX, float posY, string sliderLabel, float minValue, float maxValue, float defaultValue, Action<float> sliderAction, Color? labelColor = null)
        {
            Initialize(location.GetMenuObject().transform, posX, posY, sliderLabel, minValue, maxValue, defaultValue, sliderAction, labelColor);
        }

        public QMSlider(Transform location, float posX, float posY, string sliderLabel, float minValue, float maxValue, float defaultValue, Action<float> sliderAction, Color? labelColor = null)
        {
            Initialize(location, posX, posY, sliderLabel, minValue, maxValue, defaultValue, sliderAction, labelColor);
        }

        private void Initialize(Transform location, float posX, float posY, string sliderLabel, float minValue, float maxValue, float defaultValue, Action<float> sliderAction, Color? labelColor = null)
        {
            slider = UnityEngine.Object.Instantiate(APIStuff.GetSliderTemplate(), location);
            slider.transform.localScale = new Vector3(1, 1, 1);
            slider.name = $"{BlazesAPI.Identifier}-QMSlider-{APIStuff.RandomNumbers()}";

            label = UnityEngine.Object.Instantiate(GameObject.Find("UserInterface/MenuContent/Screens/Settings/AudioDevicePanel/LevelText"), slider.transform);
            label.name = "QMSlider-Label";
            label.transform.localScale = new Vector3(1, 1, 1);
            label.GetComponent<RectTransform>().sizeDelta = new Vector2(360, 50);
            label.GetComponent<RectTransform>().anchoredPosition = new Vector2(10.4f, 55);
            sliderComp = slider.GetComponent<Slider>();
            sliderComp.wholeNumbers = false;
            sliderComp.onValueChanged = new Slider.SliderEvent();
            sliderComp.onValueChanged.AddListener(sliderAction);
            sliderComp.onValueChanged.AddListener(new Action<float>(delegate (float f)
            {
                currentValue = f;
                slider.transform.Find("Fill Area/Label").GetComponent<Text>().text = $"{currentValue}/{maxValue}";
            }));

            text = label.GetComponent<Text>();
            text.resizeTextForBestFit = false;
            if (labelColor != null) SetLabelColor((Color)labelColor);

            SetLocation(new Vector2(posX, posY));
            SetLabelText(sliderLabel);
            SetValue(minValue, maxValue, defaultValue);

            BlazesAPI.allQMSliders.Add(this);
        }

        public void SetLocation(Vector2 location)
        {
            slider.GetComponent<RectTransform>().anchoredPosition = location;
        }

        public void SetLabelText(string label)
        {
            text.text = label;
        }

        public void SetLabelColor(Color color)
        {
            text.color = color;
        }

        public void SetValue(float min, float max, float current)
        {
            minValue = min;
            maxValue = max;
            currentValue = current;

            sliderComp.minValue = minValue;
            sliderComp.maxValue = maxValue;
            sliderComp.value = currentValue;
        }

        public GameObject GetGameObject()
        {
            return slider;
        }
    }
}
