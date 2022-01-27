using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Media;


namespace Cliente1
{        

    public partial class Iniciar : Form
    {


        Socket server;
        Thread atender;
        string anfitrion;
        string usuario;
        string contraseña;
        string usuarioRe;
        string contraseñaRe;
        int edad;
        int puerto = 9100;
        string ip = "147.83.117.22";
        int timePartida;
        int indicePartida;
        int[,,] cartones = new int[5, 3, 9] {
            {{3,0,22,0,41,52,0,0,89},{7,12,0,38,0,57,0,73,0},{0,18,25,0,44,0,64,76,0}},
            {{0,13,0,33,42,0,64,0,87},{0,0,29,36,0,50,0,75,89},{7,14,0,0,44,58,0,78,0}},
            {{2,10,0,0,48,0,0,72,84},{0,19,0,35,49,0,62,79,0},{5,0,24,0,0,51,66,0,85}},
            {{4,11,0,34,0,52,63,0,0},{9,0,23,39,0,0,69,0,81},{0,12,27,0,43,59,0,77,0}},
            {{3,0,21,0,41,0,0,71,80},{0,0,26,32,0,54,65,0,82},{6,18,0,37,45,0,68,0,0}}
        };
        int cartonJuego;  // número de cartón que se juega en la partida actual
        int extraccionActual; // última extracción recibida
        int bola; // número de bola recibido
        bool enPartida = false;  // flag que nos informa si estamos en partida
        bool hayBingo = false; // flag que pondremos a true cuando haya bingo en la partida
        int numAciertos;  // número de aciertos que llevamos en el cartón. Con 15 aciertos, es bingo
        bool soyAnfitrion = false;

        PictureBox[,] cartonPartida;
        PictureBox[] serpentinBolas;
        Bitmap[] ImgBolas;

        Bitmap ImgAspa = (Bitmap)Image.FromFile("aspa.png"); //cargamos imagen del aspa
        Bitmap ImgBombo = (Bitmap)Image.FromFile("bombo.png"); //cargamos imagen del espacio/hueco
        Bitmap ImgFondo = new Bitmap(100, 100);

        public Iniciar()
        {
            InitializeComponent();
            DesconectarBtn.Enabled = false;
            BajaBtn.Enabled = false;
            EnviarPet.Enabled = false;
            CrearBtn.Enabled = false;
            IniciarBtn.Enabled = false;
            CerrarBtn.Enabled = false;
            InvitarBtn.Enabled = false;
            BingoBtn.Enabled = false;
            PartConsGanadorTbt.Enabled = false;
            JugadorGanadasTbt.Enabled = false;
 /*           cartonPartida = new TextBox[3, 9] {
                {c11,c12,c13,c14,c15,c16,c17,c18,c19},
                {c21,c22,c23,c24,c25,c26,c27,c28,c29},
                {c31,c32,c33,c34,c35,c36,c37,c38,c39}
            };
 */
            cartonPartida = new PictureBox[3, 9]
            {
                {car11,car12,car13,car14,car15,car16,car17,car18,car19},
                {car21,car22,car23,car24,car25,car26,car27,car28,car29},
                {car31,car32,car33,car34,car35,car36,car37,car38,car39}
            };

            serpentinBolas = new PictureBox[90]
            {
                b1,b2,b3,b4,b5,b6,b7,b8,b9,b10,b11,b12,b13,b14,b15,b16,b17,b18,b19,b20,b21,b22,b23,b24,b25,b26,b27,b28,b29,b30,
                b31,b32,b33,b34,b35,b36,b37,b38,b39,b40,b41,b42,b43,b44,b45,b46,b47,b48,b49,b50,b51,b52,b53,b54,b55,b56,b57,b58,b59,b60,
                b61,b62,b63,b64,b65,b66,b67,b68,b69,b70,b71,b72,b73,b74,b75,b76,b77,b78,b79,b80,b81,b82,b83,b84,b85,b86,b87,b88,b89,b90
            };

            Graphics gr = Graphics.FromImage(ImgFondo);
            gr.Clear(Color.Transparent);

            ImgBolas = new Bitmap[90];
            for(int i=1;i<=90;i++)
            {
                serpentinBolas[i-1].SizeMode = PictureBoxSizeMode.StretchImage; // cambiar el modo de imagen del serpentin a ajustar
                serpentinBolas[i - 1].Image = ImgFondo;
                serpentinBolas[i - 1].BackgroundImage = ImgFondo;
                serpentinBolas[i - 1].BackColor = Color.Transparent;
//                serpentinBolas[i - 1].Parent = fsBox;

                ImgBolas[i-1] = (Bitmap)Image.FromFile("bolas\\b"+i.ToString()+".png"); //cargamos las imágenes de las bolas

            }
            BolaBox.SizeMode = PictureBoxSizeMode.StretchImage; // cambiar modo de imagen de visualizador ultima bola a ajustar
        }


