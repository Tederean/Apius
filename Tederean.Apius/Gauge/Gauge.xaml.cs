﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Tederean.Apius.Extensions;

namespace Tederean.Apius
{

  public partial class Gauge : UserControl
  {

    private const double AngleOffset = (Math.PI / 180.0) * 30.0;

    private const double AngleCoordinateRotation = (Math.PI / 180.0) * 270.0;

    private const double FullAngle = (2 * Math.PI) - (2 * AngleOffset);


    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(double?), typeof(Gauge), new PropertyMetadata(null, OnPropertyChanged));

    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(double?), typeof(Gauge), new PropertyMetadata(null, OnPropertyChanged));

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double?), typeof(Gauge), new PropertyMetadata(null, OnPropertyChanged));

    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(Geometry), typeof(Gauge), new PropertyMetadata(null, OnPropertyChanged));

    public static readonly DependencyProperty FormatterProperty = DependencyProperty.Register(nameof(Formatter), typeof(Func<double?, string>), typeof(Gauge), new PropertyMetadata(null, OnPropertyChanged));

    private static readonly DependencyProperty IntermediateValueProperty = DependencyProperty.Register(nameof(IntermediateValue), typeof(double), typeof(Gauge), new PropertyMetadata(0.0D, OnPropertyChanged));



    public double? Minimum
    {
      get => (double?)GetValue(MinimumProperty);
      set => SetValue(MinimumProperty, value);
    }

    public double? Maximum
    {
      get => (double?)GetValue(MaximumProperty);
      set => SetValue(MaximumProperty, value);
    }

    public double? Value
    {
      get => (double?)GetValue(ValueProperty);
      set => SetValue(ValueProperty, value);
    }

    public Geometry? Icon
    {
      get => (Geometry?)GetValue(IconProperty);
      set => SetValue(IconProperty, value);
    }

    public Func<double?, string>? Formatter
    {
      get => (Func<double?, string>?)GetValue(FormatterProperty);
      set => SetValue(FormatterProperty, value);
    }

    private double IntermediateValue
    {
      get => (double)GetValue(IntermediateValueProperty);
      set => SetValue(IntermediateValueProperty, value);
    }



    public Gauge()
    {
      InitializeComponent();
    }



    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
      base.OnRenderSizeChanged(sizeInfo);

      DrawBackground();
      DrawForeground();
    }

    private static void OnPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
      if (dependencyObject is Gauge gauge)
      {
        if (args.Property == MinimumProperty || args.Property == MaximumProperty)
        {
          gauge.DrawBackground();
          gauge.DrawForeground();
          return;
        }

        if (args.Property == IntermediateValueProperty)
        {
          gauge.DrawForeground();
          return;
        }

        if (args.Property == ValueProperty)
        {
          gauge.UpdateValueText();
          gauge.UpdateValue((double?)args.OldValue, (double?)args.NewValue);
          return;
        }

        if (args.Property == FormatterProperty)
        {
          gauge.UpdateValueText();
          return;
        }

        if (args.Property == IconProperty)
        {
          gauge.UpdateIcon();
          return;
        }
      }
    }

    private void UpdateIcon()
    {
      _iconPath.Data = Icon;
    }

    private void UpdateValueText()
    {
      _valueText.Content = Formatter?.Invoke(Value);
    }

    private void UpdateValue(double? oldValue, double? newValue)
    {
      var doubleAnimation = new DoubleAnimation(oldValue ?? 0.0, newValue ?? 0.0, TimeSpan.FromMilliseconds(500));

      BeginAnimation(IntermediateValueProperty, doubleAnimation, HandoffBehavior.SnapshotAndReplace);
    }

    private void DrawBackground()
    {
      if (_gaugeGrid.ActualWidth <= 0 || _gaugeGrid.ActualHeight <= 0)
        return;

      (var startpoint, var size, var isLargeArc, var endpoint) = Calculate(_gaugeGrid.ActualWidth, _gaugeGrid.ActualHeight, 1.0);

      _backgroundEllipseStart.Center = startpoint;

      _backgroundPathFigure.StartPoint = startpoint;
      _backgroundArcSegment.Size = size;
      _backgroundArcSegment.IsLargeArc = isLargeArc;
      _backgroundArcSegment.Point = endpoint;

      _backgroundEllipseEnd.Center = endpoint;
    }

    private void DrawForeground()
    {
      if (_gaugeGrid.ActualWidth <= 0 || _gaugeGrid.ActualHeight <= 0 || Minimum == null || Maximum == null)
        return;

      var ratio = IntermediateValue.Map(Minimum.Value, Maximum.Value, 0.0, 1.0);

      ratio = Math.Clamp(ratio, 0.0, 1.0);

      (var startpoint, var size, var isLargeArc, var endpoint) = Calculate(_gaugeGrid.ActualWidth, _gaugeGrid.ActualHeight, ratio);

      var gaugeColor = GetGaugeColor(ratio);

      _foregroundEllipseStart.Center = startpoint;

      _foregroundPathFigure.StartPoint = startpoint;
      _foregroundArcSegment.Size = size;
      _foregroundArcSegment.IsLargeArc = isLargeArc;
      _foregroundArcSegment.Point = endpoint;

      _foregroundEllipseEnd.Center = endpoint;

      _foregroundPath.Stroke = gaugeColor;
    }

    private Brush GetGaugeColor(double ratio)
    {
      var hue = ratio < 0.7 ? 120 : (int)Math.Round(ratio.Map(0.7, 1.0, 120.0, 0.0));

      var hsl = new ColorHelper.HSL(hue, 100, 50);
      var rgb = ColorHelper.ColorConverter.HslToRgb(hsl);

      return new SolidColorBrush(Color.FromRgb(rgb.R, rgb.G, rgb.B));
    }

    private (Point Startpoint, Size Size, bool IsLargeArc, Point Endpoint) Calculate(double actualWidth, double actualHeight, double ration)
    {
      var center = new Point(actualWidth / 2.0, actualHeight / 2.0);
      var radius = Math.Min(center.X, center.Y) - (Math.Min(center.X, center.Y) / 10.0);
      var size = new Size(radius, radius);

      var startPoint = new Point
        (Math.Cos(AngleCoordinateRotation - AngleOffset) * radius + center.X,
        (-Math.Sin(AngleCoordinateRotation - AngleOffset) * radius) + center.Y);

      var backgroundEndPoint = CalculateEndpoint(ration, center, radius);

      return (startPoint, size, backgroundEndPoint.IsLargeArc, backgroundEndPoint.Endpoint);
    }

    private (Point Endpoint, bool IsLargeArc) CalculateEndpoint(double ratio, Point middle, double radius)
    {
      var angle = ratio * FullAngle;
      var isLargeArc = angle > Math.PI;

      angle = AngleCoordinateRotation - AngleOffset - angle;

      var endpoint= new Point(Math.Cos(angle) * radius + middle.X, -Math.Sin(angle) * radius + middle.Y);

      return (endpoint, isLargeArc);
    }
  }
}