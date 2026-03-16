import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  username     = '';
  email        = '';
  password     = '';
  isLoading    = false;
  errorMessage = '';

  constructor(private auth: AuthService, private router: Router) {}

  onRegister(): void {
    this.errorMessage = '';
    if (!this.username || !this.email || !this.password) {
      this.errorMessage = 'Please fill in all fields.'; return;
    }
    this.isLoading = true;
    // FIX: role is hardcoded to 'User' — it is also ignored by the backend
    this.auth.register({
      username: this.username.trim(),
      email:    this.email.trim(),
      password: this.password,
      role:     'User'
    }).subscribe({
      next: () => { this.isLoading = false; this.router.navigate(['/home']); },
      error: (e) => {
        this.isLoading = false;
        this.errorMessage = e.status === 400 ? e.error?.message || 'Username or email already taken.'
          : e.status === 0 ? 'Cannot reach the server.'
          : 'Something went wrong. Please try again.';
      }
    });
  }
}
