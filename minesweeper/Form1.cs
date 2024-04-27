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
        // ���̃u���b�N��
        const int xLength = 9;
        // �c�̃u���b�N��
        const int yLength = 9;
        // ���e�̐�
        const int bombCount = 10;
        // �P�u���b�N�̃T�C�Y
        const int BlockSixe = 50;
        // �^�C�}�[
        Timer timer = new Timer();

        GameStatus gameState = GameStatus.Ready;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer.Interval = 1000; // 1�b
            txtBombCount.Value = "";
            timer.Tick += Timer_tick;
            InitGameFiled();
        }

        /// <summary>
        /// �Q�[���̌o�ߎ��Ԃ̃^�C�}�[
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Timer_tick(object? sender, EventArgs e)
        {
            this.txtBombTimer.Value = (int.Parse(this.txtBombTimer.Value) + 1).ToString();
        }

        /// <summary>
        /// �Q�[���t�B�[���h�̏�����
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
                    // �}�E�X�_�E�����̃C�x���g�B
                    label.MouseDown += (sender, e) =>
                    {
                        if (this.gameState == GameStatus.GameEnd) return;
                        if ((e.Button & MouseButtons.Left) != MouseButtons.Left) return;
                        // �Q�[���J�n
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
            // ���e�̈ʒu�������_���Ɍ���
            Random rnd = new Random();
            var setBombCount = 0;
            while (setBombCount != bombCount)
            {
                int x = rnd.Next(0, xLength);
                int y = rnd.Next(0, yLength);
                var block = (Block?)blockArea.Controls["Block" + x + y];

                // ���ɐݒ肳��Ă���ꍇ�̓X�L�b�v
                if (block.IsBomb)
                {
                    continue;
                }
                setBombCount++;
                block.IsBomb = true;
            }

            // ���e�̎��͂̃}�X�ɐ�����\��
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
        ///�@�w�肵���u���b�N�̎��͂̃u���b�N���擾
        /// </summary>
        /// <param name="block">��̃u���b�N</param>
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
        /// ���݂̃}�E�X�J�[�\���ɂ���u���b�N���擾
        /// </summary>
        /// <returns>�Ώۂ̃u���b�N�̃��X�g</returns>
        private Block? GetActiveBlock()
        {
            Control? control = blockArea.GetChildAtPoint(blockArea.PointToClient(Cursor.Position));
            if (control == null && (control is not Block)) return null;
            return (Block)control;
        }

        /// <summary>
        /// �}�E�X�A�b�v���̃C�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Block_MouseUp(object? sender, MouseEventArgs e)
        {
            if (this.gameState == GameStatus.GameEnd) return;
            ResetPreessedBlock(null);
            var block = this.GetActiveBlock();
            if (block == null) return;

            // ���N���b�N�Ŋ��ȊO���N���b�N������
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left && block.State != BlockState.Flagged)
            {
                // ���e�̏ꍇ�Q�[���I�[�o�[
                if (block.IsBomb)
                {
                    GameOver(block);
                    return;
                }

                // �����u���b�N�̏ꍇ�͈ꊇ�ŊJ��
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

            // ���N���b�N�Ő������N���b�N������
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

            // �N���A����
            CheckGameClear();
        }

        /// <summary>
        /// ���N���b�N��������ԂŃ}�E�X�ړ����̓u���b�N��������Ԃɂ���
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
        /// �u���b�N�̉�����Ԃ����Z�b�g
        /// </summary>
        /// <param name="current">�ΏۊO�ɂ���u���b�N</param>
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
        /// �u���b�N�������i�N���b�N���m�肵�ĂȂ����j
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
        /// ���͂̃u���b�N���J���B
        /// </summary>
        /// <param name="block">��̃u���b�N</param>
        /// <param name="isOpenBomb">���e�̂���u���b�N�͊J����</param>
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
        /// ���Ǝ��͂̔��e�̐�����v���Ă��邩�ǂ���
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
        /// �Q�[���N���A���ǂ���
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
            // �Ԉ�������̐����Ȃ����S�Ă̔��e�ȊO�̃u���b�N���J����Ă���ꍇ�̓N���A
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
        /// �Q�[���I�[�o�[
        /// </summary>
        /// <param name="currentBlock"></param>
        private void GameOver(Block currentBlock)
        {
            timer.Stop();
            foreach (Control control in blockArea.Controls)
            {
                if (control == null && (control is not Block)) return;
                Block block = (Block)control;

                // ���������u���b�N��Ԃ�����
                if (block == currentBlock)
                {
                    block.State = BlockState.Bomb_Opened;
                    continue;
                }

                // �ԈႢ�̊���\��
                if (block.State == BlockState.Flagged && !block.IsBomb)
                {
                    block.State = BlockState.Flagged_NG;
                    continue;
                }

                // ���J���̔��e�̈ʒu��\��
                if (block.State != BlockState.Flagged && block.IsBomb)
                {
                    block.State = BlockState.Bomb;
                    continue;
                }
                // ���Ă���u���b�N���J��
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
        /// �Q�[�����Z�b�g
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
