// session-banner.component.ts — Sprint 2
// Shows a warning banner when JWT token is about to expire (< 5 minutes)
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-session-banner',
  templateUrl: './session-banner.component.html',
  styleUrls: ['./session-banner.component.css']
})
export class SessionBannerComponent implements OnInit, OnDestroy {
  showBanner   = false;
  minutesLeft  = 0;
  dismissed    = false;
  private timer!: ReturnType<typeof setInterval>;

  constructor(private auth: AuthService, private router: Router) {}

  ngOnInit(): void {
    // Check session status every 30 seconds
    this.checkSession();
    this.timer = setInterval(() => this.checkSession(), 30000);
  }

  ngOnDestroy(): void {
    if (this.timer) clearInterval(this.timer);
  }

  checkSession(): void {
    if (!this.auth.isLoggedIn()) { this.showBanner = false; return; }

    const mins = this.auth.getSessionTimeLeft();
    this.minutesLeft = mins;

    // Show banner if session expires in less than 5 minutes and not dismissed
    if (mins >= 0 && mins < 5 && !this.dismissed) {
      this.showBanner = true;
    } else if (mins >= 5) {
      this.showBanner = false;
      this.dismissed  = false; // reset dismissed when session is renewed
    }

    // Auto logout when session expires
    if (mins === 0) {
      this.auth.logout();
      this.router.navigate(['/login']);
    }
  }

  renewSession(): void {
    // For now just dismiss — in a real app this would call a refresh-token endpoint
    this.dismiss();
  }

  dismiss(): void {
    this.showBanner = false;
    this.dismissed  = true;
  }
}
