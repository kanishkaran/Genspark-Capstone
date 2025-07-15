import { Component, Inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { Role } from '../../../models/roleModel';
import { Category } from '../../../models/categoryModel';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-role-category-dialog',
  imports: [FormsModule, MatInputModule, MatFormFieldModule, MatSlideToggleModule, MatSelectModule],
  templateUrl: './role-category-dialog.html',
  styleUrl: './role-category-dialog.css'
})
export class RoleCategoryDialog {
  isEditMode: boolean = false;
  formModel = { roleName: '', categoryName: '', canUpload: false, canDownload: false };
  roles: Role[] = [];
  categories: Category[] = [];

  constructor(
    public dialogRef: MatDialogRef<RoleCategoryDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    if (data) {
      this.formModel = { ...data.formModel };
      this.roles = data.roles || [];
      this.categories = data.categories || [];
      this.isEditMode = data.isEditMode || false;
    }
  }

  onFormSubmit(): void {
    if (this.formModel.roleName && this.formModel.categoryName) {
      this.dialogRef.close(this.formModel);
    }
  }

  cancelForm(): void {
    this.dialogRef.close(null);
  }


}
