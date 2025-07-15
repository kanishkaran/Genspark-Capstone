import { Component, Input, Output, EventEmitter, inject } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { Category } from '../../models/categoryModel';
import { CategoryService } from '../../services/category.service';
import { NotificationService } from '../../services/notification.service';

import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { FileUploadRequest } from '../../models/fileArchive.model';
import { FileArchiveService } from '../../services/fileUpload.service';
import { AuthService } from '../../services/auth.service';


@Component({
  selector: 'app-file-upload',
  standalone: true,
  imports: [MatFormFieldModule, MatIconModule, MatSelectModule, MatProgressBarModule, MatProgressSpinnerModule],
  templateUrl: './file-upload.html',
  styleUrl: './file-upload.css'
})
export class FileUpload {
  @Output() filesUploaded = new EventEmitter<void>();

  selectedFiles: File[] = [];
  selectedCategories: string[] = [];
  categories: Category[] = [];
  isDragOver = false;
  uploading = false;
  uploadProgress = 0;
  isAdmin = false;
  @Input() specificCategory = '';

  constructor(
    private fileArchiveService: FileArchiveService,
    private categoryService: CategoryService,
    private notificationService: NotificationService,
    private authService: AuthService
  ) {
    this.isAdmin = authService.isAdmin();
    if(!this.specificCategory){
      this.loadCategories();
    }
  }

  private loadCategories(): void {
    this.categoryService.getAll().subscribe((response: any) => {
      this.categories = response.data.data.$values;
      if (!this.isAdmin) {
        this.categories = this.categories.filter(c => c.access !== 'read-only')
      }
      console.log(this.categories)
    });
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    this.isDragOver = true;
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    this.isDragOver = false;
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    this.isDragOver = false;

    const files = Array.from(event.dataTransfer?.files || []);
    this.addFiles(files);
  }

  onFileSelected(event: Event): void {
    const files = Array.from((event.target as HTMLInputElement).files || []);
    this.addFiles(files);
  }

  private addFiles(files: File[]): void {
    this.selectedFiles.push(...files);
    this.selectedCategories.push(...new Array(files.length).fill(''));
  }

  removeFile(index: number): void {
    this.selectedFiles.splice(index, 1);
    this.selectedCategories.splice(index, 1);
  }

  clearFiles(): void {
    this.selectedFiles = [];
    this.selectedCategories = [];
  }

  allFilesHaveCategory(): boolean {
    return this.specificCategory ? true : this.selectedCategories.every(category => category !== '');
  }

  uploadFiles() {
    if (!this.allFilesHaveCategory()) {
      this.notificationService.showError('Please select a category for each file');
      return;
    }

    this.uploading = true;
    console.log('uploading', this.uploading);
    this.uploadProgress = 0;

    try {
      for (let i = 0; i < this.selectedFiles.length; i++) {
        const uploadRequest: FileUploadRequest = {
          file: this.selectedFiles[i],
          category: this.specificCategory ? this.specificCategory : this.selectedCategories[i]
        };

        this.fileArchiveService.uploadFile(uploadRequest).subscribe({
          next: () => {
            this.notificationService.showSuccess('Files uploaded successfully');
            console.log("Upload complete");
          },
          error: (err) =>{
            this.notificationService.showError(err.error.errors.fields + ` - ${uploadRequest.file.name}`);
          }
        });
      }

      this.filesUploaded.emit();
      this.clearFiles();
    } catch (error: any) {
      this.notificationService.showError(error.error.errors.fields);
      console.log(error)
    } finally {
      this.uploading = false;
      console.log("Upload complete", this.uploading);
      this.uploadProgress = 0;
    }
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }
}