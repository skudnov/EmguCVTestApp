namespace EmguCVTestApp
{
    partial class ViewForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Panel ToolPanel;
            this.TiltAngleLbl = new System.Windows.Forms.Label();
            this.CameraListCB = new System.Windows.Forms.ComboBox();
            this.ViewBox = new Emgu.CV.UI.ImageBox();
            this.lb_qr = new System.Windows.Forms.Label();
            this.lb_aruco = new System.Windows.Forms.Label();
            ToolPanel = new System.Windows.Forms.Panel();
            ToolPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ViewBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ToolPanel
            // 
            ToolPanel.Controls.Add(this.lb_aruco);
            ToolPanel.Controls.Add(this.lb_qr);
            ToolPanel.Controls.Add(this.TiltAngleLbl);
            ToolPanel.Controls.Add(this.CameraListCB);
            ToolPanel.Dock = System.Windows.Forms.DockStyle.Top;
            ToolPanel.Location = new System.Drawing.Point(0, 0);
            ToolPanel.Margin = new System.Windows.Forms.Padding(2);
            ToolPanel.Name = "ToolPanel";
            ToolPanel.Size = new System.Drawing.Size(936, 81);
            ToolPanel.TabIndex = 0;
            // 
            // TiltAngleLbl
            // 
            this.TiltAngleLbl.Location = new System.Drawing.Point(302, 13);
            this.TiltAngleLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.TiltAngleLbl.Name = "TiltAngleLbl";
            this.TiltAngleLbl.Size = new System.Drawing.Size(115, 19);
            this.TiltAngleLbl.TabIndex = 1;
            // 
            // CameraListCB
            // 
            this.CameraListCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CameraListCB.FormattingEnabled = true;
            this.CameraListCB.Location = new System.Drawing.Point(10, 11);
            this.CameraListCB.Margin = new System.Windows.Forms.Padding(2);
            this.CameraListCB.Name = "CameraListCB";
            this.CameraListCB.Size = new System.Drawing.Size(280, 21);
            this.CameraListCB.TabIndex = 0;
            this.CameraListCB.SelectionChangeCommitted += new System.EventHandler(this.CameraListCB_SelectionChangeCommitted);
            // 
            // ViewBox
            // 
            this.ViewBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ViewBox.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            this.ViewBox.Location = new System.Drawing.Point(0, 81);
            this.ViewBox.Margin = new System.Windows.Forms.Padding(2);
            this.ViewBox.Name = "ViewBox";
            this.ViewBox.Size = new System.Drawing.Size(936, 369);
            this.ViewBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ViewBox.TabIndex = 2;
            this.ViewBox.TabStop = false;
            // 
            // lb_qr
            // 
            this.lb_qr.AutoSize = true;
            this.lb_qr.Location = new System.Drawing.Point(403, 19);
            this.lb_qr.Name = "lb_qr";
            this.lb_qr.Size = new System.Drawing.Size(35, 13);
            this.lb_qr.TabIndex = 2;
            this.lb_qr.Text = "label1";
            this.lb_qr.Visible = false;
            // 
            // lb_aruco
            // 
            this.lb_aruco.AutoSize = true;
            this.lb_aruco.Location = new System.Drawing.Point(612, 19);
            this.lb_aruco.Name = "lb_aruco";
            this.lb_aruco.Size = new System.Drawing.Size(35, 13);
            this.lb_aruco.TabIndex = 3;
            this.lb_aruco.Text = "label2";
            this.lb_aruco.Visible = false;
            // 
            // ViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(936, 450);
            this.Controls.Add(this.ViewBox);
            this.Controls.Add(ToolPanel);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ViewForm";
            this.Text = "ViewForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ViewForm_FormClosed);
            ToolPanel.ResumeLayout(false);
            ToolPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ViewBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox CameraListCB;
        private Emgu.CV.UI.ImageBox ViewBox;
        private System.Windows.Forms.Label TiltAngleLbl;
        private System.Windows.Forms.Label lb_aruco;
        private System.Windows.Forms.Label lb_qr;
    }
}

