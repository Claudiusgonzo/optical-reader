﻿using Nokia.Graphics.Imaging;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace OpticalReaderLib
{
    class BasicEnhancer : IEnhancer
    {
        public async Task<EnhanceResult> EnhanceAsync(Frame frame)
        {
            using (var bitmap = new Bitmap(new Windows.Foundation.Size(frame.Dimensions.Width, frame.Dimensions.Height), Internal.Utilities.FrameFormatToColorMode(frame.Format), frame.Pitch, frame.Buffer.AsBuffer()))
            using (var source = new BitmapImageSource(bitmap))
            using (var effect = new FilterEffect(source))
            using (var renderer = new BitmapRenderer(effect))
            {
                effect.Filters = new List<IFilter>()
                {
                    new ContrastFilter(0.5)
                };

                using (var newBitmap = new Bitmap(new Windows.Foundation.Size(frame.Dimensions.Width, frame.Dimensions.Height), Internal.Utilities.FrameFormatToColorMode(frame.Format)))
                {
                    await effect.GetBitmapAsync(newBitmap, OutputOption.PreserveAspectRatio);

                    return new EnhanceResult()
                    {
                        Frame = new Frame()
                        {
                            Buffer = newBitmap.Buffers[0].Buffer.ToArray(),
                            Pitch = newBitmap.Buffers[0].Pitch,
                            Format = frame.Format,
                            Dimensions = newBitmap.Dimensions
                        }
                    };
                }
            }
        }
    }
}
