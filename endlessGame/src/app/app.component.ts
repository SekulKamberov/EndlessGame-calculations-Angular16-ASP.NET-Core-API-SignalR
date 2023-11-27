import { Component, Input, OnInit } from '@angular/core'
import { CommonModule } from '@angular/common'
import { RouterOutlet } from '@angular/router' 
import { FormsModule } from '@angular/forms'
import { SignalrService } from './services/signalr.service'
import { HttpClientModule, HttpClient, HttpHeaders } from '@angular/common/http' 
import * as signalR from "@microsoft/signalr"
import { Observable } from 'rxjs'
import { environment } from '../environments/environment' 
 
interface History {
  stepNumber: number
  operation: string 
}

interface User {
  username: string
  score: BigInt 
  history: string
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, FormsModule, HttpClientModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
}) 

export class AppComponent implements OnInit {
  url: string = environment.apiBaseUrl + '/user/'   
  username: string = ''
  input: number = 1
  oneTime: boolean = false
  maxscore: User = {username: '', score: BigInt(0), history: ''}
  maxscore$!: Observable<User> 
 
  auth: boolean = false; 

  steps: number = 0
  result: BigInt = BigInt(0)
  history: History[] = [] 
  gameHistory: string = ""
  hide: boolean = true  
 
  model = {username: this.username} 
  constructor(public signalRService: SignalrService, public http: HttpClient) { }
   
  ngOnInit() { 
    this.signalRService.startConnection() 
    this.signalRService.addBroadcastDataListener() 
    this.signalRService.maxScore$.subscribe((data) =>  {   
        this.maxscore.username = data.username  
        this.hide = this.input.toString().length > 2 ? false : true
        console.log('type', typeof this.input)
        console.log('input', this.input)
        this.maxscore.score = BigInt(data.score)  
    });     
  } 
 
  onSubmit() {  
    const httpOptions: Object = { headers: new HttpHeaders({'Content-Type': 'application/json'}) } 
    this.username = this.model.username    
    let history = this.gameHistory.length == 0 ? "|" :  this.gameHistory
    let resStr = this.result.toString()
     
    //this.http.get<User>(this.url + this.username + '/' + resStr + '/' + history).subscribe(
      this.http.post<User>(this.url, {username: this.username, score: resStr + "", history: history}, httpOptions)
        .subscribe((response)  => {  
          this.result = BigInt(response.score.toString()) ? BigInt(response.score.toString()) : BigInt(1) 
          this.input = Number(response.score)
          this.gameHistory += response.history ? response.history : ''  
          this.setItem("username", this.model.username)
          this.auth = true;
        },
        (error) => { console.log(error) })   
  }
 
  public setItem(key: string, data: string): void {  
    localStorage.setItem(key, JSON.stringify(data))
  }

  public getItem(key: string): string { 
    return JSON.parse(localStorage.getItem(key)?.toString() || "")
  } 

  undo( ) {
    if(this.steps > 2 || this.steps >= 1 || this.steps == 3 || this.steps !== 0) { 
      this.steps -= 1  
      let last: string = this.history.pop()?.operation || "" 
      let operation: string = (last === "+ 200") ? "- 200" : (last === "+ 20") ? "- 20" : (last === "* 2") ? "/ 2" : ""
        this.input = eval(this.input.toString()  + operation )

      if(this.steps >= 1 ) {  
        let indx = this.gameHistory.lastIndexOf(last)
        this.gameHistory = this.gameHistory.substring(0, indx)
        this.steps = 1
        this.oneTime = true
      } 

      if(this.steps === 0) {  
        let indx = this.gameHistory.lastIndexOf(last)
        this.gameHistory = this.gameHistory.substring(0, indx)
      }   
    } 
  }

  checkSteps() {  
    if(this.steps > 2) { 
      this.steps = 1
      this.input += 200
      this.history.push({stepNumber: this.steps, operation: "+ 200"})
     
      this.result = BigInt(this.input)
      console.log('this.result', this.result)
      this.gameHistory +=  " + 200"    

      const httpOptions: Object = { headers: new HttpHeaders({'Content-Type': 'application/json'}) }; 
      this.http.post<User>(this.url, {username: this.username, score: this.result + "", history: this.gameHistory }, httpOptions)
        .subscribe((response) => {  
          this.result = BigInt(response.toString())     
          this.setItem("username", this.model.username)
          this.auth = true
        },
        (error) => { console.log(error) })
    }
  }

  addTwenty() {
    this.steps += 1
    this.checkSteps()
    this.input = eval(this.input.toString()  + "+ 20")
    this.history.push({stepNumber: this.steps, operation:  "+ 20"})
    this.gameHistory += " + 20"
    this.hide = false  
    this.oneTime = false
  }
   
  multiplyTwo() {
    this.steps += 1
    this.checkSteps() 
    this.input = eval(this.input.toString()  + "* 2") 
    this.history.push({stepNumber: this.steps, operation: "* 2"})
    this.gameHistory += " * 2"
    this.hide = false  
    this.oneTime = false 
  } 
}  
