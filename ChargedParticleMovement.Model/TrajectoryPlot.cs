using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Math;
using Plotly;

namespace ChargedParticleMovement.Model
{
    /// <summary>
    /// Визуализация движения заряженной частицы в магнитном поле
    /// </summary>
    public class TrajectoryPlot
    {
        /// <summary>
        /// Отрисовка траектории движения частицы
        /// </summary>
        /// <param name="args">Параметры модели</param>
        /// <returns>Путь к html-файлу с графиком движения частицы</returns>
        public static string BuildPlot(TrajectoryCalculatorArgs args)
        {
            var points = TrajectoryCalculator.Calculate(args);
            var plot = new Plot
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
            var plotFileName = "plot.html";
            File.WriteAllText
            (
                plotFileName,
                plot
                    .Render()
                    .ToString()
                    .Replace("<body>", "<body style=\"overflow: hidden\">")
                    .Replace("height: 100%", "height: 100vh;")
                    .Replace("https://cdn.plot.ly/plotly-latest.min.js", "plotly.js")
            );
            return plotFileName;
        }

        /// <summary>
        /// Получение координат из точек траектории движения частицы
        /// </summary>
        /// <param name="points">Точки траектории движения частицы</param>
        /// <param name="selector">Селектор координат точек</param>
        /// <returns>Массив, содержащий координату каждой точки траектории</returns>
        private static float[] GetCoordinates(List<Vector3D> points, Func<Vector3D, double> selector) =>
            points
                .AsParallel()
                .Select(point => (float)selector(point))
                .ToArray();
    }
}