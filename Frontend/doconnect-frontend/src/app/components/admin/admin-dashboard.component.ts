// components/admin/admin-dashboard.component.ts — Sprint 2
// Changes: added Users tab, loadUsers(), users array
import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../services/admin.service';
import { Question, Answer, UserSummary } from '../../models/models';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {

  activeTab: 'questions' | 'answers' | 'users' = 'questions';

  questions: Question[]    = [];
  answers: Answer[]        = [];
  users: UserSummary[]     = []; // Sprint 2

  qFilter      = 'All';
  aFilter      = 'All';
  isLoading    = false;
  successMsg   = '';
  errorMsg     = '';
  pendingCount = 0;

  constructor(private adminService: AdminService) {}

  ngOnInit(): void { this.loadQuestions(); this.loadPendingCount(); }

  get filteredQ(): Question[] { return this.qFilter === 'All' ? this.questions : this.questions.filter(q => q.status === this.qFilter); }
  get filteredA(): Answer[]   { return this.aFilter === 'All' ? this.answers   : this.answers.filter(a => a.status === this.aFilter); }

  loadQuestions(): void {
    this.isLoading = true;
    this.adminService.getAllQuestions().subscribe({
      next: (d) => { this.questions = d; this.isLoading = false; },
      error: ()  => { this.isLoading = false; this.showError('Failed to load questions.'); }
    });
  }

  loadAnswers(): void {
    if (this.answers.length > 0) return;
    this.isLoading = true;
    this.adminService.getAllAnswers().subscribe({
      next: (d) => { this.answers = d; this.isLoading = false; },
      error: ()  => { this.isLoading = false; this.showError('Failed to load answers.'); }
    });
  }

  // Sprint 2: load user list
  loadUsers(): void {
    if (this.users.length > 0) return;
    this.isLoading = true;
    this.adminService.getAllUsers().subscribe({
      next: (d) => { this.users = d; this.isLoading = false; },
      error: ()  => { this.isLoading = false; this.showError('Failed to load users.'); }
    });
  }

  loadPendingCount(): void {
    this.adminService.getPendingCount().subscribe({
      next: (d) => { this.pendingCount = d.pendingCount; },
      error: ()  => { this.pendingCount = 0; }
    });
  }

  approveQ(id: number): void { this.adminService.updateQuestionStatus(id,'Approved').subscribe({ next:()=>{ this.setQ(id,'Approved'); this.showSuccess('Approved!'); }, error:()=>this.showError('Failed.') }); }
  rejectQ(id: number): void  { this.adminService.updateQuestionStatus(id,'Rejected').subscribe({ next:()=>{ this.setQ(id,'Rejected'); this.showSuccess('Rejected.'); }, error:()=>this.showError('Failed.') }); }
  deleteQ(id: number): void  {
    if (!confirm('Delete this question?')) return;
    this.adminService.deleteQuestion(id).subscribe({ next:()=>{ this.questions=this.questions.filter(q=>q.questionId!==id); this.showSuccess('Deleted.'); this.loadPendingCount(); }, error:()=>this.showError('Failed.') });
  }
  approveA(id: number): void { this.adminService.updateAnswerStatus(id,'Approved').subscribe({ next:()=>{ this.setA(id,'Approved'); this.showSuccess('Approved!'); }, error:()=>this.showError('Failed.') }); }
  rejectA(id: number): void  { this.adminService.updateAnswerStatus(id,'Rejected').subscribe({ next:()=>{ this.setA(id,'Rejected'); this.showSuccess('Rejected.'); }, error:()=>this.showError('Failed.') }); }
  deleteA(id: number): void  {
    if (!confirm('Delete this answer?')) return;
    this.adminService.deleteAnswer(id).subscribe({ next:()=>{ this.answers=this.answers.filter(a=>a.answerId!==id); this.showSuccess('Deleted.'); this.loadPendingCount(); }, error:()=>this.showError('Failed.') });
  }

  private setQ(id: number, s: string): void { const q=this.questions.find(x=>x.questionId===id); if(q) q.status=s; this.loadPendingCount(); }
  private setA(id: number, s: string): void { const a=this.answers.find(x=>x.answerId===id);   if(a) a.status=s; this.loadPendingCount(); }

  showSuccess(m: string): void { this.successMsg=m; this.errorMsg='';   setTimeout(()=>this.successMsg='',3000); }
  showError(m: string): void   { this.errorMsg=m;   this.successMsg=''; setTimeout(()=>this.errorMsg='',4000); }

  onImageError(e: Event): void {
    const img = e.target as HTMLImageElement; img.style.display='none';
    const c = img.closest('.image-container') as HTMLElement; if(c) c.style.display='none';
  }
  onImageLoad(e: Event): void { (e.target as HTMLImageElement).style.display='block'; }
}
