#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>
#include <pthread.h>
#include <ctype.h>
#include <time.h>

#define MAX_USUARIOS   300
#define MAX_PARTIDAS   10
#define NUM_CARTONES    5


typedef struct{
	int ID;
	char usuario[20];
	int socket;
} Conectado;

typedef struct{
	Conectado sockets[MAX_USUARIOS];
	int num;
} ListaConectados;

typedef struct{
	int ID; 
	char fecha[30];
	int duracion;
	char ganador[20];
	int puntos;
	ListaConectados UsuariosPartida;
	ListaConectados UsuariosInvitados;
	int HayBingo;
	time_t tInicio;
	int Extraccion[90]; //bolas extraídas en la partida
}Partida;

typedef struct{
	int num;
	Partida partidas[MAX_PARTIDAS];
}ListaPartidas;

ListaConectados UsuariosConectados;
ListaPartidas listadePartidas;


pthread_mutex_t mutexPartidas = PTHREAD_MUTEX_INITIALIZER;

pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;

//Sorteo de bolas y secuenciamiento de partida 


void SorteoBolas(int IdPart)
{
	int Bombo[90]; //bombo de donde sacaremos las bolas para la partida
	int i, j, sorteo, numextraccion;
	int BolasBombo;
	for ( i = 0 ; i < 90; i++)
		Bombo[i] = i+1;
	BolasBombo = 90;
	numextraccion = 0;
	for ( i = 0 ; i < 90; i++)
	{
		sorteo = rand()% BolasBombo; //nos da un número entre 0 y las bolas que quedan en el Bombo -1
		listadePartidas.partidas[IdPart].Extraccion[numextraccion] = Bombo[sorteo];
		for ( j = sorteo+1 ; j < BolasBombo; j++)
			Bombo[j-1] = Bombo[j]; //desplazamos todas las bolas que quedan en el Bombo
		BolasBombo--; //hay una bola menos en el bombo
		numextraccion++; //hay una nueva extracción
	}
}

void *RealizarPartida (void *IdPart)
{
	int i,tick,numextraccion;
	int IndicePartida;
	char respuesta[512];
	int *s;

	s= (int *) IdPart;
	IndicePartida = *s;
	listadePartidas.partidas[IndicePartida].HayBingo = 0;
	SorteoBolas(IndicePartida);
	for ( i = 0 ; i < 90; i++)
		printf("%d, ", listadePartidas.partidas[IndicePartida].Extraccion[i]);
	printf("\n");
	tick = 0;
	numextraccion = 0;
	while ((!listadePartidas.partidas[IndicePartida].HayBingo) && (numextraccion < 90))  // mientras no haya bingo y haya bolas
	{
		tick++;
		if (tick == 100)
		{
			tick = 0;
			sprintf(respuesta, "20/%d/%d", numextraccion, listadePartidas.partidas[IndicePartida].Extraccion[numextraccion]); 
			printf("%s\n", respuesta);
			for (i = 0; i < listadePartidas.partidas[IndicePartida].UsuariosPartida.num; i++)  // Aquí mandamos la bola que toca a todos los conectados en a partida
			{
				write(listadePartidas.partidas[IndicePartida].UsuariosPartida.sockets[i].socket, respuesta, strlen(respuesta));
			}
			numextraccion++;  // incrementamos para mandar la siguiente bola
		}
		usleep(20000);  // hacemos una pausa de 20 ms
	}


	
}

//Funciones para atender al cliente 

int Add (ListaConectados *lista, int socket, char nombre[20], int id) //Añadir un usuario a la lista de conectados devuelve -1 si la lista esta completa
{
	
	if (lista->num == MAX_USUARIOS)
		return -1;
	else {
		lista->sockets[lista->num].socket=socket;
		strcpy(lista->sockets[lista->num].usuario,nombre);
		lista->sockets[lista->num].ID = id;
		lista->num++;
		return 0;
		
	}
}

