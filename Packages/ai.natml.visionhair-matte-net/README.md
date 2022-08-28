# Hair Matte Net
Hair matte segmentation. This predictor implements [HairMatteNet](https://arxiv.org/pdf/1712.07168.pdf). This package requires [NatML](https://github.com/natmlx/NatML).

## Segmenting Hair in an Image
First, create the predictor:
```csharp
// Fetch model data from NatML Hub
var modelData = await MLModelData.FromHub("@natsuite/hair-matte-net");
// Deserialize the model
var model = modelData.Deserialize();
// Create the hair segmentation predictor
var predictor = new HairMatteNetPredictor(model);
```

Then segment hair in an image:
```csharp
Texture2D image = ...; // Can also be an `MLImageFeature`
// Segment the hair in the image
HairMatteNetPredictor.Matte matte = predictor.Predict(image);
```

Finally, render the segmentation map to a `RenderTexture`:
```csharp
// Create a `RenderTexture`
var hairMask = new RenderTexture(image.width, image.height, 0);
// Render the segmentation map into the render texture
matte.Render(hairMask);
```

## References
- https://arxiv.org/pdf/1712.07168.pdf
- https://github.com/wonbeomjang/mobile-hair-segmentation-pytorch

## Requirements
- Unity 2020.3+
- [NatML 1.0.16+](https://github.com/natmlx/NatML)

## Quick Tips
- Discover more ML models on [NatML Hub](https://hub.natml.ai).
- See the [NatML documentation](https://docs.natml.ai/unity).
- Join the [NatML community on Discord](https://discord.gg/y5vwgXkz2f).
- Discuss [NatML on Unity Forums](https://forum.unity.com/threads/natml-machine-learning-runtime.1109339/).
- Contact us at [hi@natml.ai](mailto:hi@natml.ai).

Thank you very much!