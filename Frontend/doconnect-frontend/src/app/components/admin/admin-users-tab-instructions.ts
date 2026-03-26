// Sprint 2: Add a Users tab to the admin dashboard
// Add this as a third tab in admin-dashboard.component.html

// ── PASTE THIS BLOCK after the Answers tab in admin-dashboard.component.html ──
/*
    <button class="tab-btn" [class.active]="activeTab==='users'"
      (click)="activeTab='users'; loadUsers()">
      Users <span class="tab-count">{{ users.length }}</span>
    </button>

    <!-- USERS TAB -->
    <div *ngIf="activeTab==='users' && !isLoading">
      <p class="empty-msg" *ngIf="users.length===0">No users found.</p>
      <div class="users-table" *ngIf="users.length > 0">
        <table>
          <thead>
            <tr>
              <th>Username</th>
              <th>Email</th>
              <th>Role</th>
              <th>Questions</th>
              <th>Answers</th>
              <th>Joined</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let u of users" [class.admin-row]="u.role === 'Admin'">
              <td><strong>{{ u.username }}</strong></td>
              <td>{{ u.email }}</td>
              <td>
                <span class="badge" [class.badge-admin]="u.role==='Admin'"
                  [class.badge-user]="u.role==='User'">{{ u.role }}</span>
              </td>
              <td>{{ u.questionCount }}</td>
              <td>{{ u.answerCount }}</td>
              <td>{{ u.createdAt | date:'MMM d, y' }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
*/

// ── ADD TO admin-dashboard.component.ts ──
// 1. Import UserSummary from models
// 2. Add: users: UserSummary[] = [];
// 3. Add this method:
/*
  loadUsers(): void {
    if (this.users.length > 0) return;
    this.isLoading = true;
    this.adminService.getAllUsers().subscribe({
      next: (data) => { this.users = data; this.isLoading = false; },
      error: ()    => { this.isLoading = false; this.showError('Failed to load users.'); }
    });
  }
*/

// ── ADD TO admin.service.ts ──
/*
  getAllUsers(): Observable<UserSummary[]> {
    return this.http.get<UserSummary[]>(`${this.apiUrl}/users`);
  }
*/
export {};
