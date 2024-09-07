using Microsoft.VisualStudio.TestTools.UnitTesting;

using SnakeTest.Model;
namespace Snake_UnitTest
{
    [TestClass]
    public class UnitTest
    {
        private SnakeModel _model;

        [TestInitialize]
        public void Initialize()
        {
            _model = new SnakeModel(null);
        }

        #region NewGame()
        [TestMethod]
        public void TestTableSizeAfterLoadGame()
        {
            _model.NewGame("//MAP1");
            Assert.AreEqual(_model.Test_Row, 16);
            Assert.AreEqual(_model.Test_Col, 16);
            Assert.AreEqual(_model.Test_TotalCells, 256);
        }

        [TestMethod]
        public void TestPlayerCellsAfterLoadGame()
        {
            _model.NewGame("//MAP1");
            Assert.AreEqual(_model.Test_PlayerSize, 5);
        }

        [TestMethod]
        public void TestDirectionAfterLoadGame()
        {
            _model.NewGame("//MAP1");
            Assert.AreEqual(_model.Direction, SnakeModel.Dir.UP);
        }

        [TestMethod]
        public void TestCounterAfterLoadGame()
        {
            _model.NewGame("//MAP1");
            Assert.AreEqual(_model.Test_Counter, 0);
        }
        #endregion
        #region StepGame()
        [TestMethod]
        public void TestSimpleStep()
        {
            _model.NewGame("//MAP1");
            _model.StepGame();
            Assert.AreEqual(_model.GetField(10, 7), Cell.EMPTY);
            Assert.AreEqual(_model.GetField(5, 7), Cell.PLAYER);
        }

        [TestMethod]
        public void TestPlayerSizeChanged()
        {
            _model.NewGame("//MAP1");
            bool change = _model.GetField(5, 7) == Cell.EGG;
            _model.StepGame();
            if (change)
            {
                Assert.AreEqual(_model.Test_PlayerSize, 6);
                Assert.AreEqual(_model.Test_Counter, 1);
            }
            else
            {
                Assert.AreEqual(_model.Test_PlayerSize, 5);
                Assert.AreEqual(_model.Test_Counter, 0);
            }
        }

        [TestMethod]
        public void TestChangeDirectionStep()
        {
            _model.NewGame("//MAP1");
            _model.Direction = SnakeModel.Dir.RIGHT;
            _model.StepGame();
            Assert.AreEqual(_model.GetField(6, 8), Cell.PLAYER);
        }
        #endregion
        #region GameOver
        /*
         * For testing this, we use "TEST_gameOver" variable in SnakeModel class 
         * to see if the GameOver event was invoked.
        */

        [TestMethod]
        public void TestCollideWithWall()
        {
            _model.NewGame("//MAP1");
            for (int i = 0; i < 2; ++i) _model.StepGame();
            _model.Direction = SnakeModel.Dir.LEFT;
            for (int i = 0; i < 2; ++i) _model.StepGame();
            Assert.AreEqual(_model.Test_GameOver, true);
        }

        [TestMethod]
        public void TestCollideWithPlayer()
        {
            _model.NewGame("//MAP1");
            _model.Direction = SnakeModel.Dir.DOWN;
            _model.StepGame();
            Assert.AreEqual(_model.Test_GameOver, true);
        }

        [TestMethod]
        public void TestOutOfIndex()
        {
            _model.NewGame("//MAP1");
            for (int i = 0; i < 7; i++) _model.StepGame();
            Assert.AreEqual(_model.Test_GameOver, true);
        }

        #endregion

    }
}
