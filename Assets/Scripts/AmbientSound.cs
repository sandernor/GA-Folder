using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class AmbientSound : MonoBehaviour
{
    AudioSource audioSource;
    float[] audioData;

    void Awake()
    {
        string audioFilePath = ReadAudioFilePathFromFile();
        audioSource = GetComponent<AudioSource>();

        audioSource.spatialize = false;
        audioSource.volume = 0.5f;

        if (!string.IsNullOrEmpty(audioFilePath))
        {
            audioData = LoadAudioData(audioFilePath);
            PlayAudio();
        }
        else
        {
            Debug.LogError("Failed to read audio file path.");
        }
    }

    void PlayAudio()
    {
        if (audioData != null && audioData.Length > 0)
        {
            audioSource.clip = AudioClip.Create("LoadedClip", audioData.Length, 1, 44100, false);
            audioSource.clip.SetData(audioData, 0);
            audioSource.Play(0);
        }
        else
        {
            Debug.LogError("Failed to load audio data.");
        }
    }

    float[] LoadAudioData(string filePath)
    {
        try
        {
            // Load WAV file 
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            int headerOffset = 32; // Assuming a standard WAV header size
            float[] audioData = new float[(fileBytes.Length - headerOffset) / 2];

            Debug.Log(audioData.Length);

            for (int i = 0; i < audioData.Length; i++)
            {
                audioData[i] = (short)(fileBytes[headerOffset + i * 2] | (fileBytes[headerOffset + i * 2 + 1] << 8)) / 32768.0f; // refer to footnotes Part1
            }

            return audioData;
        }

        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to load audio data: {ex.Message}");
            return null;
        }
    }

    string ReadAudioFilePathFromFile()
    {

        string p1 = Application.dataPath; // get the path 
        p1 = p1.Replace("/Assets", "/Assets/Audio/wind.mp3"); // replace the app address with the following

        return p1.ToString();

    }

    private void LateUpdate()
    {

    }
}
