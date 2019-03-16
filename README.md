# Client and Server Messaging

Simple server/client with math funtionality and reverse string.

## Getting Started

- Download the repository 

* Server side: Set your own ip address into [Program.cs](https://github.com/MarcoAcquaviva/MessageServer/blob/master/ServerMessagingApp/ServerMessagingApp/Program.cs) 
	```
	transport.Bind("Your_Net_IP", Port);
	```

P.S. You need two different project to use Client and Server on same machine

### Technical Information

- Packet command server side:



* **Command* - Function
	```
	Packet recived
	```
	
	
	
* **0** - String reverse
	```
	Packet packet = new Packet(0,"Client_String");
	```
* **1** - Addition - *type*('i' => integer || 'f' => float)
	```
	Packet packet = new Packet(1,'type',numA,numB);
	```
* **2** - Substraction - *type*('i' => integer || 'f' => float)
	```
	Packet packet = new Packet(2,'type',numA,numB);
	```
* **3** - Multiply - *type*('i' => integer || 'f' => float)
	```
	Packet packet = new Packet(3,'type',numA,numB);
	```
* **4** - Division - *type*('i' => integer || 'f' => float)
	```
	Packet packet = new Packet(4,'type',numA,numB);
	```

- Packet command client side:
* **0** - String 
	```
	Packet packet = new Packet(0,"Server_String");
	```
* **1** - Number - *type*('i' => integer || 'f' => float)
	```
	Packet packet = new Packet(1,type,number);
	```
	
	
## Authors
* **Acquaviva Marco** - [MarcoAcquaviva](https://github.com/MarcoAcquaviva)
* **Ippoliti Simone** - [SimoneIppoliti](https://github.com/simoneippoliti)

