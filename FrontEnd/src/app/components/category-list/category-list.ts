import { Component, Injectable, OnInit } from '@angular/core';
import { QueryParams } from '../../models/apiResponse';
import { Category } from '../../models/categoryModel';
import { AuthService } from '../../services/auth.service';
import { CategoryService } from '../../services/category.service';
import { NotificationService } from '../../services/notification.service';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { AccessLevelService } from '../../services/access.service';
import { AccessLevel } from '../../models/access.model';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { DataTable, TableAction, TableColumn } from '../../shared/data-table/data-table';
import { CategoryDialog } from '../dialogs/category-dialog/category-dialog';
import { MatDialog } from '@angular/material/dialog';
import { DeleteDialog } from '../dialogs/delete-dialog/delete-dialog';
import { MatIconModule } from '@angular/material/icon';


@Component({
  selector: 'app-category-list',
  imports: [DataTable, FormsModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatIconModule],
  templateUrl: './category-list.html',
  styleUrl: './category-list.css'
})


export class CategoryList implements OnInit {
  categories: Category[] = [];
  totalCount = 0;
  pageSize = 10;
  loading = false;
  currentQuery: QueryParams = { page: 1, pageSize: 10, IncludeInactive: false };
  isAdmin: boolean = false;

  columns: TableColumn[] = [];
  accessLevels: AccessLevel[] = [];

  actions: TableAction[] = [
    {
      label: 'Edit',
      icon: 'edit',
      color: 'text-primary',
      action: (category) => this.openCategoryDialog(true, category),
      showIf: () => this.isAdmin
    },
    {
      label: 'Delete',
      icon: 'delete',
      color: 'text-danger',
      action: (category) => this.openDeleteDialog(category),
      showIf: () => this.isAdmin
    }
  ];

  constructor(
    private categoryService: CategoryService,
    public authService: AuthService,
    private notificationService: NotificationService,
    private accessLevelService: AccessLevelService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();

    this.columns = [
      { key: 'categoryName', label: 'Category Name', sortable: true },
      { key: 'access', label: 'Access Level', sortable: true }
    ];

    if (this.isAdmin) {
      this.columns.push({ key: 'isDeleted', label: 'Deleted', pipe: 'boolean' });
    }
    this.loadAccessLevels()
    this.loadCategories();
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

  loadCategories(): void {
    this.loading = true;
    this.categoryService.getAll(this.currentQuery).subscribe({
      next: (response: any) => {
        console.log(response.data.data.$values);
        this.categories = response.data.data.$values;
        this.totalCount = response.data.totalRecords;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.notificationService.showError('Failed to load categories');
      }
    });
  }

  onQueryChanged(params: Partial<QueryParams>): void {

    this.currentQuery = { ...this.currentQuery, ...params };
    console.log(this.currentQuery)
    this.loadCategories();
  }

  openCategoryDialog(isEditMode = false, existingData: any = null) {
    const dialogRef = this.dialog.open(CategoryDialog, {
      width: '600px',
      data: {
        formModel: existingData || { categoryName: '', accessLevel: '' },
        accessLevels: this.accessLevels,
        isEditMode
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (isEditMode) {
          this.updateCategory(result);
        } else {
          this.createCategory(result);
        }
      }
    });
  }
  createCategory(result: any) {
    this.categoryService.create(result).subscribe({
      next: () => {
        this.notificationService.showSuccess('Category added successfully');
        this.loadCategories();
      },
      error: () => this.notificationService.showError('Failed to add category')
    });
  }

  updateCategory(result: any) {
    const payload = {
      categoryName: result.categoryName,
      accessLevel: result.accessLevel
    };
    this.categoryService.update(result.id, payload).subscribe({
      next: () => {
        this.notificationService.showSuccess('Category updated successfully');
        this.loadCategories();
      },
      error: (err) => this.notificationService.showError(`Failed to update category ${err.message}`)
    });
  }

  openDeleteDialog(category: Category) {
    const dialogRef = this.dialog.open(DeleteDialog, {
      width: '600px',
      data: category.categoryName
    })

    dialogRef.afterClosed().subscribe(res => {
      if (res) {
        this.deleteCategory(category);
      }
    })
  }


  deleteCategory(category: Category): void {

    this.categoryService.delete(category.id).subscribe({
      next: () => {
        this.notificationService.showSuccess('Category deleted successfully');
        this.loadCategories();
      },
      error: (err) => {
        console.log(err)
        this.notificationService.showError(`Failed to delete category: ${err.message}`);
      }
    });

  }
}