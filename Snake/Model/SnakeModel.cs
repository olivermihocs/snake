using System;
using System.Collections.Generic;

using Snake.Persistence;

namespace Snake.Model
{
    public enum Cell
    {
        PLAYER,
        EMPTY,
        WALL,
        EGG,
    }

    public class SnakeModel
    {
        #region Constructor
        public SnakeModel(SnakePersistence persistence)
        {
            _IO = persistence;
        }
        #endregion
        #region Attributes
        public enum Dir { UP = 1, DOWN = 2, LEFT = 3, RIGHT = 4 };

        private Cell[,] _table;
        private int _row;
        private int _col;

        readonly private int _eggMax = 6;
        readonly private int _eggSec = 10;
        private int _currentSec = 0;
        private int _totalEggs = 0;
        private int _counter = 0;

        private SnakePersistence _IO;

        public Dir Direction { get; set; }
        private List<Position> _player = new List<Position>();
        #endregion
        #region Setters and Getters
        /// <summary>
        /// Calculates The field the player is about to step on
        /// </summary>
        private Position Next()
        {
            Position next = _player[0];
            switch (Direction)
            {
                case Dir.UP: --next.i; break;
                case Dir.DOWN: ++next.i; break;
                case Dir.LEFT: --next.j; break;
                case Dir.RIGHT: ++next.j; break;
                default: break;
            }
            return next;
        }

        public Cell GetField(int i, int j)
        {
            return _table[i, j];
        }

        public int GetRow()
        {
            return _row;
        }
        public int GetCol()
        {
            return _col;
        }

        #endregion
        #region Table Management
        /// <summary>
        /// New board initialization
        /// </summary>
        private void LoadBoard(string filePath)
        {
            _player.Clear();
            List<string> input = new List<string>();
            Direction = (Dir)_IO.ReadData(filePath, ref input, ref _row, ref _col, ref _counter, ref _player);
            _table = new Cell[_row, _col];
            for (int i = 0; i < _row; ++i)
            {
                for (int j = 0; j < _col; ++j)
                {
                    Cell cell = Cell.EMPTY;
                    switch (input[i][j])
                    {
                        case 'W': cell = Cell.WALL; break;
                        case 'E': cell = Cell.EMPTY; break;
                        case 'P': cell = Cell.PLAYER; break;
                        case '0': cell = Cell.EGG; ++_totalEggs; break;
                        default: break;
                    }
                    _table[i, j] = cell;
                }
            }
        }

        public void SaveGame(string filePath)
        {
            List<string> output = new List<string>();
            for (int i = 0; i < _row; ++i)
            {
                string current = "";
                for (int j = 0; j < _col; ++j)
                {
                    switch (_table[i, j])
                    {
                        case Cell.EGG: current += '0'; break;
                        case Cell.WALL: current += 'W'; break;
                        case Cell.PLAYER: current += 'P'; break;
                        case Cell.EMPTY: current += 'E'; break;
                        default: break;
                    }
                }
                output.Add(current);
            }
            _IO.WriteData(filePath, output, _counter, (int)Direction, _player);
        }

        /// <summary>
        /// Finds a valid random position for the next egg to spawn on.
        /// </summary>
        private void SpawnEgg()
        {
            if (_totalEggs < _eggMax)
            {
                Random rnd = new Random();
                bool spawned = false;
                do
                {
                    int i = rnd.Next(0, _row);
                    int j = rnd.Next(0, _col);
                    if (_table[i, j] == Cell.EMPTY)
                    {
                        _table[i, j] = Cell.EGG;
                        spawned = true;
                    }
                } while (!spawned);
                ++_totalEggs;
            }
            _currentSec = 0;
        }

        
        public void NewGame(string filePath)
        {
            if (filePath.Length == 6 && filePath.Substring(0, 5) == "//MAP") LoadDefault(filePath[5]);
            else LoadBoard(filePath);
            OnCounterChanged();
            OnGameCreated();
        }

        /// <summary>
        /// Checks if the next position the player is about to step on is valid or not.
        /// </summary>
        private bool IsValid(Position next)
        {
            return (next.i >= 0 && next.i < _row && next.j >= 0 && next.j < _col);
        }

        public void StepGame()
        {
            ++_currentSec;
            Position next = Next();
            if (IsValid(next) && _table[next.i, next.j] != Cell.PLAYER && _table[next.i, next.j] != Cell.WALL)
            {
                bool increaseLength = (_table[next.i, next.j] == Cell.EGG);
                Position tmp = _player[0];
                _table[next.i, next.j] = Cell.PLAYER;
                for (int i = 0; i < _player.Count; ++i)
                {
                    tmp = _player[i];
                    _player[i] = next;
                    next = tmp;
                }
                if (increaseLength)
                {
                    ++_counter;
                    --_totalEggs;
                    OnCounterChanged();
                    _player.Add(next);
                }
                else
                {
                    _table[next.i, next.j] = Cell.EMPTY;
                }
                if (_currentSec >= _eggSec)
                    SpawnEgg();
                OnStepGame();
            }
            else
            {
                OnGameOver();
            }
        }
        #endregion
        #region Events

