import { Component, OnInit } from '@angular/core';

import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCheckboxModule } from '@angular/material/checkbox';

import { MatSelectModule } from '@angular/material/select';
import { DataTable, TableAction, TableColumn } from '../../shared/data-table/data-table';
import { CreateRoleCategoryRequest, RoleCategory } from '../../models/roleCategoryModel';
import { QueryParams } from '../../models/apiResponse';
import { Role } from '../../models/roleModel';
import { Category } from '../../models/categoryModel';
import { RoleCategoryService } from '../../services/roleCategory.service';
import { NotificationService } from '../../services/notification.service';
import { AuthService } from '../../services/auth.service';
import { RoleService } from '../../services/role.service';
import { CategoryService } from '../../services/category.service';
import { MatDialog } from '@angular/material/dialog';
import { RoleCategoryDialog } from '../dialogs/role-category-dialog/role-category-dialog';
import { DeleteDialog } from '../dialogs/delete-dialog/delete-dialog';



@Component({
  selector: 'app-role-category-list',
  imports: [DataTable, MatFormFieldModule, CommonModule, FormsModule, MatCheckboxModule, MatSelectModule],
  templateUrl: './role-category-list.html',
  styleUrl: './role-category-list.css'
})
export class RoleCategoryList implements OnInit {
  
  
  roleCategories: RoleCategory[] = [];
  totalCount: number = 0;
  pageSize = 10;
  isAdmin: boolean = false;
  currentQuery: QueryParams = { page: 1, pageSize: 10 }
  loading: boolean = false;
  showForm: boolean = false;
  editRoleCategoryId: string | null = null;
  
  roles: Role[] = [];
  categories: Category[] = [];
  
  actions: TableAction[] = [
    {
      label: 'Edit',
      icon: 'edit',
      color: 'text-primary',
      action: (roleCategory) => this.openRoleCategoryDialog(true, roleCategory),
      showIf: () => this.isAdmin
    },
    {
      label: 'Delete',
      icon: 'delete',
      color: 'text-danger',
      action: (roleCategory : RoleCategory) => this.openDeleteDialog(roleCategory),
      showIf: () => this.isAdmin
    }
  ]
  
  columns: TableColumn[] = [
    { key: 'roleName', label: 'Role Name', sortable: true },
    { key: 'categoryName', label: 'Category Name', sortable: true },
    { key: 'canUpload', label: 'Can Upload',  pipe: 'boolean' },
    { key: 'canDownload', label: 'Can Download ', pipe: 'boolean' }
    
  ]
  
  
  
  constructor(
    private roleCategoryService: RoleCategoryService,
    protected notificationService: NotificationService,
    private authService: AuthService,
    private roleService: RoleService,
    private categoryService: CategoryService,
    private dialog: MatDialog
  ) {
    this.isAdmin = this.authService.isAdmin();
    console.log(this.isAdmin)
  }
  
  ngOnInit(): void {
    
    this.loadRolesAndCategories();
    this.loadRoleCategories();
    
  }
  loadRolesAndCategories() {
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
    
    this.categoryService.getAll({ IncludeInactive: false }).subscribe({
      next: (response: any) => {
        this.categories = response.data.data.$values;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notificationService.showError('Failed to load roles');
      }
    })
    
  }
  
  loadRoleCategories() {
    this.loading = true;
    
    this.roleCategoryService.getAll(this.currentQuery).subscribe({
      next: (response: any) => {
        this.roleCategories = response.data.data.$values;
        this.totalCount = response.data.totalRecords;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notificationService.showError('Failed to load role category Accesss');
      }
    })
  }
  
  onQueryChanged(params: Partial<QueryParams>): void {
    this.currentQuery = { ...this.currentQuery, ...params };
    this.loadRoleCategories();
  }
  
  
  openDeleteDialog(roleCategory: RoleCategory): void {
     const dialogRef = this.dialog.open(DeleteDialog, {
          width: '600px',
          data: `${roleCategory.roleName}-${roleCategory.categoryName} Access`
        })
    
        dialogRef.afterClosed().subscribe(res => {
          if (res) {
            this.deleteRoleCategory(roleCategory);
          }
        })
  }
  
  deleteRoleCategory(roleCategory: any): void {
    if (confirm('Are you sure you want to delete this role-category access?')) {
      this.roleCategoryService.delete(roleCategory.id).subscribe({
        next: (data) => {
          this.notificationService.showSuccess(data.message);
          this.loadRoleCategories();
        },
        error: (err) => {
          console.log(err)
          this.notificationService.showError(`Failed to delete the role-category access: ${err.message}`);
        }
      })
    }
  }

  openRoleCategoryDialog(isEditMode = false, existingData: any = null) {
    const dialogRef = this.dialog.open(RoleCategoryDialog, {
      width: '700px',
      data: {
        formModel: existingData || { role: '', category: '', canUpload: false, canDownload: false },
        roles: this.roles,
        categories: this.categories,
        isEditMode
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (isEditMode) {
          this.updateRoleCategory(result);
        } else {
          this.createRoleCategory(result);
        }
      }
    });
  }
  createRoleCategory(result: any) {
    const payload: CreateRoleCategoryRequest = {
      role : result.roleName,
      category: result.categoryName,
      canDownload: result.canDownload,
      canUpload: result.canUpload
    }
    this.roleCategoryService.create(payload).subscribe({
      next: (res) => {
        this.notificationService.showSuccess('Role-Category Access added successfully');
        this.loadRoleCategories();
      },
      error: (err) => {
        console.log(err)
        this.notificationService.showError(`Failed to add: ${err.error.errors.fields}`);
      }
    });
  }
  updateRoleCategory(result: any) {
    console.log(result)
    const payload = {
      role: result.role,
      category: result.category,
      canUpload: result.canUpload,
      canDownload: result.canDownload
    };
    this.roleCategoryService.update(result.id, payload).subscribe({
      next: (res) => {
        this.notificationService.showSuccess('Role-Category Access updated successfully');
        this.loadRoleCategories();
      },
      error: (err) => {
        this.notificationService.showError(`Failed to update: ${err.message}`);
      }
    });
  }


}



