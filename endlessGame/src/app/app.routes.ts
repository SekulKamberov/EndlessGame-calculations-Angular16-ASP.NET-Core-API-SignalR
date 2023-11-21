import { Routes } from '@angular/router';
import { AppComponent } from './app.component';

export const routes: Routes = [
    { path: '', pathMatch: 'full', redirectTo: 'app' },
    { path: 'app', component: AppComponent }, 
];
