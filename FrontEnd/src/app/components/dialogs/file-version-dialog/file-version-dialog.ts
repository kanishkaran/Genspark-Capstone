import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FileVersion } from '../../../models/fileVersion.model';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { DatePipe } from '@angular/common';
import { FileArchiveService } from '../../../services/fileUpload.service';
import { NotificationService } from '../../../services/notification.service';

@Component({
  selector: 'app-file-version-dialog',
  imports: [MatCardModule, MatIconModule, DatePipe],
  templateUrl: './file-version-dialog.html',
  styleUrl: './file-version-dialog.css'
})
export class FileVersionDialog {

  fileVersions: FileVersion[] = [];
  constructor(
    public dialogRef: MatDialogRef<FileVersionDialog>,
    private fileArchiveService: FileArchiveService,
    private notificationService: NotificationService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
      if(data) {
        this.fileVersions = data.versions;
      }
  }

  downloadFileVersion(fileVersion: FileVersion) {
    this.fileArchiveService.downloadFile(fileVersion.fileName, fileVersion.versionNumber).subscribe({
      next: (blob) => {
        console.log(blob)
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `V${fileVersion.versionNumber}_${fileVersion.fileName}`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
        this.notificationService.showSuccess(` Version ${fileVersion.versionNumber} downloaded successfully`);
      },
      error: () => {
        this.notificationService.showError('Failed to download file version');
      }
    });
  }

  close(){
    this.data = null;
    this.dialogRef.close(null);
  }
}
