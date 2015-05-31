Os elementos do grupo são:
Francisco Miguel Amaro Maciel - 201100692
Hugo Miguel Ribeiro de Sousa – 201100690
Ricardo Daniel Soares da Silva - 201108043

-------------------------------------------

A solução é composta por 7 projetos (4 da loja e 3 do armazém). Mais informação 
acerca destes pode ser lida no relatório final.


O sistema MSMQ deve ser ativado no sistema operativo, e deve ser criada 
manualmente uma fila de mensagens privada com o nome warehouse_books.

Os projetos OrderSite e WarehouseService devem estar a correr no IIS. 

Para evitar a configuração manual de portas (5440 até 5444) que são usadas
para comunicação de canal duplex dos serviços, deve-se executar as aplicações 
(ou o Visual Studio, caso seja executado a partir deste) em modo de administrador. 
Caso não seja corrido em modo de administrador, nem as portas sejam abertas manualmente, 
serão lançadas exceções relativas a estas portas.

No lado do armazém, caso o servidor não esteja a correr, as mensagens da fila não serão lidas 
e não chegarão novas encomendas, pelo que esta aplicação deve estar a correr simultaneamente 
com a GUI.
