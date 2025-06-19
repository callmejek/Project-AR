using System;
using System.Collections;
using System.Collections.Generic;
using Niantic.Lightship.AR.ObjectDetection;
using UnityEngine;

public class ObjectDetectionSample : MonoBehaviour
{
    [SerializeField] private float _probabilityThreshold = .5f;

    [SerializeField] private ARObjectDetectionManager _objectDetectionManager;

    private Color[] colors = new[]
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta,
        Color.cyan,
        Color.white,
        Color.black
    };

    [SerializeField] private DrawRect _drawRect;

    private Canvas _canvas;

    private void Awake()
    {
        _canvas = FindObjectOfType<Canvas>();
    }

    private void Start()
    {
        _objectDetectionManager.enabled = true;
        _objectDetectionManager.MetadataInitialized += ObjectDetectionManagerOnMetadataInitialized;
        SetObjectDetectionChannels();
    }

    private void OnDestroy()
    {
        _objectDetectionManager.MetadataInitialized -= ObjectDetectionManagerOnMetadataInitialized;
        _objectDetectionManager.ObjectDetectionsUpdated -= ObjectDetectionManagerOnObjectDetectionsUpdated;
    }

    private void ObjectDetectionManagerOnMetadataInitialized(ARObjectDetectionModelEventArgs obj)
    {
        _objectDetectionManager.ObjectDetectionsUpdated += ObjectDetectionManagerOnObjectDetectionsUpdated;
    }

    private void ObjectDetectionManagerOnObjectDetectionsUpdated(ARObjectDetectionsUpdatedEventArgs obj)
    {
        string resultString = "";
        float confidence = 0;
        string name = "";
        var results = obj.Results;

        if (results == null)
            return;

        _drawRect.ClearRects();

        for (int i = 0; i < results.Count; i++)
        {
            var detection = results[i];
            var categorizations = detection.GetConfidentCategorizations(_probabilityThreshold);

            if (categorizations.Count <= 0)
            {
                break;
            }

            categorizations.Sort((a, b) => b.Confidence.CompareTo(a.Confidence));

            var categoryToDisplay = categorizations[0];
            confidence = categoryToDisplay.Confidence;
            name = categoryToDisplay.CategoryName;

            int h = Mathf.FloorToInt(_canvas.GetComponent<RectTransform>().rect.height);
            int w = Mathf.FloorToInt(_canvas.GetComponent<RectTransform>().rect.width);

            var rect = results[i].CalculateRect(w, h, Screen.orientation);

            resultString = $"{name}: {confidence}\n";

            _drawRect.CreateRect(rect, colors[i % colors.Length], resultString);
        }
    }

    private void SetObjectDetectionChannels()
    {
        // Implementasi untuk mengatur saluran deteksi jika diperlukan
    }
}
