import { Component } from '@angular/core';

import { Role } from '../../models/roleModel';
import { QueryParams } from '../../models/apiResponse';
import { RoleService } from '../../services/role.service';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { AccessLevel } from '../../models/access.model';
import { AccessLevelService } from '../../services/access.service';
import { MatInputModule } from '@angular/material/input';
import { DataTable, TableAction, TableColumn } from '../../shared/data-table/data-table';
import { RoleDialog } from '../dialogs/role-dialog/role-dialog';
import { MatDialog } from '@angular/material/dialog';
import { DeleteDialog } from '../dialogs/delete-dialog/delete-dialog';

@Component({
  selector: 'app-role-list',
  imports: [DataTable,
    MatFormFieldModule,
    FormsModule,
    MatSelectModule, MatInputModule
  ],
  templateUrl: './role-list.html',
  styleUrl: './role-list.css'
})
export class RoleList {


  roles: Role[] = [];
  accessLevels: AccessLevel[] = [];
  totalCount: number = 0;
  pageSize: number = 0;
  loading = false;
  currentQuery: QueryParams = { page: 1, pageSize: 10 }
  isAdmin: boolean = false



  columns: TableColumn[] = [
    { key: 'roleName', label: 'Role Name', sortable: true },
    { key: 'accessLevel', label: 'Access Level', sortable: true },
    { key: 'isDeleted', label: 'Deleted', pipe: 'boolean' }
  ];

  actions: TableAction[] = [
    {
      label: 'Edit',
      icon: 'edit',
      color: 'text-info',
      action: (role) => this.openRoleDialog(true, role),
      showIf: () => this.isAdmin
    }
    ,
    {
      label: 'Delete',
      icon: 'delete',
      color: 'text-danger',
      action: (role) => this.openDeleteDialog(role),
      showIf: () => this.isAdmin
    }
  ]

  constructor(
    private roleService: RoleService,
    public authService: AuthService,
    private notificationService: NotificationService,
    private accessLevelService: AccessLevelService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();
    this.loadAccessLevels();
    this.loadRoles();
  }


  loadRoles() {
    this.loading = true;
    console.log(this.currentQuery)
    this.roleService.getAll(this.currentQuery).subscribe({
      next: (response: any) => {
        console.log(response.data.data.$values)
        this.roles = response.data.data.$values;
        this.totalCount = response.data.totalRecords;
        this.loading = false;
      }
    })
  }

  loadAccessLevels() {
    this.loading = true;
    this.accessLevelService.getAll().subscribe({
      next: (response: any) => {
        this.accessLevels = response.data.data.$values;
        console.log(this.accessLevels)
        this.loading = false;
      }
    })

  }
  onQueryChanged($event: QueryParams) {
    console.log($event)
    this.currentQuery = { ...this.currentQuery, ...$event };
    this.loadRoles();
  }

  openDeleteDialog(role: Role) {
    const dialogRef = this.dialog.open(DeleteDialog, {
      width: '700px',
      data: role.roleName
    });

    dialogRef.afterClosed().subscribe(res => {
      if (res) {
        this.deleteRole(role)
      }
    })
  }


  deleteRole(role: any): void {

    this.roleService.delete(role.id).subscribe({
      next: (data) => {
        this.notificationService.showSuccess(data.message);
        this.loadRoles();
      },
      error: (err) => {
        console.log(err)
        this.notificationService.showError(`Failed to delete category: ${err.message}`);
      }
    })

  }



  openRoleDialog(isEditMode = false, existingData: any = null) {
    const dialogRef = this.dialog.open(RoleDialog, {
      width: '600px',
      data: {
        formModel: existingData || { roleName: '', accessLevel: '' },
        accessLevels: this.accessLevels,
        isEditMode
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (isEditMode) {
          this.updateRole(result);
        } else {
          this.createRole(result);
        }
      }
    });


  }
  createRole(result: any) {
    console.log(result)
    const payload = {
      role: result.roleName,
      accessLevel: result.accessLevel
    }
    this.roleService.create(payload).subscribe({
      next: () => {
        this.notificationService.showSuccess('Role added successfully');
        this.loadRoles();
      },
      error: (err) => this.notificationService.showError(`Failed to add role ${err.message}`)
    });
  }
  updateRole(result: any) {
    const payload = {
      role: result.roleName,
      accessLevel: result.accessLevel
    }
    this.roleService.update(result.id, payload).subscribe({
      next: () => {
        this.notificationService.showSuccess('Role Updated Successfully');
        this.loadRoles();
      },
      error: (err) => this.notificationService.showError(`Failed to update role ${err.message}`)
    })
  }


}
