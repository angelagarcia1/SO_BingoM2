#include <string.h>
#include <stdlib.h>
#include <stdio.h>
#include <mysql.h>
int main(int argc, char *argv[]) {
	
	MYSQL *conn;
	int err;
	// Estructura especial para almacenar resultados de consultas 
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	//Creamos una conexion al servidor MYSQL 
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "Juego",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	char consulta[80];
	char respuesta[512];
	strcpy(consulta,"SELECT Partida.id FROM (Partida, Resultado) WHERE Partida.Ganador = 'Berta' AND Partida.id = Resultado.Partida");
	err=mysql_query (conn, consulta);
	if (err!=0)
	{
		printf("Error al realizar la consulta %u %s \n", mysql_errno(conn), mysql_error(conn));
		exit(1);
	}
	resultado=mysql_store_result(conn);
	row = mysql_fetch_row(resultado);
	if (row == NULL)
		strcpy(respuesta,"3/Base de datos vacia");
	else
	{
		while (row!=NULL){
			printf ("%s\n", row[0]);
			row = mysql_fetch_row (resultado);
		}
	}
	mysql_close(conn);
	exit(0);
}
	
