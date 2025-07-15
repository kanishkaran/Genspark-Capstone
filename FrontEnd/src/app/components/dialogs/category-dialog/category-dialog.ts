import { Component, Inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-category-dialog',
  imports: [MatFormFieldModule,
    FormsModule,
    MatSelectModule,
    MatInputModule],
  templateUrl: './category-dialog.html',
  styleUrl: './category-dialog.css'
})
export class CategoryDialog {

  formModel: { categoryName: string; accessLevel: string } = { categoryName: '', accessLevel: '' };
  accessLevels: any[] = [];
  isEditMode: boolean = false;

  constructor(
    public dialogRef: MatDialogRef<CategoryDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    if (data) {
      this.formModel = { ...data.formModel };
      this.accessLevels = data.accessLevels || [];
      this.isEditMode = data.isEditMode || false;
    }
  }

  onFormSubmit(): void {
    if (this.formModel.categoryName && this.formModel.accessLevel) {
      this.dialogRef.close(this.formModel);
    }
  }

  cancelForm(): void {
    this.dialogRef.close(null);
  }

}
