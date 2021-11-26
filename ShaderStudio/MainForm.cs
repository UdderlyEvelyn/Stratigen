#region File Description
//-----------------------------------------------------------------------------
// MainForm.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace ShaderStudio
{
    // System.Drawing and the XNA Framework both define Color types.
    // To avoid conflicts, we define shortcut names for them both.
    using GdiColor = System.Drawing.Color;
    using XnaColor = Microsoft.Xna.Framework.Color;

    
    /// <summary>
    /// Custom form provides the main user interface for the program.
    /// In this sample we used the designer to add a splitter pane to the form,
    /// which contains a SpriteFontControl and a SpinningTriangleControl.
    /// </summary>
    public partial class MainForm : Form
    {        
        public MainForm()
        {
            InitializeComponent();
            tbXl.Value = -Globals.LightPosition.Xi;
            tbYl.Value = Globals.LightPosition.Yi;
            tbZl.Value = Globals.LightPosition.Zi;
            lbXl.Text = Math.Round(Globals.LightPosition.X, 2).ToString();
            lbYl.Text = Math.Round(Globals.LightPosition.Y, 2).ToString();
            lbZl.Text = Math.Round(Globals.LightPosition.Z, 2).ToString(); 
            tbXc.Value = Globals.CameraPosition.Xi;
            tbYc.Value = Globals.CameraPosition.Yi;
            tbZc.Value = Globals.CameraPosition.Zi;
            lbXc.Text = Math.Round(Globals.CameraPosition.X, 2).ToString();
            lbYc.Text = Math.Round(Globals.CameraPosition.Y, 2).ToString();
            lbZc.Text = Math.Round(Globals.CameraPosition.Z, 2).ToString();
            tbXt.Value = Globals.TrianglePosition.Xi;
            tbYt.Value = Globals.TrianglePosition.Yi;
            tbZt.Value = Globals.TrianglePosition.Zi;
            lbXt.Text = Math.Round(Globals.TrianglePosition.X, 2).ToString();
            lbYt.Text = Math.Round(Globals.TrianglePosition.Y, 2).ToString();
            lbZt.Text = Math.Round(Globals.TrianglePosition.Z, 2).ToString();
            sceneControl.Invalidated += sceneControl_Invalidated;
        }

        void sceneControl_Invalidated(object sender, InvalidateEventArgs e)
        {
            if (chkAuto.Checked)
            {
                Globals.Rotation.X = MathHelper.WrapAngle(Globals.Rotation.X + .0001f);
                Globals.Rotation.Y = MathHelper.WrapAngle(Globals.Rotation.Y + .0001f);
                Globals.Rotation.Z = MathHelper.WrapAngle(Globals.Rotation.Z + .0001f);
                lbXv.Text = (Math.Round(Globals.Rotation.X / MathHelper.TwoPi, 2) * 100) + "%";
                lbYv.Text = (Math.Round(Globals.Rotation.Y / MathHelper.TwoPi, 2) * 100) + "%";
                lbZv.Text = (Math.Round(Globals.Rotation.Z / MathHelper.TwoPi, 2) * 100) + "%";
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tbRotX_Scroll(object sender, EventArgs e)
        {
            Globals.Rotation.X = MathHelper.ToRadians(tbRotX.Value);
            lbXv.Text = (Math.Round(Globals.Rotation.X / MathHelper.TwoPi, 2) * 100) + "%";
        }

        private void tbRotY_Scroll(object sender, EventArgs e)
        {
            Globals.Rotation.Y = MathHelper.ToRadians(tbRotY.Value);
            lbYv.Text = (Math.Round(Globals.Rotation.Y / MathHelper.TwoPi, 2) * 100) + "%";
        }

        private void tbRotZ_Scroll(object sender, EventArgs e)
        {
            Globals.Rotation.Z = MathHelper.ToRadians(tbRotZ.Value);
            lbZv.Text = (Math.Round(Globals.Rotation.Z / MathHelper.TwoPi, 2) * 100) + "%";
        }

        private void tbXl_Scroll(object sender, EventArgs e)
        {
            Globals.LightPosition.X = -tbXl.Value;
            lbXl.Text = Math.Round(-Globals.LightPosition.X, 2).ToString();
        }

        private void tbYl_Scroll(object sender, EventArgs e)
        {
            Globals.LightPosition.Y = tbYl.Value;
            lbYl.Text = Math.Round(Globals.LightPosition.Y, 2).ToString();
        }

        private void tbZl_Scroll(object sender, EventArgs e)
        {
            Globals.LightPosition.Z = tbZl.Value;
            lbZl.Text = Math.Round(Globals.LightPosition.Z, 2).ToString();
        }

        private void tbXc_Scroll(object sender, EventArgs e)
        {
            Globals.CameraPosition.X = tbXc.Value;
            lbXc.Text = Math.Round(Globals.CameraPosition.X, 2).ToString();
        }

        private void tbYc_Scroll(object sender, EventArgs e)
        {
            Globals.CameraPosition.Y = tbYc.Value;
            lbYc.Text = Math.Round(Globals.CameraPosition.Y, 2).ToString();
        }

        private void tbZc_Scroll(object sender, EventArgs e)
        {
            Globals.CameraPosition.Z = tbZc.Value;
            lbZc.Text = Math.Round(Globals.CameraPosition.Z, 2).ToString();
        }

        private void tbXt_Scroll(object sender, EventArgs e)
        {
            Globals.TrianglePosition.X = tbXt.Value;
            lbXt.Text = Math.Round(Globals.TrianglePosition.X, 2).ToString();
        }

        private void tbYt_Scroll(object sender, EventArgs e)
        {
            Globals.TrianglePosition.Y = tbYt.Value;
            lbYt.Text = Math.Round(Globals.TrianglePosition.Y, 2).ToString();
        }

        private void tbZt_Scroll(object sender, EventArgs e)
        {
            Globals.TrianglePosition.Z = tbZt.Value;
            lbZt.Text = Math.Round(Globals.TrianglePosition.Z, 2).ToString();
        }
    }
}