        public void AñadeLista(ListBox lista, string texto)
        {
            lista.Items.Add(texto);
        }
        
// función para pintar el cartón en la pantalla
        public void PintarCarton(int ncarton)
        {
            if (ncarton < cartones.Length)
            {
               Font arialFont = new Font("Arial Narrow", 14, FontStyle.Bold);
               this.Invoke(new Action(() =>
                {

                    RectangleF rect = new RectangleF(0,0,35,40);
                    StringFormat formato = new StringFormat();
                    formato.Alignment = StringAlignment.Center;
                    formato.LineAlignment = StringAlignment.Center;
                    formato.FormatFlags = StringFormatFlags.NoClip;

                    for (int i = 0; i < 3; i++)
                        for (int j = 0; j < 9; j++)
                        {
                            cartonPartida[i, j].BackgroundImageLayout = ImageLayout.Stretch;
                            if(cartones[ncarton, i, j] == 0)
                                cartonPartida[i, j].BackgroundImage = ImgBombo;
                            else
                            {
                                Bitmap ImgFondo = new Bitmap(35, 40);
                                Graphics gr = Graphics.FromImage(ImgFondo);
                                gr.Clear(Color.White);
                                gr.DrawString(cartones[ncarton, i, j].ToString(), arialFont, Brushes.Blue, rect, formato);
                                cartonPartida[i, j].BackgroundImage = ImgFondo;
                            }
                        }
                }));
            }

        }

// método para pintar una bola en la pantalla
        public void PintarBola(int bola)
        {
               this.Invoke(new Action(() =>
               {
                   serpentinBolas[extraccionActual].Image = ImgBolas[bola-1];
                   extraccionActual++;
                   BolaBox.Image = ImgBolas[bola-1];
               }));

        }

// método para limpiar el cartón, la lista de bolas, etc 
        public void LimpiarCartonBolas()
        {
            this.Invoke(new Action(() =>
            {

                Bitmap ImgFondo = new Bitmap(100, 100);
                Graphics gr = Graphics.FromImage(ImgFondo);
                gr.Clear(Color.Transparent);

                for (int i = 0; i < 90; i++)
                    serpentinBolas[i].Image = ImgFondo;
                BolaBox.Image = ImgFondo;

                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 9; j++)
                    {
                        cartonPartida[i, j].BackgroundImage = ImgFondo;
                        cartonPartida[i, j].Image = ImgFondo;

                    }
            }));

        }

        // método para comprobar la bola en el cartón y marcarla si procede
        public void ComprobarBola(int bola)
        {
            int i, j;
            bool encontrado = false;


            // buscamos una coincidencia en el cartón que se pasa como
            i = 0;
            do
            {
                j = 0;
                do
                {
                    if (bola == cartones[cartonJuego, i, j])
                        encontrado = true;
                    else
                        j++;
                } while (!encontrado && j < 9);
                if (!encontrado)
                    i++;
            } while (!encontrado && i < 3);

            if (encontrado)
            {
                numAciertos++;
                this.Invoke(new Action(() =>
                {
                    cartonPartida[i, j].SizeMode = PictureBoxSizeMode.StretchImage;
                    cartonPartida[i, j].Image = ImgAspa;
                }));

            }
        }


