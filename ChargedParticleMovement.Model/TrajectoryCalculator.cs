using System.Collections.Generic;
using CodeContracts;
using Math;

namespace ChargedParticleMovement.Model
{
    /// <summary>
    /// Моделирование движения заряженной частицы в магнитном поле
    /// </summary>
    public static class TrajectoryCalculator
    {
        /// <summary>
        /// Расчет траектории движения заряженной частицы в магнитном поле
        /// </summary>
        /// <param name="args">Параметры модели</param>
        /// <returns>Траектория движения частицы</returns>
        public static List<Vector3D> Calculate(TrajectoryCalculatorArgs args)
        {
            Requires.NotNull(args, nameof(args), "Не заданы параметры движения частицы");
            var dt = args.T / args.N;
            Vector3D a;
            var v = args.V0;
            var r = args.R0;
            var trajectory = new List<Vector3D>();
            trajectory.Add(args.R0);
            for (var i = 0; i < args.N; i++)
            {
                a = args.Q / args.M * v.Cross(args.B);
                r = r + v * dt + a * dt * dt / 2;
                v = v + a * dt;
                trajectory.Add(r);
            }
            return trajectory;
        }
    }
}