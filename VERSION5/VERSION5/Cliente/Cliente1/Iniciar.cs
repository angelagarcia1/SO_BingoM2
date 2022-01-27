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
        string ip = "192.168.56.102";
        int timePartida;
        int indicePartida;

        delegate void DelegadoAddListBox(string texto);
        delegate void DelegadoClearListBox();
        delegate void DelegadoButtState(bool estado);

        public Iniciar()
        {
            InitializeComponent();
            DesconectarBtn.Enabled = false;
            EnviarPet.Enabled = false;
        }
        /*
        public void PonFormConectar(string m) //Metodo para activar/desactivar algunos botones/text box cuando inicias sesion
        {
            try
            {
                this.Text = m;
                SignInBtn.Enabled = false;
                UsuarioTbt.Enabled = false;
                ContraseñaTbt.Enabled = false;
                
            }
            catch (FormatException)
            {
                MessageBox.Show("Error al poner los datos del form");
            }
        }
        */
        public void AñadeLista(ListBox lista, string texto)
        {
            lista.Items.Add(texto);
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
                        case 3: //resultado consulta ganador partida 2
                            try
                            {
                                this.Invoke(new Action(() =>
                                {
                                    listBox_consultas.Items.Clear();
                                    mensaje = trozos[1].Split('\0')[0];
                                    listBox_consultas.Items.Add("el ganador de la partida 2 es:");
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
                                listBox_consultas.Items.Add("Partidas ganadas por Berta:");
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
                        case 10: //Recibes la respuesta de iniciar partida
                            try
                            {
                                mensaje = trozos[1].Split('\0')[0];
                                int indice = Convert.ToInt32(mensaje);
                                if (indice != -1)
                                {
                                    MessageBox.Show("Partida creada correctamente "+ indice.ToString());
                                    indicePartida = indice;
                                }
                                else
                                    MessageBox.Show("No se ha podido crear la partida");
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 10");
                            }

                            break;
                        case 14: //Recibes una invitación a una partida
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
                                }));

                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("Error al procesar los datos de la respuesta 3");
                            }
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
            IPEndPoint ipep = new IPEndPoint(direc, puerto);

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
                atender.Abort();
                this.BackColor = Color.Gray;
                server.Shutdown(SocketShutdown.Both);
                server.Close();
            }
            catch (FormatException)
            {
                MessageBox.Show("No se ha desconectado correctamente");
            }
        }

        private void EnviarPet_Click(object sender, EventArgs e)
        {

            if (p1.Checked)//ganador partida 2
            {
                string mensaje;

                mensaje = "3/";
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            else if (p2.Checked)//partidas ganadas por Berta
            {
                string mensaje;

                mensaje = "4/";
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

        private void p3_CheckedChanged(object sender, EventArgs e)
        {

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
            }
        }

        private void CerrarBtn_Click(object sender, EventArgs e)
        {

        }
    }


}