void DameConectados (ListaConectados *lista, char conectados[512]) //Devuelve los usuarios conectados
{
	sprintf(conectados,"7/%d/%s",lista->num, lista->sockets[0].usuario);
	int i;
	for(int i=1; i< lista->num; i++)
		sprintf(conectados,"%s/%s", conectados, lista->sockets[i].usuario);
	
	printf("Conectados: %s\n",conectados);
}

int DesconectarUsuario(ListaConectados *lista, char nombre[20]) //Elimina un usuario de la lista si no lo encuentra nos devuelve -1
{
	int j=0;
	int encontrado=0;
	while((j<lista->num) && !encontrado){
		if (strcmp(lista->sockets[j].usuario,nombre)==0)
			encontrado=1;
		if(!encontrado)
			j=j+1;
	}
	if (encontrado)
	{
		for(int pos=j; pos<lista->num-1; pos++)
		{
			strcpy(lista->sockets[pos].usuario,lista->sockets[pos+1].usuario);
			lista->sockets[pos].socket=lista->sockets[pos+1].socket;
		}
		lista->num--;
		return 0;
	}
	
	else
		return -1;
	
}

int DameSocket(ListaConectados *l, char nombre[20])//Devuelve el socket del usuario con ese nombre
{
	int found = 0;
	int i = 0;
	while((i<l->num) && (strcmp(l->sockets[i].usuario, nombre)!=0))
		i++;
	if(i<l->num)
		found = l->sockets[i].socket;
	return found;
}		

int DameNombre(ListaConectados *l, int socket, char nombre[20]) //Devuelve el nombre del usuario con ese socket	en caso de no encontrarlo recibimos -1
{
	int found = 0;
	int i = 0;
	while ((found==0) && (i < l->num))
	{
		if (l->sockets[i].socket==socket)
		{
			found = 1;
		}
		else
			i = i + 1;
	}
	if (found==1)
	{
		strcpy(nombre,l->sockets[i].usuario);
		return 1;
	}
	else
		return -1;
}

int DameID(ListaConectados *l, int socket, int *id) //Devuelve el id del usuario con ese socket	si no lo encuentra devuelve -1
{
	int found = 0;
	int i = 0;
	while ((found == 0) && (i < l->num))
	{
		if (l->sockets[i].socket == socket)
		{
			found = 1;
		}
		else
			i = i + 1;
	}
	if (found == 1)
	{
		*id = l->sockets[i].ID;
		return 1;
	}
	else
		return -1;
}



