
namespace Cliente1
{
    partial class Iniciar
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Iniciar));
            this.label1 = new System.Windows.Forms.Label();
            this.UsuarioTbt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ContraseñaTbt = new System.Windows.Forms.TextBox();
            this.SignInBtn = new System.Windows.Forms.Button();
            this.RegistrarseBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.p3 = new System.Windows.Forms.RadioButton();
            this.p2 = new System.Windows.Forms.RadioButton();
            this.p1 = new System.Windows.Forms.RadioButton();
            this.EnviarPet = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.DesconectarBtn = new System.Windows.Forms.Button();
            this.listBox_consultas = new System.Windows.Forms.ListBox();
            this.listBox_Conectados = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.InvitarBtn = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Mensajes = new System.Windows.Forms.Label();
            this.listBox_Mensajes = new System.Windows.Forms.ListBox();
            this.IniciarBtn = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.listBox_Partida = new System.Windows.Forms.ListBox();
            this.CrearBtn = new System.Windows.Forms.Button();
            this.ChatTbt = new System.Windows.Forms.TextBox();
            this.CerrarBtn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(3, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Usuario:";
            // 
            // UsuarioTbt
            // 
            this.UsuarioTbt.Location = new System.Drawing.Point(6, 34);
            this.UsuarioTbt.Name = "UsuarioTbt";
            this.UsuarioTbt.Size = new System.Drawing.Size(106, 20);
            this.UsuarioTbt.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(3, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Contraseña:";
            // 
            // ContraseñaTbt
            // 
            this.ContraseñaTbt.Location = new System.Drawing.Point(6, 75);
            this.ContraseñaTbt.Name = "ContraseñaTbt";
            this.ContraseñaTbt.PasswordChar = 'o';
            this.ContraseñaTbt.Size = new System.Drawing.Size(106, 20);
            this.ContraseñaTbt.TabIndex = 6;
            // 
            // SignInBtn
            // 
            this.SignInBtn.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SignInBtn.Location = new System.Drawing.Point(2, 101);
            this.SignInBtn.Name = "SignInBtn";
            this.SignInBtn.Size = new System.Drawing.Size(55, 24);
            this.SignInBtn.TabIndex = 7;
            this.SignInBtn.Text = "Sign In";
            this.SignInBtn.UseVisualStyleBackColor = true;
            this.SignInBtn.Click += new System.EventHandler(this.SignInBtn_Click);
            // 
            // RegistrarseBtn
            // 
            this.RegistrarseBtn.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RegistrarseBtn.Location = new System.Drawing.Point(25, 130);
            this.RegistrarseBtn.Name = "RegistrarseBtn";
            this.RegistrarseBtn.Size = new System.Drawing.Size(75, 26);
            this.RegistrarseBtn.TabIndex = 8;
            this.RegistrarseBtn.Text = "Registrarse";
            this.RegistrarseBtn.UseVisualStyleBackColor = true;
            this.RegistrarseBtn.Click += new System.EventHandler(this.RegistrarseBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.groupBox1.Controls.Add(this.p3);
            this.groupBox1.Controls.Add(this.p2);
            this.groupBox1.Controls.Add(this.p1);
            this.groupBox1.Controls.Add(this.EnviarPet);
            this.groupBox1.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(725, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(152, 134);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Consultas";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // p3
            // 
            this.p3.AutoSize = true;
            this.p3.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p3.Location = new System.Drawing.Point(6, 68);
            this.p3.Name = "p3";
            this.p3.Size = new System.Drawing.Size(152, 32);
            this.p3.TabIndex = 9;
            this.p3.TabStop = true;
            this.p3.Text = "Máximo número de puntos \r\nganados en una partida";
            this.p3.UseVisualStyleBackColor = true;
            this.p3.CheckedChanged += new System.EventHandler(this.p3_CheckedChanged);
            // 
            // p2
            // 
            this.p2.AutoSize = true;
            this.p2.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p2.Location = new System.Drawing.Point(6, 44);
            this.p2.Name = "p2";
            this.p2.Size = new System.Drawing.Size(147, 18);
            this.p2.TabIndex = 7;
            this.p2.TabStop = true;
            this.p2.Text = "Partidas ganadas por Berta";
            this.p2.UseVisualStyleBackColor = true;
            // 
            // p1
            // 
            this.p1.AutoSize = true;
            this.p1.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p1.Location = new System.Drawing.Point(6, 20);
            this.p1.Name = "p1";
            this.p1.Size = new System.Drawing.Size(107, 18);
            this.p1.TabIndex = 8;
            this.p1.TabStop = true;
            this.p1.Text = "Ganador partida 2";
            this.p1.UseVisualStyleBackColor = true;
            // 
            // EnviarPet
            // 
            this.EnviarPet.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnviarPet.Location = new System.Drawing.Point(3, 105);
            this.EnviarPet.Name = "EnviarPet";
            this.EnviarPet.Size = new System.Drawing.Size(97, 23);
            this.EnviarPet.TabIndex = 5;
            this.EnviarPet.Text = "Enviar petición";
            this.EnviarPet.UseVisualStyleBackColor = true;
            this.EnviarPet.Click += new System.EventHandler(this.EnviarPet_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.groupBox2.Controls.Add(this.DesconectarBtn);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.UsuarioTbt);
            this.groupBox2.Controls.Add(this.RegistrarseBtn);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.ContraseñaTbt);
            this.groupBox2.Controls.Add(this.SignInBtn);
            this.groupBox2.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(141, 163);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Inicio de sesión";
            // 
            // DesconectarBtn
            // 
            this.DesconectarBtn.Location = new System.Drawing.Point(63, 101);
            this.DesconectarBtn.Name = "DesconectarBtn";
            this.DesconectarBtn.Size = new System.Drawing.Size(75, 23);
            this.DesconectarBtn.TabIndex = 8;
            this.DesconectarBtn.Text = "Desconectar";
            this.DesconectarBtn.UseVisualStyleBackColor = true;
            this.DesconectarBtn.Click += new System.EventHandler(this.DesconectarBtn_Click);
            // 
            // listBox_consultas
            // 
            this.listBox_consultas.FormattingEnabled = true;
            this.listBox_consultas.Location = new System.Drawing.Point(725, 168);
            this.listBox_consultas.Name = "listBox_consultas";
            this.listBox_consultas.Size = new System.Drawing.Size(138, 56);
            this.listBox_consultas.TabIndex = 16;
            // 
            // listBox_Conectados
            // 
            this.listBox_Conectados.FormattingEnabled = true;
            this.listBox_Conectados.Location = new System.Drawing.Point(12, 194);
            this.listBox_Conectados.Name = "listBox_Conectados";
            this.listBox_Conectados.Size = new System.Drawing.Size(138, 160);
            this.listBox_Conectados.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 178);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Usuarios Conectados";
            // 
            // InvitarBtn
            // 
            this.InvitarBtn.Location = new System.Drawing.Point(96, 360);
            this.InvitarBtn.Name = "InvitarBtn";
            this.InvitarBtn.Size = new System.Drawing.Size(75, 23);
            this.InvitarBtn.TabIndex = 19;
            this.InvitarBtn.Text = "Invitar =>";
            this.InvitarBtn.UseVisualStyleBackColor = true;
            this.InvitarBtn.Click += new System.EventHandler(this.InvitarBtn_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.CerrarBtn);
            this.groupBox3.Controls.Add(this.ChatTbt);
            this.groupBox3.Controls.Add(this.Mensajes);
            this.groupBox3.Controls.Add(this.listBox_Mensajes);
            this.groupBox3.Controls.Add(this.IniciarBtn);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.listBox_Partida);
            this.groupBox3.Controls.Add(this.CrearBtn);
            this.groupBox3.Location = new System.Drawing.Point(177, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(542, 453);
            this.groupBox3.TabIndex = 20;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Partida";
            // 
            // Mensajes
            // 
            this.Mensajes.AutoSize = true;
            this.Mensajes.Location = new System.Drawing.Point(273, 292);
            this.Mensajes.Name = "Mensajes";
            this.Mensajes.Size = new System.Drawing.Size(52, 13);
            this.Mensajes.TabIndex = 5;
            this.Mensajes.Text = "Mensajes";
            // 
            // listBox_Mensajes
            // 
            this.listBox_Mensajes.FormattingEnabled = true;
            this.listBox_Mensajes.Location = new System.Drawing.Point(276, 311);
            this.listBox_Mensajes.Name = "listBox_Mensajes";
            this.listBox_Mensajes.Size = new System.Drawing.Size(247, 82);
            this.listBox_Mensajes.TabIndex = 4;
            // 
            // IniciarBtn
            // 
            this.IniciarBtn.Location = new System.Drawing.Point(133, 341);
            this.IniciarBtn.Name = "IniciarBtn";
            this.IniciarBtn.Size = new System.Drawing.Size(122, 23);
            this.IniciarBtn.TabIndex = 3;
            this.IniciarBtn.Text = "Iniciar Partida";
            this.IniciarBtn.UseVisualStyleBackColor = true;
            this.IniciarBtn.Click += new System.EventHandler(this.IniciarBtn_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 292);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Usuarios en partida";
            // 
            // listBox_Partida
            // 
            this.listBox_Partida.FormattingEnabled = true;
            this.listBox_Partida.Location = new System.Drawing.Point(6, 311);
            this.listBox_Partida.Name = "listBox_Partida";
            this.listBox_Partida.Size = new System.Drawing.Size(120, 134);
            this.listBox_Partida.TabIndex = 1;
            // 
            // CrearBtn
            // 
            this.CrearBtn.Location = new System.Drawing.Point(132, 311);
            this.CrearBtn.Name = "CrearBtn";
            this.CrearBtn.Size = new System.Drawing.Size(123, 23);
            this.CrearBtn.TabIndex = 0;
            this.CrearBtn.Text = "Crear Partida";
            this.CrearBtn.UseVisualStyleBackColor = true;
            this.CrearBtn.Click += new System.EventHandler(this.CrearBtn_Click);
            // 
            // ChatTbt
            // 
            this.ChatTbt.Location = new System.Drawing.Point(276, 416);
            this.ChatTbt.Name = "ChatTbt";
            this.ChatTbt.Size = new System.Drawing.Size(247, 20);
            this.ChatTbt.TabIndex = 6;
            this.ChatTbt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ChatTbt_KeyDown);
            // 
            // CerrarBtn
            // 
            this.CerrarBtn.Location = new System.Drawing.Point(133, 370);
            this.CerrarBtn.Name = "CerrarBtn";
            this.CerrarBtn.Size = new System.Drawing.Size(122, 23);
            this.CerrarBtn.TabIndex = 7;
            this.CerrarBtn.Text = "Cerrar Partida";
            this.CerrarBtn.UseVisualStyleBackColor = true;
            this.CerrarBtn.Click += new System.EventHandler(this.CerrarBtn_Click);
            // 
            // Iniciar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(903, 494);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.InvitarBtn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.listBox_Conectados);
            this.Controls.Add(this.listBox_consultas);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Iniciar";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Iniciar_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox UsuarioTbt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ContraseñaTbt;
        private System.Windows.Forms.Button SignInBtn;
        private System.Windows.Forms.Button RegistrarseBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton p3;
        private System.Windows.Forms.RadioButton p2;
        private System.Windows.Forms.RadioButton p1;
        private System.Windows.Forms.Button EnviarPet;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listBox_consultas;
        private System.Windows.Forms.Button DesconectarBtn;
        private System.Windows.Forms.ListBox listBox_Conectados;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button InvitarBtn;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button IniciarBtn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox listBox_Partida;
        private System.Windows.Forms.Button CrearBtn;
        private System.Windows.Forms.ListBox listBox_Mensajes;
        private System.Windows.Forms.Label Mensajes;
        private System.Windows.Forms.TextBox ChatTbt;
        private System.Windows.Forms.Button CerrarBtn;
    }
}

