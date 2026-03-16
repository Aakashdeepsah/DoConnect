import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';
import { AuthService } from '../../services/auth.service';
import { AdminService } from '../../services/admin.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit, OnDestroy {
  searchQuery  = '';
  isLoggedIn   = false;
  isAdmin      = false;
  username     = '';
  pendingCount = 0;
  menuOpen     = false;

  // FIX: store subscriptions so we can clean them up on destroy
  private routerSub!: Subscription;
  private pollInterval!: ReturnType<typeof setInterval>;

  constructor(
    private auth: AuthService,
    private adminService: AdminService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // FIX: Re-read auth state on EVERY navigation event.
    // This means after login/logout the navbar updates instantly.
    this.routerSub = this.router.events.pipe(
      filter(e => e instanceof NavigationEnd)
    ).subscribe(() => {
      this.refresh();
    });

    // Also refresh immediately on first load
    this.refresh();

    // FIX: Store interval ID so we can clear it on destroy (no memory leak)
    this.pollInterval = setInterval(() => {
      if (this.isAdmin) this.loadPendingCount();
    }, 30000);
  }

  // FIX: Clear interval and unsubscribe on destroy
  ngOnDestroy(): void {
    if (this.routerSub)    this.routerSub.unsubscribe();
    if (this.pollInterval) clearInterval(this.pollInterval);
  }

  refresh(): void {
    this.isLoggedIn = this.auth.isLoggedIn();
    this.isAdmin    = this.auth.isAdmin();
    this.username   = this.auth.getUsername() || '';
    if (this.isAdmin) this.loadPendingCount();
  }

  loadPendingCount(): void {
    this.adminService.getPendingCount().subscribe({
      next: (d) => { this.pendingCount = d.pendingCount; },
      error: ()  => { this.pendingCount = 0; }
    });
  }

  onSearch(): void {
    if (this.searchQuery.trim()) {
      this.router.navigate(['/home'], { queryParams: { search: this.searchQuery.trim() } });
      this.searchQuery = '';
      this.menuOpen = false;
    }
  }

  logout(): void {
    this.auth.logout();
    this.isLoggedIn = false; this.isAdmin = false;
    this.username = ''; this.pendingCount = 0;
    this.router.navigate(['/']);
  }

  toggleMenu(): void { this.menuOpen = !this.menuOpen; }
}
