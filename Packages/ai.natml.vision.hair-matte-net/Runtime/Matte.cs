/* 
*   Hair Matte Net
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Vision {

    using System;
    using UnityEngine;
    using Unity.Collections.LowLevel.Unsafe;

    public sealed partial class HairMatteNetPredictor {

        /// <summary>
        /// Hair segmentation map.
        /// </summary>
        public readonly struct Matte {

            #region --Client API--
            /// <summary>
            /// Map width.
            /// </summary>
            public readonly int width;

            /// <summary>
            /// Map height.
            /// </summary>
            public readonly int height;

            /// <summary>
            /// Render the probability map to a texture.
            /// Each pixel will have value `(p, p, p, 1.0)` where `p` is the hair matte alpha.
            /// </summary>
            /// <param name="destination">Destination texture.</param>
            /// <param name="strength">Control the hardness of the matte.</param>
            public unsafe void Render (RenderTexture destination, float strength = 4f) {
                // Check texture
                if (!destination)
                    throw new ArgumentNullException(nameof(destination));
                // Upload texture data
                var planeSize = width * height;
                var foreTexture = new Texture2D(width, height, TextureFormat.RFloat, false) { filterMode = FilterMode.Point };
                var backTexture = new Texture2D(width, height, TextureFormat.RFloat, false) { filterMode = FilterMode.Point };
                fixed (float* logits = data) {
                    UnsafeUtility.MemCpy(foreTexture.GetRawTextureData<float>().GetUnsafePtr(), logits, planeSize * sizeof(float));
                    UnsafeUtility.MemCpy(backTexture.GetRawTextureData<float>().GetUnsafePtr(), &logits[planeSize], planeSize * sizeof(float));
                }
                foreTexture.Apply(false);
                backTexture.Apply(false);
                // Blit
                renderer = renderer ? renderer : new Material(Shader.Find(@"Hidden/HairMatteNet/Blit"));
                renderer.SetTexture(@"_BackTexture", backTexture);
                renderer.SetFloat(@"_Strength", strength);
                Graphics.Blit(foreTexture, destination, renderer);
                // Release
                Texture2D.Destroy(foreTexture);
                Texture2D.Destroy(backTexture);
            }
            #endregion


            #region --Operations--
            private readonly float[] data;
            private static Material renderer;

            internal Matte (int width, int height, float[] data) {
                this.width = width;
                this.height = height;
                this.data = data;
            }
            #endregion
        }
    }
}