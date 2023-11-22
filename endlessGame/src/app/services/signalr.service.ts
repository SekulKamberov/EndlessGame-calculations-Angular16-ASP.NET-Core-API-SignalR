import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { BehaviorSubject, Subject } from 'rxjs'; 
import { environment } from '../../environments/environment';
import { HttpClient } from "@angular/common/http";

interface User {
  username: string;
  score: number; 
}

@Injectable({
  providedIn: 'root'
})

export class SignalrService {   
  private hubConnection!: signalR.HubConnection; 
  private userScore = new Subject<number>();
  private maxScore = new Subject<User>(); 
 
  get maxScore$() {
    return this.maxScore.asObservable();
  }
 
  get userScore$() {
    return this.userScore.asObservable();
  } 
  
    public startConnection = () => {
      this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl('https://localhost:5001/chat')
        .build();
        
      this.hubConnection
        .start()
        .then(() => console.log('Connection started'))
        .catch(err => console.log('Error while starting connection: ' + err))
    }
     
    public broadcastData = (username: string) => { 
      console.log('broadcastdata')
      
      this.hubConnection.invoke('broadcastdata', username)
      .catch(err => console.error(err));
    }

    public addBroadcastDataListener = () => { 
      this.hubConnection.on('score', (data: User) => { 
        console.log('data.score ------------------------------', data.score);
        this.maxScore.next(data); 
         
      }) 
    } 
}
