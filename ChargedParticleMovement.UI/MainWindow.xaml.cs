using System;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Math;
using ChargedParticleMovement.Model;
using static ChargedParticleMovement.Model.ParticleType;
using static ChargedParticleMovement.Model.TrajectoryType;
using System.ComponentModel;
using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;

namespace ChargedParticleMovement.UI
{
    /// <summary>
    /// Окно работы с моделью движения заряженной частицы в магнитном поле
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// Инициализация окна
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            SetTrajectoryCalculatorArgs
            (
                TrajectoryCalculatorArgs.GetArgs
                (
                    AlphaParticle,
                    Spiral,
                    false
                )
            );
        }

        /// <summary>
        /// Обработчик события нажатия кнопки расчета траектории движения частицы
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void CalculateTrajectoryClicked(object sender, RoutedEventArgs e)
        {
            if (TryGetTrajectoryCalculatorArgs(out TrajectoryCalculatorArgs args))
            {
                var waitingAnimationWindow = new WaitingAnimationWindow();
                waitingAnimationWindow.Owner = this;
                IsEnabled = false;
                waitingAnimationWindow.Message = "Расчет траектории";
                waitingAnimationWindow.Show();
                void StopAnimation(object _, WebViewControlDOMContentLoadedEventArgs __)
                {
                    TrajectoryWebView.DOMContentLoaded -= StopAnimation;
                    IsEnabled = true;
                    waitingAnimationWindow.Hide();
                }
                TrajectoryWebView.DOMContentLoaded += StopAnimation;

                var worker = new BackgroundWorker();
                worker.DoWork += (_, e) => e.Result = TrajectoryPlot.BuildPlot(args);
                worker.RunWorkerCompleted += (_, e) => TrajectoryWebView.NavigateToLocal((string)e.Result);
                worker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки генерации параметров модели
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void GenerateTrajectoryCalculatorArgsClicked(object sender, RoutedEventArgs e) =>
            SetTrajectoryCalculatorArgs
            (
                TrajectoryCalculatorArgs.GetArgs
                (
                    ParticleTypeComboBox.SelectedValue.ToString() switch
                    {
                        "Альфа-частица" => AlphaParticle,
                        "Электрон" => Electron,
                        "Протон" => Proton
                    },
                    TrajectoryTypeComboBox.SelectedValue.ToString() switch
                    {
                        "Прямая" => Straight,
                        "Окружность" => Circle,
                        "Спираль" => Spiral
                    },
                    UseRandomComboBox.SelectedValue.ToString() switch
                    {
                        "Да" => true,
                        "Нет" => false
                    }
                )
            );

        /// <summary>
        /// Попытка конструирования параметров модели на основе введенных значений
        /// </summary>
        /// <param name="args">Параметры модели, соответствующие введенным данным</param>
        /// <returns>Истина, если введенные значения параметров корректны, иначе - ложь</returns>
        private bool TryGetTrajectoryCalculatorArgs(out TrajectoryCalculatorArgs args)
        {
            try
            {
                args = new TrajectoryCalculatorArgs
                (
                    MNumericUpDown.Value.Value,
                    QNumericUpDown.Value.Value,
                    TNumericUpDown.Value.Value,
                    (int)NNumericUpDown.Value.Value,
                    new Vector3D
                    (
                        R0xNumericUpDown.Value.Value,
                        R0yNumericUpDown.Value.Value,
                        R0zNumericUpDown.Value.Value
                    ),
                    new Vector3D
                    (
                        V0xNumericUpDown.Value.Value,
                        V0yNumericUpDown.Value.Value,
                        V0zNumericUpDown.Value.Value
                    ),
                    new Vector3D
                    (
                        BxNumericUpDown.Value.Value,
                        ByNumericUpDown.Value.Value,
                        BzNumericUpDown.Value.Value
                    )
                );
                return true;
            }
            catch (ArgumentException ex)
            {
                this.ShowModalMessageExternal("Ошибка", ex.Message);
            }
            catch (InvalidOperationException)
            {
                this.ShowModalMessageExternal("Ошибка", "Заданы значения не для всех параметров модели");
            }
            args = null;
            return false;
        }

        /// <summary>
        /// Установка значений полей ввода на основе параметров модели
        /// </summary>
        /// <param name="args">Параметры модели</param>
        private void SetTrajectoryCalculatorArgs(TrajectoryCalculatorArgs args)
        {
            MNumericUpDown.Value = args.M;
            QNumericUpDown.Value = args.Q;
            R0xNumericUpDown.Value = args.R0.X;
            R0yNumericUpDown.Value = args.R0.Y;
            R0zNumericUpDown.Value = args.R0.Z;
            V0xNumericUpDown.Value = args.V0.X;
            V0yNumericUpDown.Value = args.V0.Y;
            V0zNumericUpDown.Value = args.V0.Z;
            BxNumericUpDown.Value = args.B.X;
            ByNumericUpDown.Value = args.B.Y;
            BzNumericUpDown.Value = args.B.Z;
            TNumericUpDown.Value = args.T;
            NNumericUpDown.Value = args.N;
        }
    }
}