/* 
*   Hair Matte Net
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples {

    using UnityEngine;
    using UnityEngine.UI;
    using NatML;
    using NatML.Devices;
    using NatML.Devices.Outputs;
    using NatML.Vision;

    public class HairMatteNetSample : MonoBehaviour {

        [Header(@"Camera Preview")]
        public RawImage cameraPanel;
        public AspectRatioFitter cameraAspectFitter;

        [Header(@"Hair Overlay")]
        public RawImage overlayPanel;
        public AspectRatioFitter overlayAspectFitter;

        CameraDevice cameraDevice;
        TextureOutput textureOutput;

        MLModelData modelData;
        MLModel model;
        HairMatteNetPredictor predictor;
        RenderTexture hairMask;

        async void Start () {
            // Request camera permissions
            var permissionStatus = await MediaDeviceQuery.RequestPermissions<CameraDevice>();
            if (permissionStatus != PermissionStatus.Authorized) {
                Debug.LogError(@"User did not grant camera permissions");
                return;
            }
            // Get the default camera device
            var query = new MediaDeviceQuery(MediaDeviceCriteria.CameraDevice);
            cameraDevice = query.current as CameraDevice;
            // Start the camera preview
            cameraDevice.previewResolution = (1280, 720);
            textureOutput = new TextureOutput();
            cameraDevice.StartRunning(textureOutput);
            // Display the camera preview
            var previewTexture = await textureOutput;
            var aspectRatio = (float)previewTexture.width / previewTexture.height;
            cameraPanel.texture = previewTexture;
            cameraAspectFitter.aspectRatio = aspectRatio;
            // Display the hair mask
            hairMask = new RenderTexture(previewTexture.width, previewTexture.height, 0);
            overlayPanel.texture = hairMask;
            overlayAspectFitter.aspectRatio = aspectRatio;    
            // Create the hair matte predictor
            Debug.Log("Fetching model data from NatML");
            modelData = await MLModelData.FromHub("@natsuite/hair-matte-net");
            model = modelData.Deserialize();
            predictor = new HairMatteNetPredictor(model);
        }

        void Update () {
            // Check that predictor has been created
            if (predictor == null)
                return;
            // Predict hair mask
            var matte = predictor.Predict(textureOutput.texture);
            matte.Render(hairMask);
        }

        void OnDisable () {
            // Dispose the model
            model?.Dispose();
        }
    }
}