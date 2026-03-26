// my-questions.component.ts — Sprint 2
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { QuestionService } from '../../services/question.service';
import { Question } from '../../models/models';

@Component({
  selector: 'app-my-questions',
  templateUrl: './my-questions.component.html',
  styleUrls: ['./my-questions.component.css']
})
export class MyQuestionsComponent implements OnInit {
  questions: Question[] = [];
  isLoading    = true;
  errorMessage = '';

  constructor(private questionService: QuestionService, private router: Router) {}

  ngOnInit(): void {
    this.questionService.getMine().subscribe({
      next: (data) => { this.questions = data; this.isLoading = false; },
      error: ()    => { this.isLoading = false; this.errorMessage = 'Failed to load your questions.'; }
    });
  }

  goTo(id: number): void { this.router.navigate(['/question', id]); }
}
