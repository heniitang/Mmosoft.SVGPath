﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SVGPath;
using SVGPath.CmdDrawer;

namespace GUI
{
    public partial class frmSvgPath : Form
    {        
        Regex pathRegexer = new Regex("d=\"(.*?)\"");
        private string[] fileNames;

        public frmSvgPath()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                List<Cmd> cmds = CmdParser.Parse(textBox1.Text);
                lbParsedCmds.Items.Clear();
                lbParsedCmds.Items.AddRange(cmds.Select(i => i.ToString()).ToArray());
                List<Cmd> processedCmds = CmdScaler.Scale(CmdConverter.ExtractToSingleAndAbsolutePositionCmds(cmds), 10);                                
                pictureBox1.Image = CmdDrawer.Draw(processedCmds, 81, 81, Brushes.Black);
                lblError.Text = string.Empty;
            }
            catch (InvalidSvgCharacterException isce)
            {
                pictureBox1.Image = null;
                lblError.Text = "Error: " + isce.Message;
            }
            catch (InvalidCmdException invalidCmdEx)
            {
                pictureBox1.Image = null;                
                lbParsedCmds.SelectedIndex = invalidCmdEx.Index;
                lblError.Text = "Error: " + invalidCmdEx.Message;
            }
            catch(Exception ex)
            {
                pictureBox1.Image = null;                
            }
        }       
        
        private void lbShape_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] lines = File.ReadAllLines(fileNames[lbShape.SelectedIndex]);
            Match match;
            foreach (var item in lines)
            {
                if ((match = pathRegexer.Match(item)).Success)
                {
                    textBox1.Text = match.Groups[1].Value;
                    break;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string svgDirectory = "./samples";
            if (Directory.Exists(svgDirectory))
            {
                fileNames = Directory.GetFiles(svgDirectory);
                lbShape.Items.Clear();
                lbShape.Items.AddRange(fileNames.Select(name => Path.GetFileNameWithoutExtension(name)).ToArray());
            }            
        }
    }
}
