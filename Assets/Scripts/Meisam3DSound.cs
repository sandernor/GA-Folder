using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour
{
    AudioSource audioSource;
    float[] audioData;
    [SerializeField, Header("3Dsound")]
    float maxDistance = 200.0f;
    public GameObject player1;

    void Awake()
    {
        // Load an audio file  from a text file
        string audioFilePath = ReadAudioFilePathFromFile();
        audioSource = GetComponent<AudioSource>();
        //3D settings
        audioSource.spatialize = true;
        audioSource.spatialBlend = 1.0f; // 1.0 means full 3D spatialization
        audioSource.spatializePostEffects = true;
        audioSource.spatialize = true;
        audioSource.maxDistance = 400.0f;
        audioSource.dopplerLevel = 0.0f;
        GameObject player = GetComponent<GameObject>();

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
            int headerOffset = 44; // Assuming a standard WAV header size
            float[] audioData = new float[(fileBytes.Length - headerOffset) / 2];

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
        p1 = p1.Replace("/Assets", "/Assets/Audio/myfile.wav"); // replace the app address with the following

        return p1.ToString();

    }

    private void LateUpdate()
    {
        Vector3 playerPosition = player1.transform.position; // player position
        Vector3 soundPosition = transform.position; // source of audio // rotating cube

        float distance = Vector3.Distance(playerPosition, soundPosition);

        audioSource.volume = Mathf.Clamp01(1.0f - distance / maxDistance);   //  adjust volume based on distancex

    }
}
