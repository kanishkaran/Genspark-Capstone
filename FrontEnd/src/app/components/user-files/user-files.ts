import { Component } from '@angular/core';
import { FileArchiveService } from '../../services/fileUpload.service';
import { FileVersionService } from '../../services/fileVersion.service';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';
import { FileArchive, FileSummary } from '../../models/fileArchive.model';
import { FileVersion } from '../../models/fileVersion.model';
import { EmployeeService } from '../../services/employee.service';
import { Employee } from '../../models/employee.model';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatMenuModule } from '@angular/material/menu';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FileSummaryService } from '../../services/fileSummary.service';
import { MatDialog } from '@angular/material/dialog';
import { FileSummaryDialog } from '../dialogs/file-summary-dialog/file-summary-dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router } from '@angular/router';
import { FileVersionDialog } from '../dialogs/file-version-dialog/file-version-dialog';

@Component({
  selector: 'app-user-files',
  imports: [MatIconModule, MatCardModule, MatMenuModule, MatTooltipModule, MatProgressSpinnerModule],
  templateUrl: './user-files.html',
  styleUrl: './user-files.css'
})
export class UserFiles {

  currentUser: any;
  myFileArchives: FileArchive[] = [];
  fileVersions: FileVersion[] = [];
  currEmployee: Employee = {};
  fileSummary: FileSummary = { fileName: '', summary: '' }
  loading: boolean = false;

  constructor(
    private fileArchiveService: FileArchiveService,
    private fileVersionService: FileVersionService,
    private employeeService: EmployeeService,
    private authService: AuthService,
    private notificationService: NotificationService,
    private fileSummaryService: FileSummaryService,
    private dialog: MatDialog,
    private router: Router
  ) {
  }

  ngOnInit() {
  this.authService.currUser$.subscribe({
    next: (result) => {
      this.currentUser = result;
      this.loadEmployee(); 
    },
    error: (err) => {
      this.notificationService.showError(err.errors.error.fields);
    }
  });
}


  loadEmployee() {
    this.employeeService.getAllEmployee({ email: this.currentUser.username }).subscribe({
      next: (response: any) => {
        this.currEmployee = response.data.data.$values[0];
        this.loadFileArchives();
        console.log(this.currEmployee)
      },
      error: (err) => {
        this.notificationService.showError(err.errors.error.fields)
      }
    })
  }

  loadFileArchives() {
    if (!this.currEmployee?.firstName) return;
    this.fileArchiveService.getAll({ page: 1, pageSize: 100 }).subscribe({
      next: (response: any) => {
        this.myFileArchives = response.data.data.$values;

        this.myFileArchives = this.myFileArchives.filter(fa => fa.uploadedByName == this.currEmployee.firstName && !fa.status);
        console.log(this.myFileArchives)
      }
    })

  }

  openFileVersionDialog() {
    this.dialog.open(FileVersionDialog, {
      data: { versions: this.fileVersions }
    })
  }

  getFileVersionsForArchive(id: string) {

    this.fileVersionService.getByArchiveId(id).subscribe({
      next: (response: any) => {
        this.fileVersions = response.data.$values;
        if (this.fileVersions.length > 0) {
          this.openFileVersionDialog();
        } else {
          this.notificationService.showInfo('No versions available for this file.');
        }
      },
      error: (err) => {
        this.notificationService.showError(err.errors.error.fields);
      }
    });
  }

  downloadFile(fileName: string) {
    this.fileArchiveService.downloadFile(fileName).subscribe({
      next: (blob) => {
        console.log(blob)
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `${fileName}`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
        this.notificationService.showSuccess(` downloaded successfully`);
      },
      error: () => {
        this.notificationService.showError('Failed to download file version');
      }
    });
  }



  getFileSummary(file: FileArchive) {
    this.loading = true;
    this.fileSummaryService.getFileSummary(file.fileName).subscribe({
      next: (response: any) => {
        console.log(response)
        this.fileSummary = response;
        this.loading = false;
        this.openSummaryDialog(file);
      },
      error: (err) => {
        this.loading = false;
        this.notificationService.showError(err.message)
      }
    })
  }

  openSummaryDialog(file: any) {
    this.dialog.open(FileSummaryDialog, {
      width: '800px',
      data: { summary: this.fileSummary, file: file }
    })
  }

  navigate() {
    this.router.navigate(['home', 'upload'])
  }
}
