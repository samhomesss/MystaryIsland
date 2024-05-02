using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weather : MonoBehaviour
{
    public enum WeatherState
    {
        SUNNY = 0,
        CLOUDY = 1,
        RAINY,
    }

    private Sprite[] weatherImages;
    public Image imageSprite;
    public AudioSource audioSource;

    public static float WEATHER_TIME = 5.0f; // 날씨 시간 상수.
    private float timer = 0;

    WeatherState currentState;

    void Start()
    {
        imageSprite = GameObject.Find("Weather").GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        weatherImages = Resources.LoadAll<Sprite>("UI/Weather");

        int num = Random.Range(0, 2);
        currentState = (WeatherState)num;
        imageSprite.sprite = findImage(currentState.ToString());

        Color color = imageSprite.color;
        color.a = 1.0f;
        imageSprite.color = color;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > WEATHER_TIME)
        {
            timer = 0;
            currentState = changeWeatherState(currentState);
            imageSprite.GetComponent<Image>().sprite = findImage(currentState.ToString());

            if (currentState == WeatherState.RAINY)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            else
                audioSource.Stop();
        }
    }

    public WeatherState changeWeatherState(WeatherState state)
    {
        int num = Random.Range(0, 3);
        if (num == (int)state)
            return state;

        return (WeatherState)num;
    }

    private Sprite findImage(string name)
    {
        if (name == null)
            Debug.Log("이름없음");

        for (int i = 0; i < weatherImages.Length; i++)
        {
            if (weatherImages[i].name == name)
            {
                return weatherImages[i] as Sprite;
            }
        }

        return null;
    }

    public WeatherState GetWeatherState()
    {
        return currentState;
    }
}
