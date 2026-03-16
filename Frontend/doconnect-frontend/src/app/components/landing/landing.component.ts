import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.css']
})
export class LandingComponent implements OnInit {
  isLoggedIn = false;
  isAdmin    = false;

  constructor(private auth: AuthService, private router: Router) {}

  ngOnInit(): void {
    this.isLoggedIn = this.auth.isLoggedIn();
    this.isAdmin    = this.auth.isAdmin();

    // If already logged in, redirect to the appropriate dashboard
    if (this.isLoggedIn) {
      this.router.navigate([this.isAdmin ? '/admin' : '/home']);
    }
  }
}
