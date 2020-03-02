using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mania
{
    public partial class Mania : Form
    {
        new public Form ParentForm { get; set; }
        public TableLayoutPanel RowsLayout = new TableLayoutPanel();
        public List<HitObject> HitObjects;
        public Mania(Form parent, List<HitObject> hitObjects)
        {
            this.InitializeComponent();
            this.Size = new Size(GameInfo.NoteWidth * (GameInfo.LargestRowIndex + 1), 800);
            this.ParentForm = parent;
            this.HitObjects = hitObjects;
            this.RowsLayout.ColumnCount = GameInfo.LargestRowIndex + 1;
            this.RowsLayout.RowCount = 1;
            this.RowsLayout.Location = new Point(0, 0);
            this.RowsLayout.Anchor = AnchorStyles.None;
            this.RowsLayout.Dock = DockStyle.Fill;
            this.RowsLayout.Size = new Size(this.Size.Width, this.Size.Height);
            this.RowsLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            this.RowsLayout.BackColor = Color.Transparent;
            this.RowsLayout.RowStyles.Clear();
            this.RowsLayout.ColumnStyles.Clear();
            for (int i = 0; i < GameInfo.LargestRowIndex + 1; i++)
            {
                this.RowsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100 / (GameInfo.LargestRowIndex + 1)));
            }
            this.Controls.Add(this.RowsLayout);
            this.RowsLayout.Paint += new PaintEventHandler(this.RowsLayout_Paint);

            for (int i = 0; i < GameInfo.LargestRowIndex + 1; i++)//add row panels to `RowsLayout`
            {
                Panel row = new Panel
                {
                    Name = "row" + $"{i}",
                    Anchor = AnchorStyles.None,
                    Dock = DockStyle.Fill
                };
                row.Paint += new PaintEventHandler(this.Row_Paint);
                this.RowsLayout.Controls.Add(row, i, 0);
            }
            //this.Start_Countdown();
            this.Start_Game();
        }

        private void RowsLayout_Paint(object sender, PaintEventArgs e)
        {
            Pen judgementLinePen = new Pen(Color.Green, 2);
            TableLayoutPanel raiser = (TableLayoutPanel)sender;
            e.Graphics.DrawLine(judgementLinePen, 0, raiser.Height - GameInfo.JudgementLineHeight, raiser.Width, raiser.Height - GameInfo.JudgementLineHeight);
        }

        private void Row_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void Mania_Load(object sender, EventArgs e)
        {
            
        }

        private void Mania_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.ParentForm.AllowDrop = true;
            Label DropFilePrompt = (Label)this.ParentForm.Controls.Find("DropFilePrompt", true).FirstOrDefault();
            DropFilePrompt.Font = new Font("Microsoft JhengHei UI", 48);
            DropFilePrompt.ForeColor = Color.Green;
            DropFilePrompt.Text = "Drop Beatmap File";
            GameInfo.Reset();
        }

        private void Mania_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Mania_KeyUp(object sender, KeyEventArgs e)
        {

        }
        private void Start_Countdown()
        {
            this.Countdown.Visible = true;
            this.Countdown.Parent = this.RowsLayout;
            Panel firstRow = (Panel)this.Controls.Find("row0", true).FirstOrDefault();
            if (firstRow != null)
            {
                firstRow.Controls.Add(this.Countdown);
            }
            this.Countdown.Dock = DockStyle.Fill;
            this.Countdown.TextAlign = ContentAlignment.MiddleCenter;
            this.Countdown.AutoSize = false;
            this.Countdown.BringToFront();
            Timer countdown = new Timer
            {
                Interval = 1000
            };
            countdown.Start();
            countdown.Tick += new EventHandler((object sender, EventArgs e) => 
            {
                Timer timer = (Timer)sender;
                int num = int.Parse(this.Countdown.Text);
                num--;
                this.Countdown.Text = num.ToString();
                if (num == 0)//when timer reaches 0
                {
                    this.Countdown.Visible = false;
                    timer.Stop();
                    return;
                }
            });
        }
        private void Start_Game()
        {
            Timer noteSpawnTimer = new Timer
            {
                Interval = 1000 / (GameInfo.Bpm / 60)//updates every beat
            };
            noteSpawnTimer.Tick += new EventHandler(NoteSpawnTimer_Tick);

            Timer updateNotesTimer = new Timer()
            {
                Interval = 1000 / 60//60fps
            };
            updateNotesTimer.Tick += new EventHandler((object sender, EventArgs e) =>
            {
                Timer timer = (Timer)sender;
            });
            noteSpawnTimer.Start();
            updateNotesTimer.Start();
        }
        private void NoteSpawnTimer_Tick(object sender, EventArgs e)
        {
            Timer timer = (Timer)sender;
            if (this.HitObjects[GameInfo.NoteProgress].BeatStamp == GameInfo.BeatProgress)//if there's a note needed to be spawned on this beat
            {
                Panel row = (Panel)RowsLayout.Controls.Find("row" + HitObjects[GameInfo.NoteProgress].Row, true).FirstOrDefault();//get the reference to the row the current spawning note belongs to
                PictureBox note = new PictureBox()
                {
                    BackColor = Color.Red,
                    Location = new Point(0, 0),
                    Anchor = AnchorStyles.None,
                    Parent = row
                };
                if (HitObjects[GameInfo.NoteProgress].IsHoldNote == false)
                {
                    if (row != null)
                    {
                        note.Size = new Size(row.Width, 10);
                    }
                }
                //todo: add ihn spawning, fix stack overflow
                if (row != null)
                {
                    row.Controls.Add(note);
                    if (HitObjects.Count - 1 == GameInfo.NoteProgress)//if all notes have finished spawning
                    {
                        timer.Stop();
                    }
                }
                if (GameInfo.NoteProgress < HitObjects.Count - 1)
                {
                    if (HitObjects[GameInfo.NoteProgress + 1].BeatStamp == GameInfo.BeatProgress && HitObjects.Count - 1 != GameInfo.NoteProgress)//if the next note also needs to be spawned
                    {
                        NoteSpawnTimer_Tick(sender, e);
                    }
                    GameInfo.BeatProgress++;
                    GameInfo.NoteProgress++;
                }
            }
            else//if no note needs to be spawned
            {
                GameInfo.BeatProgress++;
            }
        }
    }
}
