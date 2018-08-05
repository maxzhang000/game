using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class Server{
    
    private TcpListener myListener;
    private Socket sock;

    public Server(String ip, int port){
        Console.WriteLine("Server Start");
        IPAddress ipAd = IPAddress.Parse("127.0.0.1");
        try {
        
            myListener = new TcpListener(ipAd,8001);
            
            myListener.Start();

            Console.WriteLine("The server is running at port 8001...");    
            Console.WriteLine("The local End point is  :" + myListener.LocalEndpoint );
            Console.WriteLine("Waiting for a connection.....");
            sock=myListener.AcceptSocket();
            Console.WriteLine("Connection accepted from " + sock.RemoteEndPoint);
        }catch(Exception e) {
            Console.WriteLine("Server Error..... " + e.StackTrace);
        }
    }

    public String Read(){
        try {
            byte[] b = new byte[100];
            int k=sock.Receive(b);
            Console.WriteLine("Recieved...");
            String recievedStr = "";
            for (int i=0;i<k;i++)
                recievedStr = recievedStr + Convert.ToChar(b[i]);
            Console.Write(recievedStr);
            return recievedStr;
        }catch(Exception e){
            Console.WriteLine("Read Error..... " + e.StackTrace);
            return "READERROR";
        }
    }

    public String Write(String s){
        try{
            ASCIIEncoding asen=new ASCIIEncoding();
            sock.Send(asen.GetBytes(s));
            Console.WriteLine("\nSent Acknowledgement");
            return s;
        }catch(Exception e){
            Console.WriteLine("Read Error..... " + e.StackTrace);
            return "WRITEERROR";
        }
    }

    public void Close(){
        sock.Close();
        myListener.Stop();
    }

    public void Start(){
        bool reading = true;
        while(reading){
            String a = Read();
            if (a.Equals("STOP") || a.Equals("READERROR")){
                reading = false;
            }else{
                Write("The string was recieved by the server.");
            }
        }
        Close();
    }

    public static void Main(){
        Server s = new Server("127.0.0.1",8001);
        s.Start();
    }
}
