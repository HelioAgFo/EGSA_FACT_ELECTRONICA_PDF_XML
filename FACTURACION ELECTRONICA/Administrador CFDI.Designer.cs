namespace FACT_ELECTRONICA_PDF_XML
{
    partial class form_Administrador_CFDI
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
            this.txtboxShipperId = new System.Windows.Forms.TextBox();
            this.lblShipperId = new System.Windows.Forms.Label();
            this.lblStatusCFDI = new System.Windows.Forms.Label();
            this.btnGenerarPDF = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtboxShipperId
            // 
            this.txtboxShipperId.Location = new System.Drawing.Point(86, 26);
            this.txtboxShipperId.Name = "txtboxShipperId";
            this.txtboxShipperId.Size = new System.Drawing.Size(180, 20);
            this.txtboxShipperId.TabIndex = 0;
            this.txtboxShipperId.Leave += new System.EventHandler(this.txtboxShipperId_Leave);
            // 
            // lblShipperId
            // 
            this.lblShipperId.AutoSize = true;
            this.lblShipperId.Location = new System.Drawing.Point(13, 29);
            this.lblShipperId.Name = "lblShipperId";
            this.lblShipperId.Size = new System.Drawing.Size(67, 13);
            this.lblShipperId.TabIndex = 1;
            this.lblShipperId.Text = "Embarque Id";
            // 
            // lblStatusCFDI
            // 
            this.lblStatusCFDI.AutoSize = true;
            this.lblStatusCFDI.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusCFDI.Location = new System.Drawing.Point(293, 26);
            this.lblStatusCFDI.Name = "lblStatusCFDI";
            this.lblStatusCFDI.Size = new System.Drawing.Size(0, 20);
            this.lblStatusCFDI.TabIndex = 2;
            // 
            // btnGenerarPDF
            // 
            this.btnGenerarPDF.Location = new System.Drawing.Point(47, 84);
            this.btnGenerarPDF.Name = "btnGenerarPDF";
            this.btnGenerarPDF.Size = new System.Drawing.Size(131, 32);
            this.btnGenerarPDF.TabIndex = 3;
            this.btnGenerarPDF.Text = "Generar PDF";
            this.btnGenerarPDF.UseVisualStyleBackColor = true;
            this.btnGenerarPDF.Click += new System.EventHandler(this.btnGenerarPDF_Click);
            // 
            // form_Administrador_CFDI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 146);
            this.Controls.Add(this.btnGenerarPDF);
            this.Controls.Add(this.lblStatusCFDI);
            this.Controls.Add(this.lblShipperId);
            this.Controls.Add(this.txtboxShipperId);
            this.Name = "form_Administrador_CFDI";
            this.Text = "Administrador_CFDI";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtboxShipperId;
        private System.Windows.Forms.Label lblShipperId;
        private System.Windows.Forms.Label lblStatusCFDI;
        private System.Windows.Forms.Button btnGenerarPDF;
    }
}