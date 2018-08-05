using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;


public class Client {

    private TcpClient tcpclient;

    public Client(String ip,int port){
        Console.WriteLine("Client Start");
        tcpclient = new TcpClient();
        try{  
            Console.WriteLine("Connecting.....");
            tcpclient.Connect(ip,port);
            Console.WriteLine("Connected");
        }catch (Exception e) {
            Console.WriteLine("Client Error..... " + e.StackTrace);
        }
    }

    public String Write(){
        try{
            Console.Write("Enter the string to be transmitted : ");
            
            String str = Console.ReadLine();
            //adds 4 byte header to string
            String header = ""+str.Length;
            while(header.Length<4){
                header = "0"+header;
            }
            str = header+str;

            Stream stm = tcpclient.GetStream();
                        
            ASCIIEncoding asen= new ASCIIEncoding();
            byte[] ba=asen.GetBytes(str);
            Console.WriteLine("Transmitting.....: " + str);
            
            stm.Write(ba,0,ba.Length);
            return str;
        }catch(Exception e) {
            Console.WriteLine("Write Error..... " + e.StackTrace);
            return "WRITEERROR";
        }
    }
/*
    public String Write(String s){
        try{
            Stream stm = tcpclient.GetStream();
                        
            ASCIIEncoding asen= new ASCIIEncoding();
            byte[] ba=asen.GetBytes(s);
            Console.WriteLine("Transmitting.....");
            
            stm.Write(ba,0,ba.Length);
            return s;
        }catch(Exception e) {
            Console.WriteLine("Write Error..... " + e.StackTrace);
            return "WRITEERROR";
        }
    }
*/
    public int Read(){
        try{
            NetworkStream stm = tcpclient.GetStream();
            while(stm.DataAvailable){
                byte[] bb=new byte[100];
                int k=stm.Read(bb,0,100);
                String read = "";
                for (int i=0;i<k;i++)
                    read = read + Convert.ToChar(bb[i]);
                Console.WriteLine(read);              
            }
            return 0;
        }catch(Exception e) {
            Console.WriteLine("Read Error..... " + e.StackTrace);
            return -1;
        }
    }
    
    public void Start(){
        bool reading = true;
        while(reading){
            String a = Write();
            if (a.Equals("STOP") || a.Equals("WRITEERROR")){
                reading = false;
            }
            Read();
        }
        Close();
    }

    public void Close(){
        tcpclient.Close();
    }


    public static void Main() {
        Client c1 = new Client("127.0.0.1",8001);
        c1.Start();
    }

}
