using System.IO;
using System.Linq;
using System.Text;
using ChargedParticleMovement.Model;

namespace ChargedParticleMovement.Console
{
    public static class Program
    {
        public static void Main()
        {
            var trajectory = TrajectoryCalculator.Calculate(TrajectoryCalculatorArgs.GetArgs(ParticleType.Electron, TrajectoryType.Straight, false));
            File.Create("out.csv").Close();
            File.AppendAllText("out.csv", "X;Y;Z\n", Encoding.ASCII);
            File.AppendAllLines("out.csv", trajectory.Select(vector => $"{vector.X};{vector.Y};{vector.Z}"));
        }
    }
}
