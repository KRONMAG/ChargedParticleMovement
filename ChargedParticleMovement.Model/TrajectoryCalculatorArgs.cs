using System;
using CodeContracts;
using Math;
using static System.Math;

namespace ChargedParticleMovement.Model
{
    public class TrajectoryCalculatorArgs
    {
        public Vector3D R0 { get; }

        public Vector3D V0 { get; }

        public Vector3D B { get; }

        public double T { get; }

        public int N { get; }

        public double Q { get; }

        public double M { get; }

        public TrajectoryCalculatorArgs
        (
            Vector3D r0,
            Vector3D v0,
            Vector3D b,
            double t,
            int n,
            double q,
            double m
        )
        {
            Requires.NotNull(r0, nameof(r0), "Не задано начальное положение частицы");
            Requires.NotNull(v0, nameof(v0), "Не задан вектор начальной скорости частицы");
            Requires.NotNull(b, nameof(b), "Не задам вектор магнитной индукции");
            Requires.True(v0 != Vector3D.Zero, "Вектор скорости частицы не может быть нулевым");
            Requires.True(b != Vector3D.Zero, "Вектор магнитной индукции не должен быть нулевым");
            Requires.True(t > 0, "Время движения частицы должно быть больше нуля");
            Requires.True(n > 0, "Количество отрезков времени должно быть больше нуля");
            Requires.True(q != 0, "Заряд частицы должен отличаться от нуля");
            Requires.True(m > 0, "Масса частицы должна быть положительной");

            R0 = r0;
            V0 = v0;
            B = b;
            T = t;
            N = n;
            Q = q;
            M = m;
        }

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
            return new TrajectoryCalculatorArgs(r0, v0, b, t, n, q, m);
        }
    }
}