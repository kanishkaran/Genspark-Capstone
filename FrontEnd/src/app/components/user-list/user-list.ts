import { Component, OnInit } from '@angular/core';
import { User } from '../../models/user.model';
import { QueryParams } from '../../models/apiResponse';
import { DataTable, TableAction, TableColumn } from '../../shared/data-table/data-table';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';
import { NotificationService } from '../../services/notification.service';
import { MatDialog } from '@angular/material/dialog';
import { RoleService } from '../../services/role.service';
import { Role } from '../../models/roleModel';
import { UserDialog } from '../dialogs/user-dialog/user-dialog';
import { DeleteDialog } from '../dialogs/delete-dialog/delete-dialog';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-list',
  imports: [DataTable],
  templateUrl: './user-list.html',
  styleUrl: './user-list.css'
})
export class UserList implements OnInit {
  users: User[] = [];
  roles: Role[] = [];
  loading: boolean = false;
  page: number = 0;
  pageSize: number = 10;
  currentQuery: QueryParams = { page: 1, pageSize: 10 };
  total: number = 0;

  isAdmin: boolean = false;


  columns: TableColumn[] = [
    { key: 'username', label: 'Username', sortable: true},
    { key: 'roleName', label: 'Role', sortable: true },
    { key: 'isDeleted', label: 'Active', pipe: 'boolean' }
  ]

  actions: TableAction[] = [
    {
      label: 'Change Role',
      icon: 'build',
      action: (user) => this.openUserDialog(true, user),
      showIf: () => this.isAdmin
    },
    {
      label: 'Disable',
      icon: 'delete',
      action: (user) => this.openDeleteDialog(user),
      showIf: () => this.isAdmin
    },
    {
      label: 'View More',
      icon: 'people',
      action: (user) => this.openProfile(user),
      showIf: () => this.isAdmin
    }
  ]

  constructor(private authService: AuthService,
    private userService: UserService,
    private notificationService: NotificationService,
    private roleService: RoleService,
    private dialog: MatDialog,
    private router: Router
  ) {

  }
  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();
    this.loadRoles();
    this.loadUsers()
  }
  loadRoles() {
    this.loading = true;

    this.roleService.getAll({ IncludeInactive: false }).subscribe({
      next: (response: any) => {
        this.roles = response.data.data.$values;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notificationService.showError('Failed to load roles');
      }
    })
  }

  loadUsers() {
    this.loading = true;

    this.userService.getAll(this.currentQuery).subscribe({
      next: (response: any) => {
        this.users = response.data.data.$values;
        this.users = this.users.filter(u => u.roleName !== "Admin")
        this.total = response.data.totalRecords;
        this.loading = false
      },
      error: () => {
        this.loading = false;
        this.notificationService.showError('Cannot Load Users')
      }
    })
  }
  onQueryChanged(params: Partial<QueryParams>): void {
    this.currentQuery = { ...this.currentQuery, ...params };
    this.loadUsers();
  }

  openProfile(user: User){
    this.router.navigate(['home', 'admin', 'users', user.username]);
  }


  openUserDialog(isEditMode = false, existingData: any = null): void {
    var dialogRef = this.dialog.open(UserDialog, {
      width: '600px',
      data: {
        formModel: existingData || { username: '', password: '', roleName: '' },
        roles: this.roles,
        isEditMode
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (isEditMode) {
          this.updateUserRole(result);
        } else {
          this.createUser(result);
        }
      }
    });
  }
  createUser(result : any){
    console.log(result);
    this.userService.create(result).subscribe({
      next: () => {
        this.notificationService.showSuccess("User has been successfully added");
        this.loadUsers();
      },
      error: (err) =>{
        this.notificationService.showWarning("Failed to add user" + err.message)
      }
    })
  }
  updateUserRole(result : any){
   
    const payload = {
      username: result.username,
      role: result.roleName
    }
    this.userService.updateUserRole(payload).subscribe({
      next: () => {
        this.notificationService.showSuccess("User Role has been updated to "+ result.roleName);
        this.loadUsers();
      },
      error: () => {
        this.notificationService.showError("Failed to update the role of the user")
      }
    })
  }

  openDeleteDialog(user: User){
    var dialogRef = this.dialog.open( DeleteDialog, {
      width: '700px',
      data: `the user with username: ${user.username}`
    });

    dialogRef.afterClosed().subscribe( res => {
      if(res){
        this.deleteUser(user);
      }
    })
  }


  deleteUser(user: User): void {
   console.log(user);

   this.userService.delete(user.username).subscribe({
    next: () => {
      this.notificationService.showSuccess('Deleted User Successfully');
      this.loadUsers();
    },
    error: (err) => {
      this.notificationService.showError(err.message);
    }
   })
  }

}
