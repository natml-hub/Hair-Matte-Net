/* 
*   Hair Matte Net
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Vision {

    using System;
    using NatML.Features;
    using NatML.Internal;
    using NatML.Types;

    /// <summary>
    /// Hair Matte Net hair segmentation.
    /// </summary>
    public sealed partial class HairMatteNetPredictor : IMLPredictor<HairMatteNetPredictor.Matte> {

        #region --Client API--
        /// <summary>
        /// Create the Hair Matte Net segmentation predictor.
        /// </summary>
        /// <param name="model">Hair Matte Net ML model.</param>
        public HairMatteNetPredictor (MLModel model) => this.model = model as MLEdgeModel;

        /// <summary>
        /// Segment hair in an image.
        /// </summary>
        /// <param name="inputs">Input image.</param>
        /// <returns>Hair segmentation map.</returns>
        public Matte Predict (params MLFeature[] inputs) {
            // Check
            if (inputs.Length != 1)
                throw new ArgumentException(@"HairMatteNet predictor expects a single feature", nameof(inputs));
            // Check type
            var input = inputs[0];
            if (!MLImageType.FromType(input.type))
                throw new ArgumentException(@"HairMatteNet predictor expects an an array or image feature", nameof(inputs));
            // Predict
            var inputType = model.inputs[0];
            using var inputFeature = (input as IMLEdgeFeature).Create(inputType);
            using var outputFeatures = model.Predict(inputFeature);
            // Marshal
            var matte = new MLArrayFeature<float>(outputFeatures[0]);
            var result = new Matte(matte.shape[3], matte.shape[2], matte.ToArray());
            // Return
            return result;
        }
        #endregion


        #region --Operations--
        private readonly MLEdgeModel model;

        void IDisposable.Dispose () { }
        #endregion
    }
}