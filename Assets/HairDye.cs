/* 
*   Hair Dye
*   Copyright (c) 2021 Yusuf Olokoba.
*/

namespace NatSuite.Examples {

    using UnityEngine;
    using UnityEngine.UI;
    using NatSuite.Devices;
    using NatSuite.ML;
    using NatSuite.ML.Vision;

    public class HairDye : MonoBehaviour {

        [Header(@"NatML Hub")]
        public string accessKey;

        [Header(@"Visualization")]
        public RawImage cameraPanel;
        public RawImage overlayPanel;
        public AspectRatioFitter cameraAspectFitter;
        public AspectRatioFitter overlayAspectFitter;

        CameraDevice cameraDevice;
        Texture2D previewTexture;
        MLModelData modelData;
        MLModel model;
        HairMatteNetPredictor predictor;
        RenderTexture segmentationImage;

        async void Start () {
            // Request camera permissions
            if (!await MediaDeviceQuery.RequestPermissions<CameraDevice>()) {
                Debug.LogError(@"User did not grant camera permissions");
                return;
            }
            // Get the default camera device
            var query = new MediaDeviceQuery(MediaDeviceCriteria.FrontCamera);
            cameraDevice = query.current as CameraDevice;
            // Start the camera preview
            cameraDevice.previewResolution = (1280, 720);
            previewTexture = await cameraDevice.StartRunning();
            // Create the segmentation image
            segmentationImage = new RenderTexture(previewTexture.width, previewTexture.height, 0);
            // Display the preview
            cameraPanel.texture = previewTexture;
            overlayPanel.texture = segmentationImage;
            cameraAspectFitter.aspectRatio = 
            overlayAspectFitter.aspectRatio = (float)previewTexture.width / previewTexture.height;
            // Fetch the model data from Hub
            Debug.Log("Fetching model data from Hub");
            modelData = await MLModelData.FromHub("@natsuite/hair-matte-net", accessKey);
            // Deserialize the model
            model = modelData.Deserialize();
            // Create the hair matte predictor
            predictor = new HairMatteNetPredictor(model);
        }

        void Update () {
            // Check that predictor has been created
            if (predictor == null)
                return;
            // Predict the hand pose
            var segmentationMap = predictor.Predict(previewTexture);
            // Render the segmentation map to texture
            segmentationMap.Render(segmentationImage, matteStrength: 5f);
        }

        void OnDisable () {
            // Dispose the model
            model?.Dispose();
            // Stop the camera preview
            if (cameraDevice?.running ?? false)
                cameraDevice.StopRunning();
        }
    }
}