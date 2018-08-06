using System;

public class MessageBoard{

    private String[] messages;
    private int size;
    public MessageBoard(){
        messages = new String[10];
        size = 0;
    }

    public void push(String s){
        messages[size] = s;
        size++;
    }

    public String pop(){
        String retval = messages[0];
        if (size>0){
            for (int i = 0; i<size-1;i++){
                messages[i] = messages[i+1];
            }
            messages[size-1] = null;
            size--;
        }
        return retval;
    }

    public String update(String s){
        String retval="";
        if (size==10){
            retval = pop();
        }
        push(s);
        return retval;
    }

    public String output(){
        String retval = "";
        for (int i=0; i<size;i++){
            retval += messages[i]+"\n";
        }
        return retval;
    }
    
    public int Length(){
        return size;
    }
}