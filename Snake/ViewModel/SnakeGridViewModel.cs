using System;
using System.Collections.ObjectModel;

using Snake.Model;

namespace Snake.ViewModel
{
    public class SnakeGridViewModel : ViewModelBase
    {
        #region Private Attributes
        private SnakeModel _model;

        private Int32 _rowCount;
        private Int32 _columnCount;
        private Int32 _counter;

        private bool _paused;
        private bool _activeGame;
        private bool _saveGameState;

        private String _btnText;

        private Random _random;
        #endregion
        #region Properties
        public Int32 RowCount
        {
            get { return _rowCount; }
            set
            {
                if (_rowCount != value)
                {
                    _rowCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public Int32 ColumnCount
        {
            get { return _columnCount; }
            set
            {
                if (_columnCount != value)
                {
                    _columnCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public Int32 Counter
        {
            get { return _counter; }
            set
            {
                _counter = value;
                OnPropertyChanged();
            }
        }

        public bool GamePaused
        {
            get { return _paused; }
            set
            {
                _paused = value;
                SaveGameState = ActiveGame && value;
                OnPropertyChanged();
            }
        }

        public String ButtonText
        {
            get { return _btnText; }
            set
            {
                _btnText = value;
                OnPropertyChanged();
            }

        }

        public bool ActiveGame
        {
            get { return _activeGame; }
            set
            {
                _activeGame = value;
                SaveGameState = GamePaused && value;
                OnPropertyChanged();
            }
        }

        public bool SaveGameState
        {
            get { return _saveGameState; }
            set
            {
                _saveGameState = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SnakeFieldViewModel> Fields { get; private set; }
        #endregion
        #region Commands
        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand LoadFileCommand { get; private set; }
        public DelegateCommand SaveFileCommand { get; private set; }
        public DelegateCommand PauseGameCommand { get; private set; }
        public DelegateCommand ChangeDirectionCommand { get; private set; }
        #endregion
        #region Constructor
        public SnakeGridViewModel(SnakeModel model)
        {
            _model = model;
            Counter = 0;
            GamePaused = true;
            ActiveGame = false;
            ButtonText = "Paused";

            _model.GameCreated += new EventHandler(Model_OnGameCreated);
            _model.StepGameEvent += new EventHandler(Model_OnStepGame);
            _model.CounterChanged += new EventHandler<int>(Model_OnCounterChanged);
            _model.GameOver += new EventHandler(Model_OnGameOver);

            Fields = new ObservableCollection<SnakeFieldViewModel>();

            _random = new Random();

            NewGameCommand = new DelegateCommand(x => OnNewGame(x.ToString()));
            ChangeDirectionCommand = new DelegateCommand(x => OnDirectionChange(x.ToString()));
            LoadFileCommand = new DelegateCommand(x => OnLoadFile());
            SaveFileCommand = new DelegateCommand(x => OnSaveFile());
            PauseGameCommand = new DelegateCommand(x => OnGamePause());
        }
        #endregion
        #region Table Management
        private void CreateTable()
        {
            Fields.Clear();
            RowCount = _model.GetRow();
            ColumnCount = _model.GetCol();
            for (Int32 i = 0; i < RowCount; i++)
            {
                for (Int32 j = 0; j < ColumnCount; j++)
                {
                    String color = GetCellColor(_model.GetField(i, j));
                    Fields.Add(new SnakeFieldViewModel
                    {
                        Color = color,
                        Row = i,
                        Column = j
                    });
                }
            }
            ActiveGame = true;
        }

        private void RefreshTable()
        {
            for (Int32 i = 0; i < RowCount; i++)
            {
                for (Int32 j = 0; j < ColumnCount; j++)
                {
                    String color = GetCellColor(_model.GetField(i, j));
                    Fields[ColumnCount * i + j].Color = color;
                }
            }
        }

        private String GetCellColor(Cell cellType)
        {
            String color;
            switch (cellType)
            {
                case Cell.WALL: color = "#1e1e1e"; break;
                case Cell.EMPTY: color = "#d2d2d2"; break;
                case Cell.PLAYER: color = "#00a113"; break;
                case Cell.EGG: color = "#ff2400"; break;
                default: color = "#000000"; break;
            }
            return color;
        }
        #endregion
        #region Events
        public event EventHandler<String> NewGame;
        public event EventHandler LoadFile;
        public event EventHandler SaveFile;
        public event EventHandler GamePause;

        public event EventHandler<int> GameOver;
        public event EventHandler<String> DirectionChange;

        private void OnGamePause()
        {
            GamePaused = !GamePaused;
            ButtonText = GamePaused ? "Continue" : "Pause";
            if (GamePause != null)
                GamePause(this, EventArgs.Empty);
        }

        private void OnNewGame(String map)
        {
            if (NewGame != null)
                NewGame(this, map);
        }

        private void OnLoadFile()
        {
            if (LoadFile != null)
                LoadFile(this, EventArgs.Empty);
        }

        private void OnSaveFile()
        {
            if (LoadFile != null)
                SaveFile(this, EventArgs.Empty);
        }

        private void OnDirectionChange(String direction)
        {
            if (DirectionChange != null)
                DirectionChange(this, direction);
        }

        #endregion
        #region Model Events
        private void Model_OnGameCreated(object sender, EventArgs e)
        {
            CreateTable();
            OnGamePause();
        }

        private void Model_OnCounterChanged(object sender, int counter)
        {
            Counter = counter;
        }

        private void Model_OnStepGame(object sender, EventArgs e)
        {
            RefreshTable();
        }

        private void Model_OnGameOver(object sender, EventArgs e)
        {
            OnGamePause();
            ActiveGame = false;
            if (GameOver != null)
                GameOver.Invoke(this, _counter);
        }
        #endregion
    }
}