void *AtenderalCliente (void *socket) //Funcion para atender al cliente
{
	int sock_conn;
	int *s;
	s= (int *) socket;
	sock_conn= *s;
	
	MYSQL *conn;
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [500];
	conn = mysql_init(NULL);
	
	char peticion[512];
	char respuesta[512];
	int ret;
	
	if (conn==NULL) 
	{
		printf ("Error al crear la conexion: %u %s\n",mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	//Conectamos con la base de datos
	conn = mysql_real_connect (conn, "shiva2.upc.es","root", "mysql", "M2_BBDDJuego",0, NULL, 0);
	if (conn==NULL) 
	{
		printf ("Error al inicializar la conexion: %u %s\n",mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	//Bucle para atender al cliente
	char notificacion[500];
	int terminar=0;
	while (terminar==0)
	{
		ret=read(sock_conn,peticion, sizeof(peticion));
		if (ret == 0)
		{
			terminar = 1;
			continue;
		}
		printf ("Recibido\n");
		
		peticion[ret]='\0';
		
		printf ("Mensaje recibido: %s\n",peticion);
		
		char *p = strtok( peticion, "/"); //Sacamos el codigo de la peticion del cliente
		int codigo =  atoi (p);
		printf("codigo= %d\n",codigo);
		
		if (codigo==0) //Desconectar Usuario
		{ 
			char usuario[20];
			DameNombre(&UsuariosConectados,sock_conn,usuario);			
			
			pthread_mutex_lock( &mutex );
			int eliminar = DesconectarUsuario(&UsuariosConectados, usuario);
			pthread_mutex_unlock( &mutex );
			char conectados[300];
			DameConectados(&UsuariosConectados, conectados); 
			sprintf(notificacion,"%s", conectados);
			for(int k=0; k<UsuariosConectados.num; k++)
				write (UsuariosConectados.sockets[k].socket, notificacion, strlen(notificacion));
			if (eliminar==0)
			{
				printf("El usuario se ha eliminado de la lista de conectados\n");
				strcpy(respuesta,"0/");		
				write(sock_conn, respuesta, strlen(respuesta));
			}
			else
				printf("El usuario no se ha podido eliminar de la lista de conectados\n");
		}
		
		else if(codigo==1) //Iniciar sesion
		{
			char usuario[20];
			char password[20];
			p=strtok(NULL, "/");
			strcpy(usuario, p);
			p=strtok(NULL, "/");
			strcpy(password, p);
			
			err=mysql_query (conn, "SELECT * FROM Jugador");
			if (err!=0) 
			{
				printf ("Error al consultar datos de la base %u %s\n",mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			resultado = mysql_store_result (conn);
			row = mysql_fetch_row (resultado);
			if (row == NULL)
				printf ("No se han obtenido datos en la consulta\n");
			else
			{
				while (row != NULL) 
				{
					if (strcmp (row[1],usuario) ==0)
					{
						if(strcmp(row[2],password)==0)
						{
							strcpy(respuesta,"1/1");
							int id = atoi(row[0]);
							pthread_mutex_lock( &mutex );
							int res= Add(&UsuariosConectados,sock_conn,usuario,id);
							pthread_mutex_unlock(&mutex);
							sprintf(notificacion,"7/%d/%s",UsuariosConectados.num,UsuariosConectados.sockets[0].usuario);
							for(int j=1; j< UsuariosConectados.num; j++)
								sprintf(notificacion,"%s/%s", notificacion, UsuariosConectados.sockets[j].usuario);
							for(int k=0; k<UsuariosConectados.num; k++)
								write (UsuariosConectados.sockets[k].socket, notificacion, strlen(notificacion));
							if (res==0)
							{
								printf("Notificacion: %s\n",notificacion);
								printf("El usuario %s se ha aÃ±adido a la lista de conectados \n", usuario );
							}
							else
								printf("La lista de usuarios esta llena");
							
						}
						else 
						{
							strcpy(respuesta,"1/0");
						}
						row = NULL;
					}
					else 
					{
						row = mysql_fetch_row (resultado);
						strcpy(respuesta,"1/0");
					}	
				}
			}
			write(sock_conn, respuesta, strlen(respuesta));

		}
		else if (codigo==2) //Registrar
		{
			char usuario[20];
			char password[20];
			int ultimo_ID;
			p=strtok(NULL, "/");
			strcpy(usuario, p);
			p=strtok(NULL, "/");
			strcpy(password, p);
			
			
			err=mysql_query (conn, "SELECT * FROM Jugador");
			if (err!=0) 
			{
				printf ("Error al consultar datos de la base %u %s\n",mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			resultado = mysql_store_result (conn);
			row = mysql_fetch_row (resultado);
			if (row == NULL)
				printf ("No se han obtenido datos en la consulta\n");
			else
			{
				int encontrado=0;
				while ((row !=NULL)&&(!encontrado)) 
				{
					char name[strlen(row[1])];
					strcpy(name,row[1]);
					strcpy(respuesta, "2/1");
					ultimo_ID = atoi(row[0]);
					
					for (int k=0;k<strlen(name);k++)
					{
						name[k] = toupper(name[k]);
					}
					char user_mayu[20];
					for (int k=0;k<strlen(usuario);k++)
					{
						user_mayu[k] = toupper(usuario[k]);
					}
					user_mayu[strlen(usuario)] = '\0';
					if (strcmp (name,user_mayu) ==0)
					{
						strcpy(respuesta,"2/0");
						encontrado=1;
					}
					else row = mysql_fetch_row (resultado);
				}
				if (strcmp(respuesta,"2/1")==0)
				{
					char id[10];
					strcpy(consulta, "INSERT INTO Jugador VALUES('");
					sprintf(id,"%d", ultimo_ID +1);
					strcat(consulta, id);
					strcat (consulta, "','");
					strcat(consulta, usuario);
					strcat (consulta, "','");
					strcat(consulta, password);
					strcat(consulta, "');");
				err=mysql_query(conn, consulta);
				if (err!=0)
					{
					printf ("Error al introducir datos de la base %u %s \n", mysql_errno(conn), mysql_error(conn));
					exit(1);
					}
				}
					
				
				write(sock_conn, respuesta, strlen(respuesta));
			}
			
			
		}
		
		else if (codigo==3) //Consulta1: Dame el ganador de la partida que se envía en el primer parámetro
		{	
			int numPart;
			p = strtok(NULL, "/");
			numPart = atoi(p);

			char consulta[80];
			sprintf(consulta,"SELECT Partida.ganador FROM (Partida) WHERE Partida.id = %d",numPart);
			err=mysql_query (conn, consulta);
			if (err!=0)
			{
				printf("Error al realizar la consulta %u %s \n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			resultado=mysql_store_result(conn);
			row = mysql_fetch_row(resultado);
			if (row == NULL)
				strcpy(respuesta,"3/");
			else
			{
				strcpy(respuesta,"3");
				while (row!=NULL){
					printf ("%s\n", row[0]);
					strcat (respuesta,"/");
					strcat (respuesta, row[0]);
					row = mysql_fetch_row (resultado);
				}
			}
			write(sock_conn, respuesta, strlen(respuesta));
		
		}
		
		else if (codigo==4) //Consulta2: Dame las partidas ganadas por un jugador
		{	
			char consulta[80];

			p = strtok(NULL, "/");
			sprintf(consulta,"SELECT Partida.id FROM Partida WHERE Partida.Ganador = '%s' ",p);
			err=mysql_query (conn, consulta);
			if (err!=0)
			{
				printf("Error al realizar la consulta %u %s \n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			resultado=mysql_store_result(conn);
			row = mysql_fetch_row(resultado);
			if (row == NULL)
				strcpy(respuesta,"4/");
			else
			{
				strcpy(respuesta,"4");
				while (row!=NULL){
					printf ("%s\n", row[0]);
					strcat (respuesta,"/");
					strcat (respuesta, row[0]);
					row = mysql_fetch_row (resultado);
				}
			}
			write(sock_conn, respuesta, strlen(respuesta));
			
		}
		else if (codigo==5)//Consulta puntos máx. en una partida
		{
			char consulta[80];
			strcpy(consulta,"SELECT max(Resultado.puntos) FROM Resultado");
			err=mysql_query (conn, consulta);
			if (err!=0)
			{
				printf("Error al realizar la consulta %u %s \n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			resultado=mysql_store_result(conn);
			row = mysql_fetch_row(resultado);
			if (row == NULL)
				strcpy(respuesta,"5/Base de datos vacia");
			else
			{
				strcpy(respuesta,"5");
				while (row!=NULL){
					printf ("%s\n", row[0]);
					strcat (respuesta,"/");
					strcat (respuesta, row[0]);
					row = mysql_fetch_row (resultado);
				}
			}
			write(sock_conn, respuesta, strlen(respuesta));
		}
		else if (codigo == 6)// Dar de baja usuario
		{
			int indice = 0;
			char usuario[20];
			DameNombre(&UsuariosConectados, sock_conn, usuario);

			char consulta[80];
			sprintf(consulta, "SELECT id FROM Jugador WHERE nombre='%s'",usuario);
			err = mysql_query(conn, consulta);
			if (err != 0)
			{
				printf("Error al buscar id para baja %u %s \n", mysql_errno(conn), mysql_error(conn));
				exit(1);
			}
			resultado = mysql_store_result(conn);
			row = mysql_fetch_row(resultado);
			if (row == NULL)
			{
				printf("Error: no se encuentra al usuario %s para baja \n",usuario);
			}
			else
			{
				indice = atoi(row[0]);

				/* Borramos primero todas las entradas de ese jugador en la tabla de resultados */
				sprintf(consulta, "DELETE FROM Resultado WHERE Jugador=%d", indice);
				err = mysql_query(conn, consulta);
				if (err != 0)
				{
					printf("Error al borrar usuario  de la tabla de resultados %u %s \n", mysql_errno(conn), mysql_error(conn));
					exit(1);
				}

				/* Ahora borramos el jugador de la tabla de jugadores */
				sprintf(consulta, "DELETE FROM Jugador WHERE id=%d", indice);
				err = mysql_query(conn, consulta);
				if (err != 0)
				{
					printf("Error al borrar usuario %u %s \n", mysql_errno(conn), mysql_error(conn));
					exit(1);
				}

				pthread_mutex_lock(&mutex);
				int eliminar = DesconectarUsuario(&UsuariosConectados, usuario);
				pthread_mutex_unlock(&mutex);
				char conectados[300];
				DameConectados(&UsuariosConectados, conectados);
				sprintf(notificacion, "%s", conectados);
				for (int k = 0; k < UsuariosConectados.num; k++)
					write(UsuariosConectados.sockets[k].socket, notificacion, strlen(notificacion));
				if (eliminar == 0)
				{
					printf("El usuario se ha dado de baja y se ha eliminado de la lista de conectados\n");
					strcpy(respuesta, "0/");
					write(sock_conn, respuesta, strlen(respuesta));
				}
				else
					printf("El usuario no se ha podido eliminar de la lista de conectados\n");

			}

		}
		else if (codigo==10)//Crear partida
		{
			int indice=0;
			char usuario[20];
			DameNombre(&UsuariosConectados,sock_conn,usuario);
			pthread_mutex_lock( &mutex );
			if (listadePartidas.num==MAX_PARTIDAS)
			{
				indice = -1;
				printf("Máximo número de partidas alcanzado\n");
			}
			else
			{	

				//Buscamos un hueco para la partida
				while(listadePartidas.partidas[indice].ID!=0)
					indice++;
				//Creamos la partida
				int res= Add(&listadePartidas.partidas[indice].UsuariosPartida,sock_conn,usuario,0);
				if (res != 0)
					printf("Error al añadir usuario\n");
			
				listadePartidas.num++;

				char consulta[80];
				strcpy(consulta, "SELECT max(Partida.id) FROM Partida");
				err = mysql_query(conn, consulta);
				if (err != 0)
				{
					printf("Error al buscar id %u %s \n", mysql_errno(conn), mysql_error(conn));
					exit(1);
				}

				resultado = mysql_store_result(conn);
				row = mysql_fetch_row(resultado);
				if (row == NULL)
					listadePartidas.partidas[indice].ID = 1;
				else
					listadePartidas.partidas[indice].ID = atoi(row[0]) + 1;

				sprintf(consulta, "INSERT INTO Partida VALUES (%d,' ', 0, ' ')", listadePartidas.partidas[indice].ID);
				err = mysql_query(conn, consulta);
				if (err != 0)
				{
					printf("Error al insertar partida %u %s \n", mysql_errno(conn), mysql_error(conn));
					exit(1);
				}

			}
			
			pthread_mutex_unlock( &mutex );
			sprintf(respuesta,"10/%d", indice);
			write(sock_conn, respuesta, strlen(respuesta));
			
		}
		else if (codigo==11)//Iniciar partida
		{
			int PermisoPartida = 0; 
			int IndicePartida;
			p = strtok(NULL, "/");
			IndicePartida = atoi(p);
			
			if(listadePartidas.partidas[IndicePartida].UsuariosInvitados.num == 0)
			{
				strcpy(respuesta, "15/Servidor: podemos iniciar partida.");
				PermisoPartida = 1;
			}
				
			else 
				strcpy(respuesta, "15/Servidor: no se ha podido iniciar la partida. Usuario no confirmado.");
			for(int i = 0; i<listadePartidas.partidas[IndicePartida].UsuariosPartida.num; i++)
			{
				write(listadePartidas.partidas[IndicePartida].UsuariosPartida.sockets[i].socket, respuesta, strlen(respuesta));
			}
			if(PermisoPartida)
			{
				for(int i = 0; i<listadePartidas.partidas[IndicePartida].UsuariosPartida.num; i++)
				{
					sprintf(respuesta, "11/%d/%d", IndicePartida, i%NUM_CARTONES); //la i corresponde al número de cartón con el que se va a jugar
					write(listadePartidas.partidas[IndicePartida].UsuariosPartida.sockets[i].socket, respuesta, strlen(respuesta));
				}

/**************************************************************************************************/
				time_t t = time(NULL);
				struct tm tiempoLocal = *localtime(&t);
				// El lugar en donde se pondrá la fecha y hora formateadas
				char fechaHora[30];
				// El formato.
				char *formato = "%d/%m/%Y %H:%M:%S";
				// Intentar formatear
				int bytesEscritos =
					strftime(fechaHora, sizeof fechaHora, formato, &tiempoLocal);
				if (bytesEscritos != 0) {
					// Si no hay error, los bytesEscritos no son 0
					printf("Fecha y hora: %s\n", fechaHora);
				}
				else {
					printf("Error formateando fecha");
				}
/******************************************************************************/
				listadePartidas.partidas[IndicePartida].tInicio = t;
				strcpy(listadePartidas.partidas[IndicePartida].fecha, fechaHora);
				listadePartidas.partidas[IndicePartida].puntos = 10 * listadePartidas.partidas[IndicePartida].UsuariosPartida.num;

				// arrancar el hilo de partida
				pthread_t thread;
				int r = pthread_create(&thread, NULL, RealizarPartida, &IndicePartida);
				if (r !=0)
					printf("Error al crear el thread de partida %d\n", r);
				else 
					printf("Creado correctamente el thread %d\n", r);
			}

		}
		else if (codigo==12)//Finalizar partida
		{
			int idJugador;  // identificadir del jugador en base de datos
			int puntos =0;
			char consulta[80];			
			int IndicePartida;
			p = strtok(NULL, "/");
			IndicePartida = atoi(p);


			for (int i = 0; i < listadePartidas.partidas[IndicePartida].UsuariosPartida.num; i++)
			{
				sprintf(respuesta, "12/%d", IndicePartida); 
				write(listadePartidas.partidas[IndicePartida].UsuariosPartida.sockets[i].socket, respuesta, strlen(respuesta));
				DameID(&UsuariosConectados, listadePartidas.partidas[IndicePartida].UsuariosPartida.sockets[i].socket, &idJugador);
				if (strcmp(listadePartidas.partidas[IndicePartida].UsuariosPartida.sockets[i].usuario, listadePartidas.partidas[IndicePartida].ganador) == 0)
					puntos = listadePartidas.partidas[IndicePartida].puntos;
				else
					puntos = 0;
				sprintf(consulta, "INSERT INTO Resultado VALUES (%d,%d,%d)", idJugador, listadePartidas.partidas[IndicePartida].ID,puntos);
				err = mysql_query(conn, consulta);
				if (err != 0)
				{
					printf("Error al insertar partida %u %s \n", mysql_errno(conn), mysql_error(conn));
					exit(1);
				}

			}

			listadePartidas.partidas[IndicePartida].ID = 0; /*Ponemos a 0 el ID de partida para que quede disponible para una nueva partida*/
			listadePartidas.partidas[IndicePartida].UsuariosPartida.num = 0; /*Ponemos a cero los usuarios de la partida*/
			listadePartidas.partidas[IndicePartida].UsuariosInvitados.num = 0; /*Ponemos a cero los usuarios invitados*/

		}
		else if (codigo==13)//Comando para enviar invitación(lo recibe el servidor del anfitrión)
		{
			int ignorar = 0;
			char anfitrion[20];
			DameNombre(&UsuariosConectados,sock_conn,anfitrion);
			int IndicePartida;
			char invitado[20];
			printf("cod 13, p = %s , anfitrion = %s , socket = %d\n", p, anfitrion, sock_conn);

			p=strtok(NULL, "/");
			IndicePartida = atoi(p);
			p=strtok(NULL, "/");
			strcpy(invitado, p);

			/* filtro para evitar invitar a alguien que ya esté invitado o en partida */
			for (int i = 0; i < listadePartidas.partidas[IndicePartida].UsuariosInvitados.num; i++)
				if (strcmp(invitado, listadePartidas.partidas[IndicePartida].UsuariosInvitados.sockets[i].usuario) == 0)
					ignorar = 1;
			for (int i = 0; i < listadePartidas.partidas[IndicePartida].UsuariosPartida.num; i++)
				if (strcmp(invitado, listadePartidas.partidas[IndicePartida].UsuariosPartida.sockets[i].usuario) == 0)
					ignorar = 1;

			if (ignorar == 0)
			{
				int socketInvitado = DameSocket(&UsuariosConectados, invitado);
				sprintf(peticion,"14/%d/%s", IndicePartida, anfitrion);
				write(socketInvitado, peticion, strlen(peticion));
				pthread_mutex_lock(&mutex);
				int res = Add(&listadePartidas.partidas[IndicePartida].UsuariosInvitados, socketInvitado, invitado,0);
				pthread_mutex_unlock(&mutex);
				if (res != 0)
					printf("Error al añadir usuario en invitación\n");
			}

		}
		else if (codigo==14)//Respuesta a invitación enviada
		{
			char invitado[20];
			DameNombre(&UsuariosConectados, sock_conn, invitado);
			int IndicePartida;
			char anfitrion[20];
			char sino[3];
			printf("cod 14, p = %s , invitado = %s , socket = %d\n",p,invitado,sock_conn);
			p = strtok(NULL, "/");
			IndicePartida = atoi(p);
			p = strtok(NULL, "/");
			strcpy(anfitrion, p);
			p = strtok(NULL, "/");
			strcpy(sino, p);

			int socketAnfitrion = DameSocket(&UsuariosConectados, anfitrion);

			sprintf(respuesta, "13/%d/%s/%s", IndicePartida, invitado,sino);
			write(socketAnfitrion, respuesta, strlen(respuesta));
			
			if(strcmp(sino, "SI")==0)
			{

				for (int i = 0; i < listadePartidas.partidas[IndicePartida].UsuariosPartida.num; i++)
				{				
					sprintf(respuesta, "17/%s/0", listadePartidas.partidas[IndicePartida].UsuariosPartida.sockets[i].usuario);
					write(sock_conn, respuesta, strlen(respuesta));					
					usleep(100000);  // hacemos una pausa de 100 ms para que los mensajes no se junten
				}
				pthread_mutex_lock(&mutex);
				int res = Add(&listadePartidas.partidas[IndicePartida].UsuariosPartida, sock_conn, invitado,0);
				if (res != 0)
					printf("Error al añadir usuario al mover de invitado a conectado\n");
				int eliminar = DesconectarUsuario(&listadePartidas.partidas[IndicePartida].UsuariosInvitados, invitado);
				if (eliminar!=0)
					printf("Error al eliminar usuario al mover usuario de invitado a conectado\n");				
				pthread_mutex_unlock(&mutex);
				/* Comunicamos a todos los usuarios en partida que se incorpora un nuevo usuario y los puntos en juego */
				sprintf(respuesta, "17/%s/%d", invitado, 10 * listadePartidas.partidas[IndicePartida].UsuariosPartida.num);
				printf("mensaje 17: %s\n", respuesta);
				for (int i = 0; i < listadePartidas.partidas[IndicePartida].UsuariosPartida.num; i++)
				{
					write(listadePartidas.partidas[IndicePartida].UsuariosPartida.sockets[i].socket, respuesta, strlen(respuesta));
				}

			}
		}
		else if (codigo==15)//Mensajes del servidor 
		{
			char usuario[20];
			DameNombre(&UsuariosConectados, sock_conn, usuario);
			int IndicePartida;
			char mensaje[20];
			p = strtok(NULL, "/");
			IndicePartida = atoi(p);
			p = strtok(NULL, "/");
			strcpy(mensaje, p);
			sprintf(respuesta, "15/%s: %s", usuario, mensaje);
			
			for(int i=0;i < listadePartidas.partidas[IndicePartida].UsuariosPartida.num; i++)
			{
				write(listadePartidas.partidas[IndicePartida].UsuariosPartida.sockets[i].socket, respuesta, strlen(respuesta));
				
			}
			
		}
		else if (codigo == 21)// Mensaje del cliente que canta bingo 
		{
			int BingoCorrecto = 0;
			char usuario[20];
			DameNombre(&UsuariosConectados, sock_conn, usuario);
			int IndicePartida;
			char mensaje[20];
			p = strtok(NULL, "/");
			IndicePartida = atoi(p);
		
			pthread_mutex_lock(&mutexPartidas);
			if (listadePartidas.partidas[IndicePartida].HayBingo == 0)
			{
				listadePartidas.partidas[IndicePartida].HayBingo = 1;
				strcpy(listadePartidas.partidas[IndicePartida].ganador, usuario);
				time_t t = time(NULL);
				listadePartidas.partidas[IndicePartida].duracion = t - listadePartidas.partidas[IndicePartida].tInicio;
				BingoCorrecto = 1;
			}
			pthread_mutex_unlock(&mutexPartidas);


			if (BingoCorrecto == 1)
			{
				sprintf(respuesta, "15/<<<< %s ha cantado BINGO!!! >>>>", usuario);
				for (int i = 0; i < listadePartidas.partidas[IndicePartida].UsuariosPartida.num; i++)
				{
					write(listadePartidas.partidas[IndicePartida].UsuariosPartida.sockets[i].socket, respuesta, strlen(respuesta));
				}
				sprintf(respuesta, "21/");
				for (int i = 0; i < listadePartidas.partidas[IndicePartida].UsuariosPartida.num; i++)
				{
					write(listadePartidas.partidas[IndicePartida].UsuariosPartida.sockets[i].socket, respuesta, strlen(respuesta));
				}

				char consulta[150];
				sprintf(consulta, "UPDATE Partida SET ganador='%s', duracion=%d, fecha='%s' WHERE id=%d", 
					listadePartidas.partidas[IndicePartida].ganador, 
					listadePartidas.partidas[IndicePartida].duracion,
					listadePartidas.partidas[IndicePartida].fecha,
					listadePartidas.partidas[IndicePartida].ID);
				err = mysql_query(conn, consulta);
				if (err != 0)
				{
					printf("Error al insertar partida %u %s \n", mysql_errno(conn), mysql_error(conn));
					exit(1);
				}
			}
		}

	}
	close(sock_conn);
	mysql_close (conn);
	
}


void Inicializar()
{
	int i;
	for(int i=0; i<MAX_PARTIDAS; i++)
		listadePartidas.partidas[i].ID=0;
	listadePartidas.num=0;
	
}

int main(int argc, char *argv[])
{	
	UsuariosConectados.num=0;
	listadePartidas.num=0;
	
	int sock_conn, sock_listen;
	struct sockaddr_in serv_adr;
	
	Inicializar();
	
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creant socket");
	
	memset(&serv_adr, 0, sizeof(serv_adr));
	serv_adr.sin_family = AF_INET;
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	
	serv_adr.sin_port = htons(50004);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind\n");
	
	if (listen(sock_listen, 2) < 0)
		printf("Error en el Listen");
	
	
	pthread_t thread;
	for(;;)
	{
		printf ("Escuchando\n");
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexion \n");
		
		pthread_create(&thread, NULL, AtenderalCliente,&sock_conn);
		
	}
}
	
