using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Radical.Windows.Effects
{
    /// <summary>
    /// A shader effect that renders the element in grayscale.
    /// </summary>
    public class GrayscaleEffect : ShaderEffect
    {
        private static readonly PixelShader _pixelShader = new PixelShader()
        {
            UriSource = new Uri(@"pack://application:,,,/Radical.Windows;component/Resources/Effects/GrayscaleEffect.ps")
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="GrayscaleEffect"/> class.
        /// </summary>
        public GrayscaleEffect()
        {
            PixelShader = _pixelShader;
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(DesaturationFactorProperty);
        }

        /// <summary>
        /// Identifies the <see cref="Input"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input",
            typeof(GrayscaleEffect), 0);

        /// <summary>
        /// Gets or sets the brush used as the effect input.
        /// </summary>
        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DesaturationFactor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DesaturationFactorProperty = DependencyProperty.Register(
            "DesaturationFactor",
            typeof(double),
            typeof(GrayscaleEffect),
            new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0), CoerceDesaturationFactor));

        /// <summary>
        /// Gets or sets the desaturation factor, between 0 and 1. Values out of range are ignored.
        /// </summary>
        public double DesaturationFactor
        {
            get { return (double)GetValue(DesaturationFactorProperty); }
            set { SetValue(DesaturationFactorProperty, value); }
        }

        private static object CoerceDesaturationFactor(DependencyObject d, object value)
        {
            var effect = (GrayscaleEffect)d;
            var newFactor = (double)value;

            if (newFactor < 0.0 || newFactor > 1.0)
            {
                return effect.DesaturationFactor;
            }

            return newFactor;
        }
    }
}
