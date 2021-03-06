﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Mania
{
    public partial class StartupScreen : Form
    {
        public StartupScreen()
        {
            this.InitializeComponent();
            Label DropFilePrompt = new Label
            {
                Text = "Drop Beatmap File",
                Font = new Font("Microsoft JhengHei UI", 48),
                Dock = DockStyle.Fill,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Name = "DropFilePrompt"
            };
            this.Controls.Add(DropFilePrompt);
        }

        private void Mania_DragEnter(object sender, DragEventArgs e)
        {
            Label DropFilePrompt = (Label)this.Controls.Find("DropFilePrompt", true).FirstOrDefault();
            DropFilePrompt.Font = new Font("Microsoft JhengHei UI", 48);
            string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop, false);//get dropped file name
            string ext = Path.GetExtension(fileNames[0]);
            if (ext == ".txt" && fileNames.Length == 1)//if one and only txt file is dropped
            {
                e.Effect = DragDropEffects.Copy;
                DropFilePrompt.ForeColor = Color.Green;
                DropFilePrompt.Text = "Drop";
            }
            else if (fileNames.Length != 1)//if more than 1 file is dropped
            {
                e.Effect = DragDropEffects.None;
                DropFilePrompt.ForeColor = Color.Red;
                DropFilePrompt.Text = "No more than 1 file!";
            }
            else//if file is not a txt file
            {
                e.Effect = DragDropEffects.None;
                DropFilePrompt.ForeColor = Color.Red;
                DropFilePrompt.Text = "Not a txt file!";
            }
        }

        private void Mania_DragDrop(object sender, DragEventArgs e)
        {
            Label DropFilePrompt = (Label)this.Controls.Find("DropFilePrompt", true).FirstOrDefault();
            DropFilePrompt.Font = new Font("Microsoft JhengHei UI", 12);
            DropFilePrompt.ForeColor = Color.Red;
            DropFilePrompt.Text = "";
            bool hasErrorOccured = false;//true if any error occured during the parsing of the beatmap file
            bool hasSetBpm = false;
            bool hasSetSpeed = false;
            string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string[] lines = File.ReadAllLines(fileNames[0]);
            List<HitObject> hitObjects = new List<HitObject>();//a list of all `HitObject`s
            for (int i = 0; i < lines.Length; i++)//parse beatmap file
            {
                lines[i] = lines[i].TrimEnd('\n', '\r');//trim newline
                lines[i] = lines[i].TrimStart(' ');//trim leading spaces
                if (string.IsNullOrEmpty(lines[i]))//if line is empty
                {
                    continue;
                }
                int positionOfFirstSoace = lines[i].IndexOf(' ');
                string command;
                if (positionOfFirstSoace == -1)
                {
                    command = lines[i];
                }
                else
                {
                    command = lines[i].Take(positionOfFirstSoace).ToSystemString();//parse the command of a line
                }
                switch (command)
                {
                    case Commands.BPM:
                        if (int.Parse(lines[i].Substring(positionOfFirstSoace + 1)) <= 0)
                        {
                            DropFilePrompt.Text += $"Syntax Error at line {i + 1}: The BPM can't be or less than 0(you set it to {int.Parse(lines[i].Substring(positionOfFirstSoace + 1))})\n";
                            hasErrorOccured = true;
                            break;
                        }
                        GameInfo.Bpm = int.Parse(lines[i].Substring(positionOfFirstSoace + 1));//get bpm argument
                        hasSetBpm = true;
                        break;
                    case Commands.SPEED:
                        if (int.Parse(lines[i].Substring(positionOfFirstSoace + 1)) <= 0)
                        {
                            DropFilePrompt.Text += $"Syntax Error at line {i + 1}: The speed can't be or less than 0(you set it to {int.Parse(lines[i].Substring(positionOfFirstSoace + 1))})\n";
                            hasErrorOccured = true;
                            break;
                        }
                        GameInfo.Speed = float.Parse(lines[i].Substring(positionOfFirstSoace + 1));//get speed argument
                        hasSetSpeed = true;
                        break;
                    case Commands.HO:
                        HitObject currentHitObject = new HitObject();
                        if ((lines[i].Contains(Commands.IHN + "=" + "0") && lines[i].Contains(Commands.HT)) ||
                            (lines[i].Contains(Commands.IHN) == false && lines[i].Contains(Commands.HT)))//if ihn=0 and ht is present at the same time or ht is present while ihn isn't specified
                        {
                            DropFilePrompt.Text += $"Syntax Error at line {i + 1}: ht cannot exist while ihn is false\n";
                            hasErrorOccured = true;
                            break;
                        }
                        string[] arguments = lines[i].Split(' ');
                        for (int j = 1; j < arguments.Length; j++)//loop until all arguments have been parsed(starts from the first argument, all arguments after the command must use a '=' to connect with their parameters and the '=' can't have leading or tailing spaces. e.g. ihn=1)
                        {
                            int parameter = int.Parse(arguments[j].Split('=')[1]);//get the parameter
                            switch (arguments[j].Split('=')[0])//determine the type of the argument
                            {
                                case Commands.BEAT:
                                    if (parameter < 0)
                                    {
                                        DropFilePrompt.Text += $"Syntax Error at line {i + 1} argument {j + 1} \"{arguments[j]}\": The beatstamp can't be less than 0(you set it to {parameter})\n";
                                        hasErrorOccured = true;
                                        break;
                                    }
                                    currentHitObject.BeatStamp = parameter;
                                    break;
                                case Commands.IHN:
                                    if (parameter != 0)//if parameter is true
                                    {
                                        currentHitObject.IsHoldNote = true;
                                    }
                                    else
                                    {
                                        currentHitObject.IsHoldNote = false;
                                    }
                                    break;
                                case Commands.HT:
                                    if (parameter <= 0)
                                    {
                                        DropFilePrompt.Text += $"Syntax Error at line {i + 1} argument {j + 1} \"{arguments[j]}\": The hold time can't be or less than 0(you set it to {parameter})\n";
                                        hasErrorOccured = true;
                                        break;
                                    }
                                    currentHitObject.HoldTime = parameter;
                                    break;
                                case Commands.ROW:
                                    currentHitObject.Row = parameter;
                                    if (parameter > 8 || parameter < 0)
                                    {
                                        DropFilePrompt.Text += $"Syntax Error at line {i + 1} argument {j + 1} \"{arguments[j]}\": Index of the row can only range from 0~8(you set it to {parameter})\n";
                                        hasErrorOccured = true;
                                        break;
                                    }
                                    if (parameter > GameInfo.LargestRowIndex)
                                    {
                                        GameInfo.LargestRowIndex = parameter;
                                    }
                                    break;
                                default:
                                    DropFilePrompt.Text += $"Syntax Error at line {i + 1} argument {j + 1}: Unknown HitObject argument \"{arguments[j]}\"(notice: no spaces allowed before and after the equal sign!)\n";
                                    hasErrorOccured = true;
                                    break;
                            }
                        }
                        if (hasErrorOccured == false)
                        {
                            hitObjects.Add(currentHitObject);//add currentHitObject to hitObjects list
                            GameInfo.TotalNoteCount++;
                        }
                        break;
                    default:
                        DropFilePrompt.Text += $"Syntax Error at line {i + 1}: Unknown Command \"{command}\"\n";
                        hasErrorOccured = true;
                        break;
                }
            }
            if (hasErrorOccured == false && hasSetBpm == true && hasSetSpeed == true)//when the game is ready to start
            {
                DropFilePrompt.Font = new Font("Microsoft JhengHei UI", 48);
                DropFilePrompt.Text = "Play!";
                DropFilePrompt.ForeColor = Color.Green;
                this.AllowDrop = false;
                GameInfo.GameStarted = true;
                hitObjects.Sort((x, y) => { return x.BeatStamp.CompareTo(y.BeatStamp); });//order `HitObject`s by their timing point from the earliest to the latest
                using (Mania mania = new Mania(this, hitObjects))
                {
                    mania.ShowDialog();
                }
            }
            else if (hasErrorOccured == true || hasSetBpm == false || hasSetSpeed == false)//if any error occured
            {
                if (hasSetBpm == false)
                {
                    DropFilePrompt.Text += $"BPM needs to be set\n";
                }
                if (hasSetSpeed == false)
                {
                    DropFilePrompt.Text += $"Speed needs to be set\n";
                }
            }
        }

        private void Mania_DragLeave(object sender, EventArgs e)
        {
            Label DropFilePrompt = (Label)this.Controls.Find("DropFilePrompt", true).FirstOrDefault();
            DropFilePrompt.ForeColor = SystemColors.ControlText;
            DropFilePrompt.Font = new Font("Microsoft JhengHei UI", 48);
            DropFilePrompt.Text = "Drop Beatmap File";
        }
    }
    public class HitObject
    {
        /// <summary>
        /// Specifies the row the <c>HitObject</c> is at.
        /// </summary>
        public int Row { get; set; } = 0;
        /// <summary>
        /// Specifies the offset of the <c>HitObject</c> from the first beat in beats.
        /// </summary>
        public int BeatStamp { get; set; } = 0;
        /// <summary>
        /// If true, this <c>HitObject</c> needs to be held down for <c>HoldTime</c> beats.
        /// </summary>
        public bool IsHoldNote { get; set; } = false;
        /// <summary>
        /// Specifies how long this <c>HitObject</c> needs to be held down in beats. Only works if <c>IsHoldNote</c> is true.
        /// </summary>
        public int? HoldTime { get; set; } = 0;
        /// <summary>
        /// Specifies the error when the <c>HitObject</c> is hit in milliseconds.
        /// </summary>
        public int HitError { get; set; } = 0;
        /// <summary>
        /// Specifies the error when the <c>HitObject</c> is released in milliseconds. Only works if <c>IsHoldNote</c> is true.
        /// </summary>
        public int? ReleaseError { get; set; } = 0;
    }
    public static class GameInfo
    {
        public static int Bpm { get; set; } = 0;
        public static float Speed { get; set; } = 0;
        public static bool GameStarted { get; set; } = false;
        public static int LargestRowIndex { get; set; } = 0;
        public static int NoteWidth { get; set; } = 50;
        public static int JudgementLineHeight { get; set; } = 100;
        public static int NoteProgress { get; set; } = 0;//progress of note
        public static int TotalNoteCount { get; set; } = 0;
        public static int BeatProgress { get; set; } = 0;//progress of beat
        public static int Score { get; set; } = 0;
        public static int Combo { get; set; } = 0;
        public static KeyCount Keys { get; set; } = KeyCount.FourK;
        public static void Reset()
        {
            Bpm = 0;
            Speed = 0;
            GameStarted = false;
            LargestRowIndex = 0;
            NoteProgress = 0;
            TotalNoteCount = 0;
            BeatProgress = 0;
            Score = 0;
            Combo = 0;
            Keys = KeyCount.FourK;
        }
    }
    public enum KeyCount
    {
        OneK,
        TwoK,
        ThreeK,
        FourK,
        FiveK,
        SixK,
        SevenK,
        EightK,
        NineK
    };
    public static class Commands
    {
        public const string ROW = "row";
        public const string BPM = "bpm";
        /// <summary>
        /// <c>HitObject</c>
        /// </summary>
        public const string HO = "ho";
        public const string BEAT = "beat";
        /// <summary>
        /// <c>IsHoldNote</c>
        /// </summary>
        public const string IHN = "ihn";
        /// <summary>
        /// <c>HoldTime</c>
        /// </summary>
        public const string HT = "ht";
        public const string SPEED = "speed";
    }
    public static class StringExtensions
    {
        public static string ToSystemString(this IEnumerable<char> source)
        {
            return new string(source.ToArray());
        }
    }
}
