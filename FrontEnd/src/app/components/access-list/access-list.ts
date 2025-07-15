import { Component } from '@angular/core';
import { AccessLevel } from '../../models/access.model';
import { QueryParams } from '../../models/apiResponse';
import { DataTable, TableAction, TableColumn } from '../../shared/data-table/data-table';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';
import { AccessLevelService } from '../../services/access.service';
import { MatDialog } from '@angular/material/dialog';
import { AccessDialog } from '../dialogs/access-dialog/access-dialog';
import { DeleteDialog } from '../dialogs/delete-dialog/delete-dialog';

@Component({
  selector: 'app-access-list',
  imports: [DataTable],
  templateUrl: './access-list.html',
  styleUrl: './access-list.css'
})
export class AccessList {

  accessLevels: AccessLevel[] = [];
  totalCount = 0;
  pageSize = 10;
  loading = false;
  currentQuery: QueryParams = { page: 1, pageSize: 10, IncludeInactive: false };
  isAdmin: boolean = false;

  columns: TableColumn[] = [
    { key: 'access', label: 'Access Level', sortable: true },
    { key: 'isActive', label: 'Active', pipe: 'boolean' }
  ];

  actions: TableAction[] = [
    {
      label: 'Delete',
      icon: 'delete',
      color: 'text-danger',
      action: (access) => this.openDeleteDialog(access),
      showIf: (access) => this.isAdmin && access.isActive
    }
  ];

  constructor(
    public authService: AuthService,
    private notificationService: NotificationService,
    private accessLevelService: AccessLevelService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();
    this.loadAccessLevels()
  }

  loadAccessLevels() {
    this.loading = true;
    this.accessLevelService.getAll(this.currentQuery).subscribe({
      next: (response: any) => {
        this.accessLevels = response.data.data.$values;
        console.log(this.accessLevels)
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notificationService.showError('Failed to load accesses');
      }
    })

  }


  onQueryChanged(params: Partial<QueryParams>): void {

    this.currentQuery = { ...this.currentQuery, ...params };
    console.log(this.currentQuery)
    this.loadAccessLevels();
  }

  openAccessDialog() {
    const dialogRef = this.dialog.open(AccessDialog, {
      width: '600px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {

        this.createAccess(result);

      }
    });
  }
  createAccess(result: any) {
    this.accessLevelService.create(result).subscribe({
      next: () => {
        this.notificationService.showSuccess('Access Level Created Successfully');
        this.loadAccessLevels();
      },
      error: (err) => {
        this.notificationService.showError(err.errors.error.fields)
      }
    })
  }

  openDeleteDialog(access: AccessLevel): void {
    const dialogRef = this.dialog.open(DeleteDialog,{
      width: '600px',
      data: `${access.access}`
    });

    dialogRef.afterClosed().subscribe(res => {
      if(res) {
        this.deleteAccess(access);
      }
    })

  }

  deleteAccess(access: AccessLevel){
    this.accessLevelService.delete(access.id).subscribe({
      next: (res) => {
        this.notificationService.showSuccess(res.data);
        this.loadAccessLevels();
      },
      error: (err) => this.notificationService.showError(err.message)
    })
  }
}
