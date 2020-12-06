using System;
using CodeContracts;
using Math;
using static System.Math;

namespace ChargedParticleMovement.Model
{
    /// <summary>
    /// Параметры модели движения заряженной частицы в магнитном поле
    /// </summary>
    public class TrajectoryCalculatorArgs
    {
        /// <summary>
        /// Начальное положение частицы
        /// </summary>
        public Vector3D R0 { get; }

        /// <summary>
        /// Вектор начальной скорости частицы
        /// </summary>
        public Vector3D V0 { get; }

        /// <summary>
        /// Вектор индукции магнитного поля
        /// </summary>
        public Vector3D B { get; }

        /// <summary>
        /// Время движения частицы
        /// </summary>
        public double T { get; }

        /// <summary>
        /// Количество отрезков времени
        /// </summary>
        public int N { get; }

        /// <summary>
        /// Заряд частицы
        /// </summary>
        public double Q { get; }

        /// <summary>
        /// Масса частицы
        /// </summary>
        public double M { get; }

        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="m">Масса частицы</param>
        /// <param name="q">Заряд частицы</param>
        /// <param name="t">Время движения частицы</param>
        /// <param name="n">Количество отрезков времени</param>
        /// <param name="r0">Начальное положение частицы</param>
        /// <param name="v0">Вектор начальной скорости частицы</param>
        /// <param name="b">Вектор индукции магнитного поля</param>
        public TrajectoryCalculatorArgs
        (
            double m,
            double q,
            double t,
            int n,
            Vector3D r0,
            Vector3D v0,
            Vector3D b
        )
        {
            Requires.NotNull(r0, nameof(r0), "Не задано начальное положение частицы");
            Requires.NotNull(v0, nameof(v0), "Не задан вектор начальной скорости частицы");
            Requires.NotNull(b, nameof(b), "Не задам вектор магнитной индукции");
            Requires.True(m > 0, "Масса частицы должна быть положительной");
            Requires.True(q != 0, "Заряд частицы должен отличаться от нуля");
            Requires.True(t > 0, "Время движения частицы должно быть больше нуля");
            Requires.True(n > 0, "Количество отрезков времени должно быть больше нуля");
            Requires.True(n <= 5000000, "Количество отрезков времени не должно превышать пяти миллионов");
            Requires.True(v0 != Vector3D.Zero, "Вектор скорости частицы не может быть нулевым");
            Requires.True(b != Vector3D.Zero, "Вектор магнитной индукции не должен быть нулевым");

            M = m;
            Q = q;
            T = t;
            N = n;
            R0 = r0;
            V0 = v0;
            B = b;
        }

        /// <summary>
        /// Получение параметров модели для определенного типа частицы
        /// </summary>
        /// <param name="particleType">Тип частицы</param>
        /// <param name="trajectoryType">Тип траектории движения частицы</param>
        /// <param name="useRandom">
        /// Требуется ли вектора индукции магнитного поля и
        /// начальной скорости частицы генерировать случайно
        /// </param>
        /// <returns>Параметры модели для заданного типа частицы</returns>
        public static TrajectoryCalculatorArgs GetArgs(ParticleType particleType, TrajectoryType trajectoryType, bool useRandom)
        {
            var r0 = new Vector3D(0, 0, 0);
            var (q, m) = particleType switch
            {
                ParticleType.AlphaParticle => (3.218e-19, 6.645e-27),
                ParticleType.Electron => (-1.602e-19, 9.109e-31),
                ParticleType.Proton => (1.602e-19, 1.673e-27)
            };
            var n = 100000;
            var (v0, t)= particleType switch
            {
                ParticleType.AlphaParticle => (new Vector3D(Sqrt(Pow(15000000, 2) / 3)), 3e-6),
                ParticleType.Electron => (new Vector3D(Sqrt(Pow(5000000, 2) / 3)), 1e-9),
                ParticleType.Proton => (new Vector3D(Sqrt(Pow(1000000, 2) / 3)), 3e-6)
            };
            var b = new Vector3D(Sqrt(Pow(0.25, 2) / 3));
            b = trajectoryType switch
            {
                TrajectoryType.Straight => b,
                TrajectoryType.Circle => new Vector3D(b.X, -b.Y / 2, -b.Z / 2),
                TrajectoryType.Spiral => b.RotateE1(180)
            };
            if (useRandom)
            {
                var random = new Random();
                var xAngle = random.NextDouble() * 90;
                var yAngle = random.NextDouble() * 90;
                var zAngle = random.NextDouble() * 90;
                switch (trajectoryType)
                {
                    case TrajectoryType.Straight:
                    case TrajectoryType.Circle:
                        v0 = v0.RotateE1(xAngle).RotateE2(yAngle).RotateE3(zAngle);
                        b = b.RotateE1(xAngle).RotateE2(yAngle).RotateE3(zAngle);
                        break;
                    case TrajectoryType.Spiral:
                        v0 = v0.RotateE1(xAngle).RotateE2(yAngle);
                        b = b.RotateE2(yAngle).RotateE3(zAngle);
                        break;
                };
            }
            v0 = RoundVector(v0, 2);
            b = RoundVector(b, 5);
            return new TrajectoryCalculatorArgs(m, q, t, n, r0, v0, b);
        }

        /// <summary>
        /// Округление координат вектора до
        /// указанного количества цифр после запятой
        /// </summary>
        /// <param name="vector">Вектор, координаты которого будут округлены</param>
        /// <param name="decimals">Количество дробных разрядов, до которых нужно округлить координаты</param>
        /// <returns>Вектор с округленными координатами</returns>
        private static Vector3D RoundVector(Vector3D vector, int decimals)
        {
            vector.X = Round(vector.X, decimals);
            vector.Y = Round(vector.Y, decimals);
            vector.Z = Round(vector.Z, decimals);
            return vector;
        }
    }
}