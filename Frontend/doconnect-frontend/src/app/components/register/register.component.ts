// register.component.ts — Sprint 2
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  username        = '';
  email           = '';
  password        = '';
  confirmPassword = '';  // Sprint 2
  showPassword    = false; // Sprint 2: toggle visibility
  isLoading       = false;
  errorMessage    = '';

  constructor(private auth: AuthService, private router: Router) {}

  // Sprint 2: password strength meter
  get passwordStrengthPercent(): number {
    const p = this.password;
    if (p.length === 0) return 0;
    let score = 0;
    if (p.length >= 6)  score += 25;
    if (p.length >= 10) score += 25;
    if (/[A-Z]/.test(p)) score += 25;
    if (/[0-9!@#$%^&*]/.test(p)) score += 25;
    return score;
  }

  get passwordStrengthLabel(): string {
    const s = this.passwordStrengthPercent;
    if (s <= 25) return 'Weak';
    if (s <= 50) return 'Fair';
    if (s <= 75) return 'Good';
    return 'Strong';
  }

  onRegister(): void {
    this.errorMessage = '';
    if (!this.username || !this.email || !this.password) {
      this.errorMessage = 'Please fill in all fields.'; return;
    }
    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'Passwords do not match.'; return;
    }
    this.isLoading = true;
    this.auth.register({
      username:        this.username.trim(),
      email:           this.email.trim(),
      password:        this.password,
      confirmPassword: this.confirmPassword,
      role:            'User'
    }).subscribe({
      next: () => { this.isLoading = false; this.router.navigate(['/home']); },
      error: (e) => {
        this.isLoading = false;
        this.errorMessage = e.status === 400
          ? e.error?.message || 'Username or email already taken.'
          : e.status === 0 ? 'Cannot reach the server.'
          : 'Something went wrong.';
      }
    });
  }
}
