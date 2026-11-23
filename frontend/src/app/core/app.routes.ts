import { Routes } from '@angular/router';
import { Home } from './home/home';
import { Login } from './auth/login/login';
import { Register } from './auth/register/register';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  {
    path: 'exploration',
    loadChildren: () =>
      import('../modules/exploration/exploration.routes').then((m) => m.explorationRoutes),
  },
  {
    path: 'social',
    loadChildren: () => import('../modules/social/social.routes').then((m) => m.socialRoutes),
  },
];
