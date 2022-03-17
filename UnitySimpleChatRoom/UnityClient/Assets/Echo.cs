using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets; //API
using UnityEngine.UI;
using System;

public class Echo : MonoBehaviour
{
    Socket socket;
    public InputField inputField;
    public Text text;

    byte[] readBuff=new byte[1024];
    string  recvStr="";
    
    public void Update()
    {
        text.text=recvStr;
    }



    public void Connection()
    {
        //地址族(v4 or v6)  套接字类型(Dgram Raw RDM..)   协议(TCP UDP)
        socket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        //Connect是一个阻塞方法  程序会卡住直到服务端回应(接收 拒绝 或超时)
        socket.BeginConnect("127.0.0.1",8888,ConnectCallBack,socket);
    }

    public void Send()
    {
        string sendStr=inputField.text;
        byte[] sendBytes=System.Text.Encoding.Default.GetBytes(sendStr);
        //send也是阻塞方法 接收一个byte[]类型的参数指明要发送的内容.返回值是发送数据的长度
        //System.Text.Encoding.Default.GetBytes(字符串转byte[]数组)
        socket.BeginSend(sendBytes,0,sendBytes.Length,0,SendCallBack,socket);
    }

    public void SendCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket=(Socket)ar.AsyncState;
            int count=socket.EndSend(ar);
            Debug.Log("Socket Send succ"+count);
        }
        catch(SocketException  ex)
        {
            Debug.Log("Socket Send fail"+ex.ToString());

        }
    }

    public void ConnectCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket=(Socket) ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log("Socker Connect Succ");
            socket.BeginReceive(readBuff,0,1024,0,ReciveCallBack,socket);
        }
        catch(SocketException ex)//如果连接失败
        {
            Debug.Log("Socker Connect fail"+ex.ToString());
        }
    }
    public void ReciveCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket=(Socket)ar.AsyncState;
            int count=socket.EndReceive(ar);

            string s=System.Text.Encoding.Default.GetString(readBuff,0,count);
            recvStr=s+"\n"+recvStr;

            socket.BeginReceive(readBuff,0,1024,0,ReciveCallBack,socket); //接收完一串数据 等待下一串到来
        }
        catch(SocketException ex)
        {
            Debug.Log("Socket Recive fail"+ex.ToString());
        }
    }
}
