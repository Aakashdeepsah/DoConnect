import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { QuestionService } from '../../services/question.service';

@Component({
  selector: 'app-ask-question',
  templateUrl: './ask-question.component.html',
  styleUrls: ['./ask-question.component.css']
})
export class AskQuestionComponent {
  topic: string = '';
  title: string = '';
  questionText: string = '';
  selectedFile: File | null = null;
  imagePreview: string | null = null;
  isLoading: boolean = false;
  errorMessage: string = '';
  successMessage: string = '';

  constructor(
    private questionService: QuestionService,
    private router: Router
  ) {}

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const file = input.files[0];

    if (file.size > 5 * 1024 * 1024) {
      this.errorMessage = 'Image is too large. Max 5MB.';
      return;
    }

    const allowed = ['image/jpeg', 'image/png', 'image/gif', 'image/webp'];
    if (!allowed.includes(file.type)) {
      this.errorMessage = 'Only JPG, PNG, GIF or WebP allowed.';
      return;
    }

    this.selectedFile = file;
    this.errorMessage = '';

    const reader = new FileReader();
    reader.onload = (e) => {
      this.imagePreview = e.target?.result as string;
    };
    reader.readAsDataURL(file);
  }

  removeFile(event: Event): void {
    event.stopPropagation();
    this.selectedFile = null;
    this.imagePreview = null;
  }

  onSubmit(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.topic || !this.title || !this.questionText) {
      this.errorMessage = 'Please fill in all required fields.';
      return;
    }

    this.isLoading = true;

    const formData = new FormData();
    formData.append('topic', this.topic);
    formData.append('title', this.title);
    formData.append('questionText', this.questionText);

    if (this.selectedFile) {
      formData.append('image', this.selectedFile, this.selectedFile.name);
    }

    this.questionService.create(formData).subscribe({
      next: () => {
        this.isLoading = false;
        this.successMessage = 'Question submitted! It will appear after admin approval.';
        this.topic = '';
        this.title = '';
        this.questionText = '';
        this.selectedFile = null;
        this.imagePreview = null;

        setTimeout(() => this.router.navigate(['/home']), 2000);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Failed to submit. Please try again.';
      }
    });
  }
}