import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { QuestionService } from '../../services/question.service';
import { AnswerService } from '../../services/answer.service';
import { AuthService } from '../../services/auth.service';
import { Question, Answer } from '../../models/models';

@Component({
  selector: 'app-question-detail',
  templateUrl: './question-detail.component.html',
  styleUrls: ['./question-detail.component.css']
})
export class QuestionDetailComponent implements OnInit {
  question: Question | null = null;
  answers: Answer[] = [];
  isLoading: boolean = true;
  answersLoading: boolean = false;
  errorMessage: string = '';
  isLoggedIn: boolean = false;
  answerText: string = '';
  answerFile: File | null = null;
  answerImagePreview: string | null = null;
  answerSubmitting: boolean = false;
  answerError: string = '';
  answerSuccess: string = '';

  constructor(
    private route: ActivatedRoute,
    private questionService: QuestionService,
    private answerService: AnswerService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.isLoggedIn = this.authService.isLoggedIn();

    const idParam = this.route.snapshot.paramMap.get('id');
    if (!idParam) {
      this.errorMessage = 'Invalid question ID.';
      this.isLoading = false;
      return;
    }

    const questionId = parseInt(idParam, 10);
    if (isNaN(questionId)) {
      this.errorMessage = 'Invalid question ID.';
      this.isLoading = false;
      return;
    }

    this.loadQuestion(questionId);
    this.loadAnswers(questionId);
  }

  loadQuestion(id: number): void {
    this.questionService.getById(id).subscribe({
      next: (data) => {
        this.question = data;
        this.isLoading = false;
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.status === 404 ? 'Question not found.' : 'Failed to load question.';
      }
    });
  }

  loadAnswers(questionId: number): void {
    this.answersLoading = true;
    this.answerService.getAnswers(questionId).subscribe({
      next: (data) => {
        this.answers = data;
        this.answersLoading = false;
      },
      error: () => {
        this.answersLoading = false;
      }
    });
  }

  onAnswerFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const file = input.files[0];
    if (file.size > 5 * 1024 * 1024) {
      this.answerError = 'Image too large (max 5MB).';
      return;
    }

    this.answerFile = file;
    this.answerError = '';

    const reader = new FileReader();
    reader.onload = (e) => {
      this.answerImagePreview = e.target?.result as string;
    };
    reader.readAsDataURL(file);
  }

  removeAnswerFile(event: Event): void {
    event.stopPropagation();
    this.answerFile = null;
    this.answerImagePreview = null;
  }

  submitAnswer(): void {
    this.answerError = '';
    this.answerSuccess = '';

    if (!this.answerText.trim() || !this.question) return;

    this.answerSubmitting = true;

    const formData = new FormData();
    formData.append('questionId', this.question.questionId.toString());
    formData.append('answerText', this.answerText.trim());

    if (this.answerFile) {
      formData.append('image', this.answerFile, this.answerFile.name);
    }

    this.answerService.create(formData).subscribe({
      next: () => {
        this.answerSubmitting = false;
        this.answerSuccess = 'Answer submitted! It will appear after admin approval.';
        this.answerText = '';
        this.answerFile = null;
        this.answerImagePreview = null;

        if (this.question) {
          this.loadAnswers(this.question.questionId);
        }
      },
      error: (err) => {
        this.answerSubmitting = false;
        this.answerError = err.error?.message || 'Failed to post answer.';
      }
    });
  }

  onImageError(event: Event): void {
    (event.target as HTMLImageElement).style.display = 'none';
  }
}