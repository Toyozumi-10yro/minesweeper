using minesweeper.Properties;

namespace minesweeper
{
    enum BlockState
    {
        Opened, // 開いた状態
        Closed, // 閉じた状態
        Flagged, // 旗が立てられた状態
        Flagged_NG, // 旗が間違って立てられた状態
        Bomb, // 爆弾のブロック
        Bomb_Opened, // 開いた爆弾のブロック
    }
    internal class Positon(int x, int y)
    {
        public int X { get; set; } = x;
        public int Y { get; set; } = y;
    }
    internal class Block : Label
    {
        public Block()
        {
            TextAlign = ContentAlignment.MiddleCenter;
            BackgroundImageLayout = ImageLayout.Stretch;
            Font = new Font("Arial", 15, FontStyle.Bold);
            AutoSize = false;
            SetImage();
        }

        private bool _IsPressed = false;
        public bool _IsOpened = false;
        private BlockState state = BlockState.Closed;
        // ブロックの状態
        public BlockState State
        {
            get { return state; }
            set
            {
                state = value;
                SetImage();
            }
        }
        // 周辺の爆弾の数
        public int BombCount = 0;
        // 爆弾のブロックかどうか
        public bool IsBomb = false;
        // ブロックの位置
        public Positon Positon = new Positon(0, 0);
        // ブロックが押されたかどうか
        public bool IsPressed
        {
            get { return _IsPressed; }
            set
            {
                _IsPressed = value;
                SetImage();
            }
        }
        // ブロックの画像を設定
        private void SetImage()
        {
            if (_IsPressed)
            {
                BackgroundImage = Resources.block_pressed;
                return;
            }
            switch (state)
            {
                case BlockState.Opened:
                    BackgroundImage = Resources.block_pressed;
                    SetBombCountText();

                    break;
                case BlockState.Closed:
                    BackgroundImage = Resources.block;

                    break;

                case BlockState.Flagged:
                    BackgroundImage = Resources.flag;
                    break;
                case BlockState.Flagged_NG:
                    BackgroundImage = Resources.flag;
                    Text = "X";
                    Font = new Font("Arial", 18, FontStyle.Bold);
                    ForeColor = Color.Red;
                    break;
                case BlockState.Bomb:
                    BackgroundImage = Resources.bomb;
                    BackColor = ColorTranslator.FromHtml("#c6c6c6");
                    break;
                case BlockState.Bomb_Opened:
                    BackgroundImage = Resources.bomb;
                    BackColor = Color.Red;
                    break;
            }
        }
        private void SetBombCountText()
        {
            if (BombCount == 0) return;
            Text = BombCount.ToString();
            switch (BombCount)
            {
                case 1:
                    ForeColor = ColorTranslator.FromHtml("#0000F7");
                    break;
                case 2:
                    ForeColor = ColorTranslator.FromHtml("#007C00");
                    break;
                case 3:
                    ForeColor = ColorTranslator.FromHtml("#EC1F1F");
                    break;
                case 4:
                    ForeColor = ColorTranslator.FromHtml("#00007C");
                    break;
                case 5:
                    ForeColor = ColorTranslator.FromHtml("#7C0000");
                    break;
                case 6:
                    ForeColor = ColorTranslator.FromHtml("#007C7C");
                    break;
                case 7:
                    ForeColor = ColorTranslator.FromHtml("#000000");
                    break;
                case 8:
                    ForeColor = ColorTranslator.FromHtml("#7C7C7C");
                    break;

            }
        }
    }
}
