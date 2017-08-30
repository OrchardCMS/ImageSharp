﻿using System;
using System.Numerics;
using System.Runtime.CompilerServices;

using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion.Implementation.YCbCrColorSapce;

namespace SixLabors.ImageSharp.Formats.Jpeg.Common.Decoder
{
    internal abstract partial class JpegColorConverter
    {
        private class FromYCbCr : JpegColorConverter
        {
            private static readonly YCbCrAndRgbConverter Converter = new YCbCrAndRgbConverter();

            public FromYCbCr()
                : base(JpegColorSpace.YCbCr)
            {
            }

            public override void ConvertToRGBA(ComponentValues values, Span<Vector4> result)
            {
                // TODO: We can optimize a lot here with Vector<float> and SRCS.Unsafe()!
                ReadOnlySpan<float> yVals = values.Component0;
                ReadOnlySpan<float> cbVals = values.Component1;
                ReadOnlySpan<float> crVals = values.Component2;

                var v = new Vector4(0, 0, 0, 1);

                Vector4 scale = new Vector4(1/255f, 1 / 255f, 1 / 255f, 1f);

                for (int i = 0; i < result.Length; i++)
                {
                    float y = yVals[i];
                    float cb = cbVals[i] - 128F;
                    float cr = crVals[i] - 128F;

                    v.X = MathF.Round(y + (1.402F * cr), MidpointRounding.AwayFromZero);
                    v.Y = MathF.Round(y - (0.344136F * cb) - (0.714136F * cr), MidpointRounding.AwayFromZero);
                    v.Z = MathF.Round(y + (1.772F * cb), MidpointRounding.AwayFromZero);

                    v *= scale;

                    result[i] = v;
                }
            }
        }
    }
}