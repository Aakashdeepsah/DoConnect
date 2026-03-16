import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../services/admin.service';
import { Question, Answer } from '../../models/models';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {

  activeTab: 'questions' | 'answers' = 'questions';
  questions: Question[] = [];
  answers: Answer[]     = [];
  questionFilter        = 'All';
  answerFilter          = 'All';
  isLoading             = false;
  successMessage        = '';
  errorMessage          = '';
  pendingCount          = 0;

  constructor(private adminService: AdminService) {}

  ngOnInit(): void { this.loadQuestions(); this.loadPendingCount(); }

  get filteredQuestions(): Question[] {
    return this.questionFilter === 'All' ? this.questions : this.questions.filter(q => q.status === this.questionFilter);
  }
  get filteredAnswers(): Answer[] {
    return this.answerFilter === 'All' ? this.answers : this.answers.filter(a => a.status === this.answerFilter);
  }

  loadQuestions(): void {
    this.isLoading = true; this.errorMessage = '';
    this.adminService.getAllQuestions().subscribe({
      next: (data) => { this.questions = data; this.isLoading = false; },
      error: ()    => { this.isLoading = false; this.errorMessage = 'Failed to load questions.'; }
    });
  }

  loadAnswers(): void {
    if (this.answers.length > 0) return;
    this.isLoading = true;
    this.adminService.getAllAnswers().subscribe({
      next: (data) => { this.answers = data; this.isLoading = false; },
      error: ()    => { this.isLoading = false; this.errorMessage = 'Failed to load answers.'; }
    });
  }

  loadPendingCount(): void {
    this.adminService.getPendingCount().subscribe({
      next: (d) => { this.pendingCount = d.pendingCount; },
      error: ()  => { this.pendingCount = 0; }
    });
  }

  approveQuestion(id: number): void {
    this.adminService.updateQuestionStatus(id, 'Approved').subscribe({
      next: () => { this.setQStatus(id, 'Approved'); this.showSuccess('Question approved!'); },
      error: () => this.showError('Failed to approve.')
    });
  }
  rejectQuestion(id: number): void {
    this.adminService.updateQuestionStatus(id, 'Rejected').subscribe({
      next: () => { this.setQStatus(id, 'Rejected'); this.showSuccess('Question rejected.'); },
      error: () => this.showError('Failed to reject.')
    });
  }
  deleteQuestion(id: number): void {
    if (!confirm('Delete this question?')) return;
    this.adminService.deleteQuestion(id).subscribe({
      next: () => { this.questions = this.questions.filter(q => q.questionId !== id); this.showSuccess('Deleted.'); this.loadPendingCount(); },
      error: () => this.showError('Failed to delete.')
    });
  }
  approveAnswer(id: number): void {
    this.adminService.updateAnswerStatus(id, 'Approved').subscribe({
      next: () => { this.setAStatus(id, 'Approved'); this.showSuccess('Answer approved!'); },
      error: () => this.showError('Failed to approve.')
    });
  }
  rejectAnswer(id: number): void {
    this.adminService.updateAnswerStatus(id, 'Rejected').subscribe({
      next: () => { this.setAStatus(id, 'Rejected'); this.showSuccess('Answer rejected.'); },
      error: () => this.showError('Failed to reject.')
    });
  }
  deleteAnswer(id: number): void {
    if (!confirm('Delete this answer?')) return;
    this.adminService.deleteAnswer(id).subscribe({
      next: () => { this.answers = this.answers.filter(a => a.answerId !== id); this.showSuccess('Deleted.'); this.loadPendingCount(); },
      error: () => this.showError('Failed to delete.')
    });
  }

  private setQStatus(id: number, status: string): void {
    const q = this.questions.find(q => q.questionId === id); if (q) q.status = status; this.loadPendingCount();
  }
  private setAStatus(id: number, status: string): void {
    const a = this.answers.find(a => a.answerId === id); if (a) a.status = status; this.loadPendingCount();
  }

  showSuccess(msg: string): void { this.successMessage = msg; this.errorMessage = ''; setTimeout(() => this.successMessage = '', 3000); }
  showError(msg: string): void   { this.errorMessage = msg; this.successMessage = ''; setTimeout(() => this.errorMessage = '', 4000); }

  onImageError(event: Event): void {
    const img = event.target as HTMLImageElement; img.style.display = 'none';
    const c = img.closest('.image-container') as HTMLElement; if (c) c.style.display = 'none';
  }
  onImageLoad(event: Event): void { (event.target as HTMLImageElement).style.display = 'block'; }
}
