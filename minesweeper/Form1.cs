using minesweeper.Properties;
using Timer = System.Windows.Forms.Timer;

namespace minesweeper
{
    public partial class Form1 : Form
    {

        enum GameStatus
        {
            Ready,
            Playing,
            GameEnd,
        }
        // 横のブロック数
        const int xLength = 9;
        // 縦のブロック数
        const int yLength = 9;
        // 爆弾の数
        const int bombCount = 10;
        // １ブロックのサイズ
        const int BlockSixe = 50;
        // タイマー
        Timer timer = new Timer();

        GameStatus gameState = GameStatus.Ready;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer.Interval = 1000; // 1秒
            txtBombCount.Value = "";
            timer.Tick += Timer_tick;
            InitGameFiled();
        }

        /// <summary>
        /// ゲームの経過時間のタイマー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Timer_tick(object? sender, EventArgs e)
        {
            this.txtBombTimer.Value = (int.Parse(this.txtBombTimer.Value) + 1).ToString();
        }

        /// <summary>
        /// ゲームフィールドの初期化
        /// </summary>
        private void InitGameFiled()
        {
            this.txtBombCount.Value = bombCount.ToString();
            this.txtBombTimer.Value = "0";
            timer.Stop();
            this.gameState = GameStatus.Ready;
            blockArea.Controls.Clear();
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    var label = new Block();
                    label.Name = "Block" + x + y;
                    label.Size = new Size(BlockSixe, BlockSixe);
                    label.Location = new Point(x * BlockSixe, y * BlockSixe);
                    label.Positon = new Positon(x, y);
                    label.MouseUp += Block_MouseUp;
                    // マウスダウン時のイベント。
                    label.MouseDown += (sender, e) =>
                    {
                        if (this.gameState == GameStatus.GameEnd) return;
                        if ((e.Button & MouseButtons.Left) != MouseButtons.Left) return;
                        // ゲーム開始
                        if (this.gameState == GameStatus.Ready)
                        {
                            this.gameState = GameStatus.Playing;
                            timer.Start();
                        }
                        PressBlock(label);
                    };
                    label.MouseMove += Block_MouseMove;

                    blockArea.Controls.Add(label);
                }
            }
            // 爆弾の位置をランダムに決定
            Random rnd = new Random();
            var setBombCount = 0;
            while (setBombCount != bombCount)
            {
                int x = rnd.Next(0, xLength);
                int y = rnd.Next(0, yLength);
                var block = (Block?)blockArea.Controls["Block" + x + y];

                // 既に設定されている場合はスキップ
                if (block.IsBomb)
                {
                    continue;
                }
                setBombCount++;
                block.IsBomb = true;
            }

            // 爆弾の周囲のマスに数字を表示
            foreach (Control control in blockArea.Controls)
            {
                if (control == null && (control is not Block)) return;
                Block block = (Block)control;
                foreach (var target in GetAroundBlok(block))
                {
                    if (target.IsBomb) block.BombCount++;
                }
            }

            this.Size = new Size(BlockSixe * xLength + 20, BlockSixe * yLength + infoArea.Height + SystemInformation.CaptionHeight + 20);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        /// <summary>
        ///　指定したブロックの周囲のブロックを取得
        /// </summary>
        /// <param name="block">基準のブロック</param>
        /// <returns></returns>
        private List<Block> GetAroundBlok(Block block)
        {
            List<Block> blocks = new List<Block>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (block.Positon.X + x < 0 || block.Positon.X + x >= xLength || block.Positon.Y + y < 0 || block.Positon.Y + y >= yLength) continue;
                    var target = (Block?)blockArea.Controls["Block" + (block.Positon.X + x) + (block.Positon.Y + y)];
                    if (target == null) return blocks;
                    blocks.Add(target);
                }
            }
            return blocks;
        }

        /// <summary>
        /// 現在のマウスカーソルにあるブロックを取得
        /// </summary>
        /// <returns>対象のブロックのリスト</returns>
        private Block? GetActiveBlock()
        {
            Control? control = blockArea.GetChildAtPoint(blockArea.PointToClient(Cursor.Position));
            if (control == null && (control is not Block)) return null;
            return (Block)control;
        }

        /// <summary>
        /// マウスアップ時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Block_MouseUp(object? sender, MouseEventArgs e)
        {
            if (this.gameState == GameStatus.GameEnd) return;
            ResetPreessedBlock(null);
            var block = this.GetActiveBlock();
            if (block == null) return;

            // 左クリックで旗以外をクリックした時
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left && block.State != BlockState.Flagged)
            {
                // 爆弾の場合ゲームオーバー
                if (block.IsBomb)
                {
                    GameOver(block);
                    return;
                }

                // 数字ブロックの場合は一括で開く
                if (block.State == BlockState.Opened && CanBlockOpen(block))
                {
                    OpenBlock(block, true);
                    CheckGameClear();
                    return;
                }

                if (block.BombCount == 0)
                {
                    OpenBlock(block, false);
                }
                else
                {
                    block.State = BlockState.Opened;
                }
            }

            // 左クリックで数字をクリックした時
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right && block.State != BlockState.Opened)
            {
                if (block.State == BlockState.Flagged)
                {
                    block.State = BlockState.Closed;
                }
                else
                {
                    block.State = BlockState.Flagged;
                }
            }

            // クリア判定
            CheckGameClear();
        }

        /// <summary>
        /// 左クリック押した状態でマウス移動時はブロックを押下状態にする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Block_MouseMove(object? sender, MouseEventArgs e)
        {
            if (this.gameState == GameStatus.GameEnd) return;
            if ((Control.MouseButtons & MouseButtons.Left) != MouseButtons.Left) return;
            var block = GetActiveBlock();
            if (block == null) return;
            if (block.State == BlockState.Opened) return;
            block.IsPressed = true;
            ResetPreessedBlock(block);
        }

        /// <summary>
        /// ブロックの押下状態をリセット
        /// </summary>
        /// <param name="current">対象外にするブロック</param>
        private void ResetPreessedBlock(Block? current)
        {
            foreach (Control control in blockArea.Controls)
            {
                if (control == null && (control is not Block)) return;
                Block block = (Block)control;
                if (block.State == BlockState.Closed && current != control)
                {
                    ((Block)control).IsPressed = false;
                }
            }
        }

        /// <summary>
        /// ブロックを押す（クリックが確定してない情報）
        /// </summary>
        /// <param name="block"></param>
        private void PressBlock(Block block)
        {
            if (block == null) return;
            if (block.State == BlockState.Closed)
            {
                block.IsPressed = true;
            }
            else
            {
                foreach (var target in GetAroundBlok(block))
                {
                    if (target.State == BlockState.Closed)
                    {
                        target.IsPressed = true;
                    }
                }
            }
        }
        /// <summary>
        /// 周囲のブロックを開く。
        /// </summary>
        /// <param name="block">基準のブロック</param>
        /// <param name="isOpenBomb">爆弾のあるブロックは開くか</param>
        private void OpenBlock(Block block, bool isOpenBomb)
        {
            if (block == null) return;

            foreach (var target in GetAroundBlok(block))
            {
                if (target.State != BlockState.Closed) continue;
                if (isOpenBomb)
                {
                    if (target.IsBomb)
                    {
                        GameOver(target);
                        return;
                    }
                }
                else
                {
                    if (target.IsBomb) continue;
                }

                target.State = BlockState.Opened;
                if (target.BombCount == 0)
                {
                    OpenBlock(target, isOpenBomb);
                }
            }
        }

        /// <summary>
        /// 旗と周囲の爆弾の数が一致しているかどうか
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private Boolean CanBlockOpen(Block block)
        {
            if (block == null) return false;
            int flagCount = 0;
            foreach (var target in GetAroundBlok(block))
            {
                if (target.State == BlockState.Flagged)
                {
                    flagCount++;
                }
            }
            return block.BombCount == flagCount;
        }

        /// <summary>
        /// ゲームクリアかどうか
        /// </summary>
        private void CheckGameClear()
        {
            int flagCount = 0;
            int openCount = 0;
            int unmacthCount = 0;
            foreach (Control control in blockArea.Controls)
            {
                if (control == null && (control is not Block)) return;
                Block block = (Block)control;
                if (block.State == BlockState.Flagged)
                {
                    flagCount++;
                }
                if (block.State == BlockState.Opened)
                {
                    openCount++;
                }

                if (block.State == BlockState.Flagged && !block.IsBomb)
                {
                    unmacthCount++;
                }
            }
            // 間違った旗の数がないかつ全ての爆弾以外のブロックが開かれている場合はクリア
            if (unmacthCount == 0 && openCount == xLength * yLength - bombCount)
            {
                timer.Stop();
                this.btnGame.BackgroundImage = Resources.button_clear;
                this.gameState = GameStatus.GameEnd;

                foreach (Control control in blockArea.Controls)
                {
                    if (control == null && (control is not Block)) return;
                    Block block = (Block)control;
                    if (block.IsBomb)
                    {
                        block.State = BlockState.Flagged;
                    }
                }
                this.txtBombCount.Value = "0";
            }
            else
            {
                this.txtBombCount.Value = (bombCount - flagCount).ToString();
            }
        }

        /// <summary>
        /// ゲームオーバー
        /// </summary>
        /// <param name="currentBlock"></param>
        private void GameOver(Block currentBlock)
        {
            timer.Stop();
            foreach (Control control in blockArea.Controls)
            {
                if (control == null && (control is not Block)) return;
                Block block = (Block)control;

                // 爆発したブロックを赤くする
                if (block == currentBlock)
                {
                    block.State = BlockState.Bomb_Opened;
                    continue;
                }

                // 間違いの旗を表示
                if (block.State == BlockState.Flagged && !block.IsBomb)
                {
                    block.State = BlockState.Flagged_NG;
                    continue;
                }

                // 未開放の爆弾の位置を表示
                if (block.State != BlockState.Flagged && block.IsBomb)
                {
                    block.State = BlockState.Bomb;
                    continue;
                }
                // 閉じているブロックを開く
                if (block.State == BlockState.Closed)
                {
                    block.State = BlockState.Opened;
                    continue;
                }
            }
            this.btnGame.BackgroundImage = Resources.button_gameorver;
            this.gameState = GameStatus.GameEnd;
        }

        /// <summary>
        /// ゲームリセット
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGame_Click(object sender, EventArgs e)
        {
            InitGameFiled();
            this.btnGame.BackgroundImage = Resources.button_default;
        }
    }
}