        private void AtenderServidor()
        {
            try
            {
                while (true)
                {
          
                    int nform;
                    byte[] msg2 = new byte[80];

                    server.Receive(msg2);
                    string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                    int codigo = Convert.ToInt32(trozos[0]);
                    string mensaje;

                    switch (codigo)
                    {
                        case 0: //Respuesta a la desconexión
                            
                            this.Invoke(new Action(() =>
                            {
                                UsuarioTbt.Clear();
                                ContraseñaTbt.Clear();
                                listBox_Conectados.Items.Clear();
                                SignInBtn.Enabled = true;
                                UsuarioTbt.Enabled = true;
                                ContraseñaTbt.Enabled = true;
                                DesconectarBtn.Enabled = false;
                                EnviarPet.Enabled = false;
                                RegistrarseBtn.Enabled = true;
                                BajaBtn.Enabled = false;
                                CrearBtn.Enabled = false;
                            }));
                            break;


                        case 1: //Recibes la respuesta de iniciar sesion
                            try
                            {
                                mensaje = trozos[1].Split('\0')[0];
                                if (mensaje == "1")
                                {
                                    MessageBox.Show("Usuario y contraseña correctos");
                                   
                                    this.Invoke(new Action(() =>
                                    {
                                        SignInBtn.Enabled = false;
                                        UsuarioTbt.Enabled = false;
                                        ContraseñaTbt.Enabled = false;
                                        DesconectarBtn.Enabled = true;
                                        EnviarPet.Enabled = true;
                                        RegistrarseBtn.Enabled = false;
                                        BajaBtn.Enabled = true;
                                        CrearBtn.Enabled = true;
                                    }));
                                }
                                else
                                    MessageBox.Show("Los datos introducidos no son los correctos");
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 1");
                            }

                            break;

                        case 2: //Recibes la respuesta de registrarse
                            try
                            {
                                mensaje = trozos[1].Split('\0')[0];
                                if (mensaje == "1")
                                {
                                    MessageBox.Show("Usuario registrado correctamente");
                                }
                                else if (mensaje == "0")
                                    MessageBox.Show("Estos datos ya estan registrados");
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 2");
                            }
                            break;
                        case 3: //Resultado consulta ganador partida que le preguntes
                            try
                            {
                                this.Invoke(new Action(() =>
                                {
                                    listBox_consultas.Items.Clear();
                                    mensaje = "El ganador de la partida " + PartConsGanadorTbt.Text + " es:";
                                    listBox_consultas.Items.Add(mensaje);
                                    mensaje = trozos[1].Split('\0')[0];
                                    if(mensaje.Length == 0)
                                        listBox_consultas.Items.Add("No se ha encontrado la partida o no hay ganador");
                                    else
                                         listBox_consultas.Items.Add(mensaje);
                                 }));                               
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 3");
                            }
                            break;
                        case 4: //resultado consulta partidas ganadas por Berta
                            try
                            {
                                this.Invoke(new Action(() =>
                                {
                                listBox_consultas.Items.Clear();
                                listBox_consultas.Items.Add("Partidas ganadas por "+JugadorGanadasTbt.Text+":");
                                for (int i = 1; i < trozos.Count(); i++)
                                {
                                    mensaje=trozos[i].Split('\0')[0];
                                        listBox_consultas.Items.Add(trozos[i]);
                                }
                                }));
                            
                            }

                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 4");
                            }
                            break;
                        case 5: //resultado puntos max en una partida
                            try
                            {
                                this.Invoke(new Action(() =>
                                {
                                    listBox_consultas.Items.Clear();
                                mensaje = trozos[1].Split('\0')[0];
                                listBox_consultas.Items.Add("Máximo de puntos en una partida:");
                                listBox_consultas.Items.Add(mensaje);
                                }));
  
                            }
                         
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 5");
                            }
                            break;
                        case 7: //lista conectados 
                            try
                            {
                                this.Invoke(new Action(() =>
                                {
                                    listBox_Conectados.Items.Clear();
                                    int numero_usuarios = Convert.ToInt32(trozos[1]);
                                    for (int i = 0; i < numero_usuarios; i++)
                                    {

                                        listBox_Conectados.Items.Add(trozos[2 + i]);
                                    }
                                }));

                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 7");
                            }
                            break;
                        case 10: //Recibes la respuesta de crear partida
                            try
                            {
                                mensaje = trozos[1].Split('\0')[0];
                                indicePartida = Convert.ToInt32(mensaje);
                                if (indicePartida != -1)
                                {
                                    soyAnfitrion = true;
                                    MessageBox.Show("Partida creada correctamente "+ indicePartida.ToString());
                                    this.Invoke(new Action(() =>
                                    {
                                        IniciarBtn.Enabled = true;
                                        CrearBtn.Enabled = false;
                                        InvitarBtn.Enabled = true;
                                        listBox_Partida.Items.Add(UsuarioTbt.Text);
                                        DesconectarBtn.Enabled = false;
                                        BajaBtn.Enabled = false;

                                    }));

                                }
                                else
                                    MessageBox.Show("No se ha podido crear la partida");
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 10");
                            }

                            break;
                        case 11:  // mensaje de inicio de partida
                            // recuperar parametros del mensaje
                            try
                            {
                                mensaje = trozos[1].Split('\0')[0];
                                indicePartida = Convert.ToInt32(mensaje);
                                mensaje = trozos[2].Split('\0')[0];
                                cartonJuego = Convert.ToInt32(mensaje);
                                if (cartonJuego == -1 || indicePartida == -1)
                                {
                                    MessageBox.Show("No se ha podido recuperar la información de inicio de partida");
                                    break;
                                }
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 11");
                            }

                            // preparar carton
                            extraccionActual = 0;
                            PintarCarton(cartonJuego);
                            // activar flag "en partida"
                            enPartida = true;
                            hayBingo = false;
                            numAciertos = 0;
                            this.Invoke(new Action(() =>
                            {
                                BingoBtn.Enabled = false;
                                if (soyAnfitrion)
                                {
                                    IniciarBtn.Enabled = false;
                                    InvitarBtn.Enabled = false;
                                }
                            }));

                            break;
                        case 12:  // mensaje de final de partida
                                  // recuperar parametros del mensaje
                            try
                            {
                                mensaje = trozos[1].Split('\0')[0];
                                indicePartida = Convert.ToInt32(mensaje);

                                if  (indicePartida == -1)
                                {
                                    MessageBox.Show("No se ha podido recuperar la información de final de partida");
                                    break;
                                }
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 12");
                            }

                            // limpiar cartón y bolas
                            LimpiarCartonBolas();

                            // desactivar flag "en partida" y anfitrion
                            enPartida = false;
                            soyAnfitrion = false;
                            
                            this.Invoke(new Action(() =>
                            {
                                BingoBtn.Enabled = false;
                                CrearBtn.Enabled = true;
                                IniciarBtn.Enabled = false;
                                CerrarBtn.Enabled = false;
                                listBox_Partida.Items.Clear();
                                listBox_Mensajes.Items.Clear();
                                DesconectarBtn.Enabled = true;
                                BajaBtn.Enabled = true;
                            }));
                            break;
                        case 14: //Recibes una invitación a una partida si nuestra respuesta es SI la partida empezará y si es NO la partida no se inciará
                            try
                            {
                                mensaje = trozos[1].Split('\0')[0];
                                int indice = Convert.ToInt32(mensaje);
                                string anfitrion = trozos[2].Split('\0')[0];
                                string resp = "NO";
                                if (MessageBox.Show("Te ha invitado " + anfitrion + " a la partida " + indice.ToString(), "Aceptar?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    resp = "SI";
                                    indicePartida = indice;
                                    this.Invoke(new Action(() =>
                                    {
                                        CrearBtn.Enabled = false;
                                        IniciarBtn.Enabled = false;
                                        CerrarBtn.Enabled = false;
                                        DesconectarBtn.Enabled = false;
                                        BajaBtn.Enabled = false;
                                    }));
                                }

                                mensaje = "14/";
                                mensaje += indice.ToString() + "/" + anfitrion + "/"+ resp;
                                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                                server.Send(msg);

                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 14");
                            }

                            break;
                        case 15: //Recepción mensaje
                            try
                            {
                                this.Invoke(new Action(() =>
                                {
                                    mensaje = trozos[1].Split('\0')[0];
                                    listBox_Mensajes.Items.Add(mensaje);
                                    listBox_Mensajes.TopIndex = listBox_Mensajes.Items.Count - 1;

                                }));

                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 15");
                            }
                            break;


                        case 17: // Notificación de conexión de usuario a la partida
                            try
                            {
                                this.Invoke(new Action(() =>
                                {
                                    mensaje = trozos[1].Split('\0')[0];
                                    listBox_Partida.Items.Add(mensaje);
                                    mensaje = trozos[2].Split('\0')[0];
                                    PuntosTbt.Text = mensaje;
                                }));

                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 17");
                            }
                            break;



                        case 20:  //Recepción de bola 
                            
                            if (!enPartida)  // si no estamos en partida, ponemos error
                            {
                                MessageBox.Show("Error: recepción de bolas sin partida iniciada");
                                break;
                            }
                            // recuperar parametros del mensaje
                            try
                            {
                                mensaje = trozos[1].Split('\0')[0];
                                int numextraccion = Convert.ToInt32(mensaje);
                                mensaje = trozos[2].Split('\0')[0];
                                bola = Convert.ToInt32(mensaje);
                                if (numextraccion == -1 || bola == -1)
                                {
                                    MessageBox.Show("No se ha podido recuperar la información de bola");
                                    break;
                                }
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de mensaje 20");
                            }

                            // pintar bola
                            PintarBola(bola);
                            // Marcar bola si está y comprobar si hay bingo y activar boton bingo
                            ComprobarBola(bola);
                            if(numAciertos == 15)
                                this.Invoke(new Action(() =>
                                {
                                    BingoBtn.Enabled = true;
                                }));

                            break;

                        case 21:  // Mensaje de que han cantado bingo
                            this.Invoke(new Action(() =>
                            {
                                BingoBtn.Enabled = false;
                                if(soyAnfitrion)
                                    CerrarBtn.Enabled = true;
                            }));

                            break;

                    }
                }

            }
            catch (FormatException)
            {
                MessageBox.Show("Error en la recepcion de los datos");
            }
        } //Metodo para atender los mensajes del servidor

        private void Iniciar_Load(object sender, EventArgs e) //conectar con el servidor cuando se abre el form
        {
            IPAddress direc = IPAddress.Parse(ip);
            IPEndPoint ipep = new IPEndPoint(direc, 50004);

            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
            }

            catch (SocketException)
            {
                MessageBox.Show("No se ha podido conectar con el servidor");
            }
            ThreadStart ts = delegate { AtenderServidor(); };
            atender = new Thread(ts);
            atender.Start();
        }

        private void Inicio_FormClosing(object sender, FormClosingEventArgs e) //MDesconectar el usuario del servidor cuando se cierra el form
        {
            try
            {
                string mensaje = "0/";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                this.BackColor = Color.Gray;
                server.Shutdown(SocketShutdown.Both);
                server.Close();
                atender.Abort();
           }
            catch (FormatException)
            {
                MessageBox.Show("No se ha desconectado correctamente");
            }
        }

        private void EnviarPet_Click(object sender, EventArgs e)
        {

            if (p1.Checked)//ganador partida 
            {
                string mensaje;

                if(PartConsGanadorTbt.Text.Length > 0)
                {
                    mensaje = "3/";
                    mensaje += PartConsGanadorTbt.Text;
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                }
            }
            else if (p2.Checked)//partidas ganadas por un jugador
            {
                string mensaje;

                mensaje = "4/";
                mensaje += JugadorGanadasTbt.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

            }
            else if (p3.Checked)//máximo número de puntos ganados en una partida
            {
                string mensaje;

                mensaje = "5/";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }

        }

        private void SignInBtn_Click(object sender, EventArgs e)
        {
            string mensaje;

            mensaje = "1/";
            mensaje += UsuarioTbt.Text + "/" + ContraseñaTbt.Text;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void RegistrarseBtn_Click(object sender, EventArgs e)
        {
            string mensaje;

            mensaje = "2/";
            mensaje += UsuarioTbt.Text + "/" + ContraseñaTbt.Text;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

        }

        private void DesconectarBtn_Click(object sender, EventArgs e)
        {
            string mensaje;
            mensaje = "0/";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void CrearBtn_Click(object sender, EventArgs e)
        {
            string mensaje;
            mensaje = "10/";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

        }

        private void InvitarBtn_Click(object sender, EventArgs e)
        {
            if (listBox_Conectados.SelectedItems.Count != 0)
            {
                string mensaje;
                mensaje = "13/";
                mensaje += indicePartida.ToString() + "/" + listBox_Conectados.SelectedItem.ToString();
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
        }

        private void IniciarBtn_Click(object sender, EventArgs e)
        {
            string mensaje;
            mensaje = "11/";
            mensaje += indicePartida.ToString();
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void ChatTbt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string mensaje;
                mensaje = "15/";
                mensaje += indicePartida.ToString() + "/" + ChatTbt.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                ChatTbt.Clear();
            }
        }

        private void CerrarBtn_Click(object sender, EventArgs e)  // Boton de cerrar partida. 
        {
            string mensaje;
            mensaje = "12/";
            mensaje += indicePartida.ToString();
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void c35_TextChanged(object sender, EventArgs e)
        {

        }

        private void BingoBtn_Click(object sender, EventArgs e)  // Botón para cantar bingo
        {
            string mensaje;
            mensaje = "21/";
            mensaje += indicePartida.ToString();
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

        }

        private void BajaBtn_Click(object sender, EventArgs e)
        {
            string mensaje;
            mensaje = "6/";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void p1_CheckedChanged(object sender, EventArgs e)
        {
            PartConsGanadorTbt.Enabled = true;
            JugadorGanadasTbt.Enabled = false;

        }

        private void p2_CheckedChanged(object sender, EventArgs e)
        {
            PartConsGanadorTbt.Enabled = false;
            JugadorGanadasTbt.Enabled = true;
        }

        private void p3_CheckedChanged(object sender, EventArgs e)
        {
            PartConsGanadorTbt.Enabled = false;
            JugadorGanadasTbt.Enabled = false;
        }

        private void car11_Click(object sender, EventArgs e)
        {

        }
    }


}
