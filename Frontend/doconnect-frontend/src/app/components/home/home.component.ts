import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { QuestionService } from '../../services/question.service';
import { AuthService } from '../../services/auth.service';
import { Question } from '../../models/models';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  questions: Question[]     = [];
  isLoading: boolean        = false;
  errorMessage: string      = '';
  searchQuery: string       = '';
  activeSearchQuery: string = '';
  isSearching: boolean      = false;
  isLoggedIn: boolean       = false;

  constructor(
    private questionService: QuestionService,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.isLoggedIn = this.authService.isLoggedIn();
    this.route.queryParams.subscribe(params => {
      if (params['search']) {
        this.searchQuery = params['search'];
        this.doSearch();
      } else {
        this.loadAllQuestions();
      }
    });
  }

  loadAllQuestions(): void {
    this.isLoading = true; this.errorMessage = ''; this.isSearching = false;
    this.questionService.getApproved().subscribe({
      next: (data) => { this.questions = data; this.isLoading = false; },
      error: ()    => { this.isLoading = false; this.errorMessage = 'Failed to load questions. Please refresh.'; }
    });
  }

  onSearchInput(): void {
    if (!this.searchQuery.trim()) this.clearSearch();
  }

  doSearch(): void {
    const q = this.searchQuery.trim();
    if (!q) { this.loadAllQuestions(); return; }
    this.isLoading = true; this.errorMessage = '';
    this.isSearching = true; this.activeSearchQuery = q;
    this.questionService.search(q).subscribe({
      next: (data) => { this.questions = data; this.isLoading = false; },
      error: ()    => { this.isLoading = false; this.errorMessage = 'Search failed. Please try again.'; }
    });
  }

  clearSearch(): void {
    this.searchQuery = ''; this.activeSearchQuery = ''; this.isSearching = false;
    this.loadAllQuestions();
  }

  goToQuestion(id: number): void { this.router.navigate(['/question', id]); }

  onImageError(event: Event): void {
    (event.target as HTMLImageElement).style.display = 'none';
  }
}
