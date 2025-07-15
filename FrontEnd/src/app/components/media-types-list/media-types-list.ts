import { Component, OnInit } from '@angular/core';
import { MediaType, MediaTypeCreateRequest } from '../../models/mediaType.model';
import { QueryParams } from '../../models/apiResponse';
import { TableAction, TableColumn, DataTable } from '../../shared/data-table/data-table';
import { MediaTypeService } from '../../services/mediaType.service';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MediaTypeDialog } from '../dialogs/media-type-dialog/media-type-dialog';
import { MatDialog } from '@angular/material/dialog';
import { DeleteDialog } from '../dialogs/delete-dialog/delete-dialog';

@Component({
  selector: 'app-media-types-list',
  imports: [DataTable,
    CommonModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule
  ],
  templateUrl: './media-types-list.html',
  styleUrl: './media-types-list.css'
})
export class MediaTypesList implements OnInit {

  mediaTypes: MediaType[] = []
  totalCount: number = 0;
  pageSize: number = 0;
  loading: boolean = false;
  currentQuery: QueryParams = { page: 1, pageSize: 10 }
  showForm: boolean = false;
  isEditMode: boolean = false;

  isAdmin: boolean = false;

  columns: TableColumn[] = []

  actions: TableAction[] = [
    {
      label: 'Edit',
      icon: 'edit',
      action: (mediaType) => this.openMediaDialog(true, mediaType),
      showIf: () => this.isAdmin
    }
    ,
    {
      label: 'Delete',
      icon: 'delete',
      color: 'text-danger',
      action: (mediaType) => this.openDeleteDialog(mediaType),
      showIf: () => this.isAdmin
    }
  ]
  editTypeId: string | null = null;

  constructor(
    private mediaTypeService: MediaTypeService,
    private authService: AuthService,
    private notificationService: NotificationService,
    private dialog: MatDialog
  ) { }




  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();

    this.columns = [
      { key: 'typeName', label: 'Content Type', sortable: true },
      { key: 'extension', label: 'Extension', sortable: true }
    ];

    if (this.isAdmin) {
      this.columns.push({ key: 'isDeleted', label: 'Deleted?', pipe: 'boolean' });
    }

    this.loadMediaTypes();
  }


  loadMediaTypes() {
    this.loading = true
    this.mediaTypeService.getAll(this.currentQuery).subscribe({
      next: (response: any) => {
        this.mediaTypes = response.data.data.$values;
        console.log(this.mediaTypes);
        this.totalCount = response.data.totalRecords;
        this.loading = false
      }
    })
  }


  onQueryChanged($event: QueryParams) {
    this.currentQuery = {
      ...this.currentQuery, ...$event
    }
    this.loadMediaTypes();
  }

  openDeleteDialog(mediaType: MediaType): void {

    const dialogRef = this.dialog.open(DeleteDialog, {
      width: '600px',
      data: mediaType.TypeName
    })

    dialogRef.afterClosed().subscribe(res => {
      if (res) {
        this.deleteMediaType(mediaType);
      }
    })

  }

  deleteMediaType(mediaType: MediaType): void {

    this.mediaTypeService.delete(mediaType.id).subscribe({
      next: (response) => {
        this.notificationService.showSuccess("Media Type has been deleted" + response.message);
        this.loadMediaTypes();
      },
      error: (err) => {
        console.log(err)
        this.notificationService.showError("Failed to delete media type" + err.message)
      }
    })

  }

  openMediaDialog(isEditMode = false, existingData: any = null) {
    const dialogRef = this.dialog.open(MediaTypeDialog, {
      width: '600px',
      data: {
        formModel: existingData || { typeName: '', extension: '' },
        isEditMode
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (isEditMode) {
          this.updateMediaType(result);
        } else {
          this.createMediaType(result);
        }
      }
    });
  }
  createMediaType(result: any) {
    this.mediaTypeService.create(result).subscribe({
      next: () => {
        this.notificationService.showSuccess('Role added successfully');
        this.loadMediaTypes();
      },
      error: (err) => this.notificationService.showError(`Failed to add Media Type ${err.message}`)
    });
  }
  updateMediaType(result: any) {
    const payload = {
      typeName: result.typeName,
      extension: result.extension
    }

    this.mediaTypeService.update(result.id, payload).subscribe({
      next: (response: any) => {
        this.notificationService.showSuccess("Updated" + response.message);
        this.loadMediaTypes();
      },
      error: (err) => this.notificationService.showError(`Failed to update Media Type ${err.message}`)
    })
  }

}
