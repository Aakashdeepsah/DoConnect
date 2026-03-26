// app.module.ts — Sprint 2 final
// Changes: added MyQuestionsComponent, SessionBannerComponent, /my-questions route
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule, Routes } from '@angular/router';

import { AppComponent }             from './app.component';
import { NavbarComponent }          from './components/navbar/navbar.component';
import { LandingComponent }         from './components/landing/landing.component';
import { LoginComponent }           from './components/login/login.component';
import { RegisterComponent }        from './components/register/register.component';
import { HomeComponent }            from './components/home/home.component';
import { AskQuestionComponent }     from './components/ask-question/ask-question.component';
import { QuestionDetailComponent }  from './components/question-detail/question-detail.component';
import { AdminDashboardComponent }  from './components/admin/admin-dashboard.component';
import { MyQuestionsComponent }     from './components/my-questions/my-questions.component';     // Sprint 2
import { SessionBannerComponent }   from './components/session-banner/session-banner.component'; // Sprint 2

import { AuthInterceptor } from './interceptors/auth.interceptor';
import { AuthGuard }       from './guards/auth.guards';
import { AdminGuard }      from './guards/admin.guards';

const routes: Routes = [
  { path: '',              component: LandingComponent },
  { path: 'home',          component: HomeComponent },
  { path: 'login',         component: LoginComponent },
  { path: 'register',      component: RegisterComponent },
  { path: 'ask',           component: AskQuestionComponent,    canActivate: [AuthGuard] },
  { path: 'question/:id',  component: QuestionDetailComponent },
  { path: 'admin',         component: AdminDashboardComponent, canActivate: [AdminGuard] },
  { path: 'my-questions',  component: MyQuestionsComponent,    canActivate: [AuthGuard] }, // Sprint 2
  { path: '**',            redirectTo: '' }
];

@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    LandingComponent,
    LoginComponent,
    RegisterComponent,
    HomeComponent,
    AskQuestionComponent,
    QuestionDetailComponent,
    AdminDashboardComponent,
    MyQuestionsComponent,     // Sprint 2
    SessionBannerComponent,   // Sprint 2
  ],
  imports: [
    BrowserModule, FormsModule, ReactiveFormsModule,
    HttpClientModule, RouterModule.forRoot(routes)
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
