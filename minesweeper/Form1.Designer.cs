namespace minesweeper
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtBombCount = new DmitryBrant.CustomControls.SevenSegmentArray();
            infoArea = new Panel();
            btnGame = new Button();
            txtBombTimer = new DmitryBrant.CustomControls.SevenSegmentArray();
            blockArea = new Panel();
            infoArea.SuspendLayout();
            SuspendLayout();
            // 
            // txtBombCount
            // 
            txtBombCount.ArrayCount = 3;
            txtBombCount.ColorBackground = Color.Black;
            txtBombCount.ColorDark = Color.Black;
            txtBombCount.ColorLight = Color.Red;
            txtBombCount.DecimalShow = true;
            txtBombCount.ElementPadding = new Padding(4);
            txtBombCount.ElementWidth = 10;
            txtBombCount.ItalicFactor = 0F;
            txtBombCount.Location = new Point(12, 12);
            txtBombCount.Name = "txtBombCount";
            txtBombCount.Size = new Size(98, 59);
            txtBombCount.TabIndex = 1;
            txtBombCount.TabStop = false;
            txtBombCount.Value = "001";
            // 
            // infoArea
            // 
            infoArea.BackColor = SystemColors.AppWorkspace;
            infoArea.Controls.Add(btnGame);
            infoArea.Controls.Add(txtBombTimer);
            infoArea.Controls.Add(txtBombCount);
            infoArea.Dock = DockStyle.Top;
            infoArea.Location = new Point(0, 0);
            infoArea.Name = "infoArea";
            infoArea.Size = new Size(317, 78);
            infoArea.TabIndex = 2;
            // 
            // btnGame
            // 
            btnGame.Anchor = AnchorStyles.Top;
            btnGame.BackgroundImage = Properties.Resources.button_default;
            btnGame.BackgroundImageLayout = ImageLayout.Stretch;
            btnGame.FlatAppearance.BorderSize = 0;
            btnGame.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnGame.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnGame.FlatStyle = FlatStyle.Flat;
            btnGame.Location = new Point(130, 12);
            btnGame.Name = "btnGame";
            btnGame.Size = new Size(58, 59);
            btnGame.TabIndex = 2;
            btnGame.UseVisualStyleBackColor = true;
            btnGame.Click += btnGame_Click;
            // 
            // txtBombTimer
            // 
            txtBombTimer.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtBombTimer.ArrayCount = 3;
            txtBombTimer.ColorBackground = Color.Black;
            txtBombTimer.ColorDark = Color.Black;
            txtBombTimer.ColorLight = Color.Red;
            txtBombTimer.DecimalShow = true;
            txtBombTimer.ElementPadding = new Padding(4);
            txtBombTimer.ElementWidth = 10;
            txtBombTimer.ItalicFactor = 0F;
            txtBombTimer.Location = new Point(207, 12);
            txtBombTimer.Name = "txtBombTimer";
            txtBombTimer.Size = new Size(98, 59);
            txtBombTimer.TabIndex = 1;
            txtBombTimer.TabStop = false;
            txtBombTimer.Value = "000";
            // 
            // blockArea
            // 
            blockArea.Dock = DockStyle.Fill;
            blockArea.Location = new Point(0, 78);
            blockArea.Name = "blockArea";
            blockArea.Size = new Size(317, 506);
            blockArea.TabIndex = 3;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(317, 584);
            Controls.Add(blockArea);
            Controls.Add(infoArea);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "まいんすいぱー";
            Load += Form1_Load;
            infoArea.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private DmitryBrant.CustomControls.SevenSegmentArray txtBombCount;
        private Panel infoArea;
        private DmitryBrant.CustomControls.SevenSegmentArray txtBombTimer;
        private Panel blockArea;
        private Button btnGame;
    }
}
