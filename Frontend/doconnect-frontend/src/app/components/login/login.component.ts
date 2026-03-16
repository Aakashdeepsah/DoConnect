import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
@Component({ selector: 'app-login', templateUrl: './login.component.html', styleUrls: ['./login.component.css'] })
export class LoginComponent {
  email=''; password=''; isLoading=false; errorMessage='';
  constructor(private auth: AuthService, private router: Router) {}
  onLogin(): void {
    this.errorMessage='';
    if (!this.email||!this.password){this.errorMessage='Please fill in all fields.';return;}
    this.isLoading=true;
    this.auth.login({email:this.email,password:this.password}).subscribe({
      next:(r)=>{this.isLoading=false;this.router.navigate([r.role==='Admin'?'/admin':'/home']);},
      error:(e)=>{this.isLoading=false;this.errorMessage=e.status===401?'Invalid email or password.':e.status===0?'Cannot reach the server.':e.error?.message||'Something went wrong.';}
    });
  }
}
