import { Component, OnInit } from '@angular/core';
import { of } from 'rxjs';
import { switchMap, map } from 'rxjs/operators';
import { CategoryService } from '../../services/category.service';
import { RoleCategoryService } from '../../services/roleCategory.service';
import { FileArchiveService } from '../../services/fileUpload.service';
import { AuthService } from '../../services/auth.service';
import { FileArchive } from '../../models/fileArchive.model';
import { Category } from '../../models/categoryModel';
import { RoleCategory } from '../../models/roleCategoryModel';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../services/notification.service';
import { MatDialog } from '@angular/material/dialog';
import { UploadDialog } from '../dialogs/upload-dialog/upload-dialog';

@Component({
  selector: 'app-user-category',
  imports: [MatIconModule, MatProgressSpinnerModule, CommonModule],
  templateUrl: './user-category.html',
  styleUrl: './user-category.css'
})
export class UserCategory implements OnInit {
  myAuthorisedCategories: any[] = [];
  selectedCategory: any;
  filesInCategory: FileArchive[] = [];
  files: FileArchive[] = [];
  loading = false;
  isAdmin = false;
  currentUser: any;

  constructor(
    private categoryService: CategoryService,
    private roleCategoryService: RoleCategoryService,
    private fileService: FileArchiveService,
    private authService: AuthService,
    private notificationService: NotificationService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.currentUser = this.authService.currUserSubject.value;
    this.isAdmin = this.currentUser.roleName === 'Admin';
    this.loadCategories();
    this.loadFiles();
  }

  loadCategories() {
    this.categoryService.getAll({ pageSize: 1000 }).pipe(
      switchMap((categoryRes: any) => {
        const categories: Category[] = categoryRes.data.data.$values;

        if (this.isAdmin) {
          return of(categories.map(cat => ({
            ...cat,
            canDownload: true,
            canUpload: true,
            isAdminOnly: cat.access === 'Admin'
          })));
        }
        else if(this.currentUser.roleName == "Viewer"){
          return of(categories.map(cat => ({
            ...cat,
            canDownload: cat.access === 'Admin' ? false : true,
            canUpload: false,
            isAdminOnly: cat.access === 'Admin'
          })));
        }

        return this.roleCategoryService.getAll({ pageSize: 100 }).pipe(
          map((roleCatRes: any) => {
            const roleCategories: RoleCategory[] = roleCatRes.data.data.$values;
            const roleName = this.currentUser.roleName;
            const roleMap = new Map<string, RoleCategory>();

            roleCategories
              .filter(rc => rc.roleName === roleName)
              .forEach(rc => roleMap.set(rc.categoryName, rc));

            return categories.map(cat => {
              const rc = roleMap.get(cat.categoryName);
              let canDownload = false;
              let canUpload = false;

              if (cat.access === 'read-only') {
                canDownload = true;
              } 
              if (rc) {
                if (cat.access === 'Write') {
                  canDownload = rc.canDownload;
                  canUpload = rc.canUpload;
                }
              }

              return {
                ...cat,
                canDownload,
                canUpload,
                isAdminOnly: cat.access === 'Admin'
              };
            });
          })
        );
      })
    ).subscribe({
      next: (result) => this.myAuthorisedCategories = result,
      error: (err) => console.error('Error fetching categories:', err)
    });
  }

  loadFiles() {
    this.fileService.getAll({ pageSize: 100 }).subscribe((res: any) => {
      this.files = res.data.data.$values as FileArchive[];
    })
  }

  openCategory(category: Category) {
    this.selectedCategory = category;
    console.log(this.selectedCategory)
    this.loading = true;
    this.filesInCategory = this.files.filter(f => f.categoryName === category.categoryName);
    this.loading = false;
    console.log(this.filesInCategory, "files", this.files)
  }

  downloadFile(fileName: string) {
    this.fileService.downloadFile(fileName).subscribe({
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
        this.notificationService.showSuccess(`downloaded successfully`);
      },
      error: () => {
        this.notificationService.showError('Failed to download file version');
      }
    });
  }
  upload(category: string){
    console.log(category)
    this.dialog.open(UploadDialog,{
      width: '800px',
      data: {categoryName: category}
    })
  }

  goBack() {
    this.selectedCategory = null;
    this.filesInCategory = [];
  }
}

