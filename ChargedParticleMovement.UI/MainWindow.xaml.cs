using System;
using System.Linq;
using Plotly;
using ChargedParticleMovement.Model;
using System.Collections.Generic;
using Math;
using MahApps.Metro.Controls;

namespace ChargedParticleMovement.UI
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var points = TrajectoryCalculator.Calculate
            (
                TrajectoryCalculatorArgs.GetArgs
                (
                    ParticleType.AlphaParticle,
                    TrajectoryType.Spiral,
                    false
                )
            );

            var chart = new Plot
            (
                Plot.traces
                (
                    Traces.scatter3d
                    (
                        Scatter3d.x(GetCoordinates(points, point => point.X)),
                        Scatter3d.y(GetCoordinates(points, point => point.Y)),
                        Scatter3d.z(GetCoordinates(points, point => point.Z)),
                        Scatter3d.mode(Scatter3d.Mode.lines())
                    )
                ),
                Plot.layout
                (
                    Layout.autosize(true),
                    Layout.margin
                    (
                        Plotly.Margin.l(0),
                        Plotly.Margin.t(0),
                        Plotly.Margin.r(0),
                        Plotly.Margin.b(0)
                    )
                )
            );

            TrajectoryWebView.NavigateToString(chart.Render().ToString().Replace("width: 100%;", string.Empty));
        }

        private float[] GetCoordinates(List<Vector3D> points, Func<Vector3D, double> selector) =>
            points
                .AsParallel()
                .Select(point => (float)selector(point))
                .ToArray();
    }
}