        public event EventHandler<int> CounterChanged;
        public event EventHandler GameOver;
        public event EventHandler StepGameEvent;
        public event EventHandler GameCreated; 

        
        public void OnGameOver()
        {
            if (GameOver != null)
                GameOver.Invoke(this,EventArgs.Empty);
        }

        public void OnStepGame()
        {
            if (StepGameEvent != null)
                StepGameEvent.Invoke(this, EventArgs.Empty);
        }
        
        public void OnCounterChanged()
        {
            if (CounterChanged != null)
                CounterChanged.Invoke(this, _counter);
        }

        public void OnGameCreated()
        {
            if (GameCreated != null)
                GameCreated(this,EventArgs.Empty);
        }
        #endregion
        #region Default Maps
        
        private void LoadDefault(char map)
        {
            _player.Clear();
            _counter = 0;
            _currentSec = 0;
            _totalEggs = 0;
            Direction = Dir.UP;
            int m = (int)Char.GetNumericValue(map);
            _row = 12 + m*4;
            _col = _row;
            _table = new Cell[_row, _col];

            for (int i = 6; i < 11; ++i) _player.Add(new Position(i, _row / 2 - 1));

            for (int i = 0; i < _row; ++i)
            {
                for (int j = 0; j < _col; ++j)
                {
                    _table[i, j] = Cell.EMPTY;
                }
            }
            if (m == 1)      LoadEasyGame();
            else if (m == 2) LoadMediumGame();
            else if (m == 3) LoadHardGame();
            for (int i = 6; i < 11; ++i) _table[i, _row / 2 - 1] = Cell.PLAYER;
            for (int i = 0; i < 5; ++i) SpawnEgg();
        }

        private void LoadEasyGame()
        {
            for (int j = 0; j < 3; ++j)  _table[2, j]  = Cell.WALL;
            for (int j = 9; j < 14; ++j) _table[2, j]  = Cell.WALL;
            for (int j = 2; j < 6; ++j)  _table[4, j]  = Cell.WALL;
            for (int j = 3; j < 6; ++j)  _table[7, j]  = Cell.WALL;
            for (int j = 13; j < 15; ++j) _table[9, j]  = Cell.WALL;
            for (int j = 3; j < 6; ++j)  _table[13, j] = Cell.WALL;
            for (int j = 7; j < 15; ++j) _table[13, j] = Cell.WALL;
            for (int j = 5; j < 8; ++j)  _table[14, j] = Cell.WALL;
        }
        private void LoadMediumGame()
        {
            LoadEasyGame();
            for (int j = 1; j < 7; ++j) _table[10, j] = Cell.WALL;
            for (int i = 4; i< 7;++i) _table[i,14] = Cell.WALL;
            for (int i = 2; i< 9;++i) _table[i,17] = Cell.WALL;
            for (int i = 2; i < 18; ++i) _table[i, 19] = Cell.WALL;
            for (int i = 3; i < 7; ++i) _table[i, 7] = Cell.WALL;

            for (int j = 2; j < 6; ++j) _table[16, j] = Cell.WALL;
            for (int j = 8; j < 12; ++j) _table[16, j] = Cell.WALL;
            for (int j = 0; j < 4; ++j) _table[18, j] = Cell.WALL;
            for (int j = 5; j < 17; ++j) _table[18, j] = Cell.WALL;
            for (int j = 3; j < 7; ++j) _table[0, j] = Cell.WALL;
        }

        private void LoadHardGame()
        {
            LoadMediumGame();
            for (int j = 21; j < 24; ++j) _table[3, j] = Cell.WALL;
            for (int j = 20; j < 23; ++j) _table[5, j] = Cell.WALL;
            for (int j = 21; j < 24; ++j) _table[8, j] = Cell.WALL;
            for (int j = 20; j < 23; ++j) _table[12, j] = Cell.WALL;

            for (int j = 1; j < 5; ++j) _table[20, j] = Cell.WALL;
            for (int j = 8; j < 11; ++j) _table[22, j] = Cell.WALL;
            for (int j = 13; j < 17; ++j) _table[20, j] = Cell.WALL;

            for (int i = 16; i < 20; ++i) _table[i, 22] = Cell.WALL;
            for (int i = 19; i < 23; ++i) _table[i, 19] = Cell.WALL;
        }

        #endregion
    }
}
