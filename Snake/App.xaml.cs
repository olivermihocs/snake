using Microsoft.Win32;
using System;
using System.Windows;
using System.Timers;

using Snake.Persistence;
using Snake.Model;
using Snake.ViewModel;
using Snake.View;

namespace Snake
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Attributes
        private SnakePersistence _persistence;
        private SnakeModel _model;
        private SnakeGridViewModel _viewModel;
        private MainWindow _view;
        private Timer _timer;
        #endregion
        #region Constructor
        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }
        #endregion
        #region Initialization
        private void App_Startup(object sender, StartupEventArgs e)
        {

            _persistence = new SnakePersistence();

            _model = new SnakeModel(_persistence);

            _viewModel = new SnakeGridViewModel(_model);
            _viewModel.NewGame += new EventHandler<String>(ViewModel_NewGame);
            _viewModel.LoadFile += new EventHandler(ViewModel_LoadFile);
            _viewModel.SaveFile += new EventHandler(ViewModel_SaveFile);
            _viewModel.GamePause += new EventHandler(ViewModel_GamePause);
            _viewModel.GameOver += new EventHandler<int>(ViewModel_OnGameOver);
            _viewModel.DirectionChange += new EventHandler<String>(ViewModel_DirectionChange);

            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Show();

            _timer = new Timer();
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _timer.Interval = 1500;
        }
        #endregion
        #region Events
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            Timer timer = sender as Timer;
            if (timer == _timer)
            {
                _model.StepGame();
            }
        }
        private void ViewModel_LoadFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Load Game";
            openFileDialog.Filter = "Snake Game Files | *.sgf";
            if (openFileDialog.ShowDialog() == true)
            {
                _model.NewGame(openFileDialog.FileName);
            }
        }

        private void ViewModel_SaveFile(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save Game";
            saveFileDialog.Filter = "Snake Game Files | *.sgf";
            if (saveFileDialog.ShowDialog() == true)
                _model.SaveGame(saveFileDialog.FileName);
        }

        private void ViewModel_NewGame(object sender, String map)
        {
            _model.NewGame(map);
        }

        private void ViewModel_GamePause(object sender, EventArgs e)
        {
            if (_timer.Enabled)
                _timer.Stop();
            else
                _timer.Start();
        }

        private void ViewModel_OnGameOver(object sender, int score)
        {
            _timer.Stop();
            MessageBox.Show("You've collected " + score + " eggs!",
                                "Snake - Game Over!",
                                MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
        }

        private void ViewModel_DirectionChange(object sender, String direction)
        {
            switch (direction)
            {
                case "UP": _model.Direction = SnakeModel.Dir.UP; break;
                case "DOWN": _model.Direction = SnakeModel.Dir.DOWN; break;
                case "LEFT": _model.Direction = SnakeModel.Dir.LEFT; break;
                case "RIGHT": _model.Direction = SnakeModel.Dir.RIGHT; break;
            }
        }
        #endregion
    }
}
