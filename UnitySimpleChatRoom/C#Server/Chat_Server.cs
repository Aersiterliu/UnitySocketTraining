using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

class ClientState
{
    public Socket socket;
    public byte[] readBuff=new byte[1024];
}
class MainClass
{

        static Socket listenfd;
        static Dictionary<Socket,ClientState>clients=new Dictionary<Socket,ClientState>();
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Socket listenfd=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

            //Bind 
            IPAddress ipAdr=IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEp=new IPEndPoint(ipAdr,8888);
            listenfd.Bind(ipEp);

            //Listen 0表示不受限制
            listenfd.Listen(0);
            Console.WriteLine("[服务器]启动成功!");

            listenfd.BeginAccept(AcceptCallBack,listenfd);
            Console.ReadLine();        
        } 

        public static void AcceptCallBack(IAsyncResult ar)
       {
           try
           {
               Console.WriteLine("[服务器]Accept");
               Socket listenfd=(Socket)ar.AsyncState;
               Socket clientfd=listenfd.EndAccept(ar);

               //client列表
               ClientState state  =new ClientState();
               state.socket=clientfd;
               clients.Add(clientfd,state);
               //接收数据BeginReceive
               clientfd.BeginReceive(state.readBuff,0,1024,0,ReceiveCallBack,state);
               listenfd.BeginAccept(AcceptCallBack,listenfd);

           }
           catch(SocketException ex)
           {
            Console.WriteLine("Socket Accept fail"+ex.ToString());
           }
       }

       public static void ReceiveCallBack(IAsyncResult ar)
       {
           try
           {
               ClientState state =(ClientState)ar.AsyncState;
               Socket clientfd=state.socket;
               int count=clientfd.EndReceive(ar);
               //客户端关闭
               if(count==0)
               {
                   clientfd.Close();
                   clients.Remove(clientfd);
                   Console.WriteLine("Socket CLose");
                   return;
               }
               string recvStr=System.Text.Encoding.Default.GetString(state.readBuff,0,count);
               byte[]  sendBytes=System.Text.Encoding.Default.GetBytes("echo"+recvStr);

               foreach(ClientState s in clients.Values)
               {
                   s.socket.Send(sendBytes);
               }


               clientfd.BeginReceive(state.readBuff,0,1024,0,ReceiveCallBack,state);
           }
           catch(SocketException ex)
           {
               Console.WriteLine("Socket Recive fail"+ex.ToString());

           }
       }

}

