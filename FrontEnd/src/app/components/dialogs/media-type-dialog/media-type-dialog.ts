import { Component, Inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MediaTypeCreateRequest } from '../../../models/mediaType.model';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-media-type-dialog',
  imports: [MatFormFieldModule,
    FormsModule,
    MatSelectModule,
    MatInputModule],
  templateUrl: './media-type-dialog.html',
  styleUrl: './media-type-dialog.css'
})
export class MediaTypeDialog {
  formModel: MediaTypeCreateRequest = { typeName: '', extension: '' }
  isEditMode: boolean = false;

  constructor(
    public dialogRef: MatDialogRef<MediaTypeDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {

    if (data) {
      this.formModel = { ...data.formModel };
      this.isEditMode = data.isEditMode || false;
    }

  }

  onFormSubmit(): void {
    if (this.formModel.typeName && this.formModel.extension) {
      this.dialogRef.close(this.formModel);
    }
  }

  cancelForm(): void {
    this.dialogRef.close(null);
  }


}
