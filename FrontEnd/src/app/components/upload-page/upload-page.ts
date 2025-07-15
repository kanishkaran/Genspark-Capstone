import { Component } from '@angular/core';
import { FileUpload } from "../../shared/file-upload/file-upload";
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-upload-page',
  imports: [FileUpload, MatCardModule, MatIconModule],
  templateUrl: './upload-page.html',
  styleUrl: './upload-page.css'
})
export class UploadPage {
  onFilesUploaded(): void {

    console.log('Files uploaded successfully!');
  }
}
