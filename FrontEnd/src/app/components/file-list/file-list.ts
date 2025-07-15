import { Component } from '@angular/core';
import { DataTable, TableAction, TableColumn } from '../../shared/data-table/data-table';
import { FileArchive, semanticSearchFile } from '../../models/fileArchive.model';
import { FileVersion } from '../../models/fileVersion.model';
import { FileArchiveService } from '../../services/fileUpload.service';
import { FileVersionService } from '../../services/fileVersion.service';
import { DeleteDialog } from '../dialogs/delete-dialog/delete-dialog';
import { AuthService } from '../../services/auth.service';
import { MatDialog } from '@angular/material/dialog';
import { QueryParams } from '../../models/apiResponse';
import { NotificationService } from '../../services/notification.service';
import { FileVersionDialog } from '../dialogs/file-version-dialog/file-version-dialog';
import { FileSummaryService } from '../../services/fileSummary.service';
import { FileSummaryDialog } from '../dialogs/file-summary-dialog/file-summary-dialog';

@Component({
  selector: 'app-file-list',
  imports: [DataTable],
  templateUrl: './file-list.html',
  styleUrl: './file-list.css'
})
export class FileList {

  FileArchives: FileArchive[] = [];
  fileVersions: FileVersion[] = [];
  isAdmin: boolean = false;
  searchLoading: boolean = false;

  totalCount: number = 0;
  pageSize: number = 0;
  loading = false;
  currentQuery: QueryParams = { page: 1, pageSize: 10 };
  searchMode: boolean = false;

  columns: TableColumn[] = [
    { key: 'fileName', label: 'File Name', sortable: true },
    { key: 'categoryName', label: 'Category' },
    { key: 'uploadedByName', label: 'Uploaded By' },
    { key: 'status', label: 'Active', pipe: 'boolean' },
    { key: 'canSummarise', label: 'Summary', pipe: 'boolean' }
  ];

  actions: TableAction[] = [
    {
      label: 'Download Latest',
      icon: 'download',
      action: (fileArchive) => this.downloadFile(fileArchive.fileName),
      showIf: (fileArchive) => this.isAdmin && !fileArchive.status
    },
    {
      label: 'View Versions',
      icon: 'history',
      color: 'text-primary',
      action: (fileArchive) => this.getFileVersionsForArchive(fileArchive),
      showIf: (fileArchive) => this.isAdmin && !fileArchive.status
    },
    {
      label: 'Delete',
      icon: 'delete',
      color: 'text-danger',
      action: (fileArchive) => this.openDeleteDialog(fileArchive),
      showIf: (fileArchive) => this.isAdmin && !fileArchive.status
    }
  ]



  constructor(
    private fileArchiveService: FileArchiveService,
    private fileVersionService: FileVersionService,
    private fileSummaryService: FileSummaryService,
    private notificationService: NotificationService,
    private authService: AuthService,
    private dialog: MatDialog
  ) { }

  ngOnInit() {
    this.isAdmin = this.authService.isAdmin();
    this.loadFileArchives();

  }
  onQueryChanged($event: QueryParams) {
    console.log($event)
    if (!this.searchMode) {
      this.currentQuery = { ...this.currentQuery, ...$event };
      this.loadFileArchives();
    }
    else {
      this.searchLoading = true;
      this.fileSummaryService.semanticSearch($event.search).subscribe({
        next: (response) => {
          console.log(response);
          this.openFileSummaryDialog(response);

          this.searchLoading = false;
        }
      })
    }
  }

  onSearchModeToggle(searchMode: any) {
    console.log(searchMode)
    this.searchMode = searchMode;
  }

  openFileSummaryDialog(data: any) {
    this.dialog.open(FileSummaryDialog, {
      width: '800px',
      data: {
        summary: { summary: data.fileSummary },
        file: { fileName: data.fileName },
        score: data.confidenceScore,
        isSearch: this.searchMode
      }
    })
  }

  loadFileArchives() {
    this.loading = true;
    this.fileArchiveService.getAll(this.currentQuery).subscribe({
      next: (response: any) => {
        this.FileArchives = response.data.data.$values;
        this.totalCount = response.data.totalRecords;
        this.loading = false;
        console.log(this.FileArchives)
      },
      error: (e) => {
        this.notificationService.showError(e.error.errors.fields);
      }
    })

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
  openFileVersionDialog() {
    this.dialog.open(FileVersionDialog, {
      data: { versions: this.fileVersions }
    })
  }

  getFileVersionsForArchive(fileArchive: FileArchive) {
    this.loading = true;
    this.fileVersionService.getByArchiveId(fileArchive.id).subscribe({
      next: (response: any) => {
        this.fileVersions = response.data.$values;
        this.loading = false;
        if (this.fileVersions.length > 0) {
          this.openFileVersionDialog();
        } else {
          this.notificationService.showInfo('No versions available for this file.');
        }
      },
      error: (err) => {
        this.notificationService.showError(err.errors.error.fields);
        this.loading = false;
      }
    });
  }



  openDeleteDialog(fileArchive: FileArchive) {
    const dialogRef = this.dialog.open(DeleteDialog, {
      width: '700px',
      data: fileArchive.fileName
    });

    dialogRef.afterClosed().subscribe(res => {
      if (res) {
        this.deleteFile(fileArchive)
      }
    })
  }

  deleteFile(fileArchive: FileArchive) {
    this.fileArchiveService.delete(fileArchive.id).subscribe({
      next: () => {
        this.notificationService.showSuccess('File Archive Deleted Successfully');
      },
      error: (e) => {
        this.notificationService.showError(e.error.errors.fields)
      }
    })
  }
}
