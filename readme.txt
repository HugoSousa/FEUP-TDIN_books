Os elementos do grupo s�o:
Francisco Miguel Amaro Maciel - 201100692
Hugo Miguel Ribeiro de Sousa � 201100690
Ricardo Daniel Soares da Silva - 201108043

-------------------------------------------

A solu��o � composta por 7 projetos (4 da loja e 3 do armaz�m). Mais informa��o 
acerca destes pode ser lida no relat�rio final.


O sistema MSMQ deve ser ativado no sistema operativo, e deve ser criada 
manualmente uma fila de mensagens privada com o nome warehouse_books.

Os projetos OrderSite e WarehouseService devem estar a correr no IIS. 

Para evitar a configura��o manual de portas (5440 at� 5444) que s�o usadas
para comunica��o de canal duplex dos servi�os, deve-se executar as aplica��es 
(ou o Visual Studio, caso seja executado a partir deste) em modo de administrador. 
Caso n�o seja corrido em modo de administrador, nem as portas sejam abertas manualmente, 
ser�o lan�adas exce��es relativas a estas portas.

No lado do armaz�m, caso o servidor n�o esteja a correr, as mensagens da fila n�o ser�o lidas 
e n�o chegar�o novas encomendas, pelo que esta aplica��o deve estar a correr simultaneamente 
com a GUI.
