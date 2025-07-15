import { Component, Inject } from '@angular/core';
import { FileUpload } from "../../../shared/file-upload/file-upload";
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-upload-dialog',
  imports: [FileUpload],
  templateUrl: './upload-dialog.html',
  styleUrl: './upload-dialog.css'
})
export class UploadDialog {

  constructor(
    public dialogRef: MatDialogRef<UploadDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
  }

  Success($event: any) {
    console.log('success', $event)
    this.dialogRef.close('success')
  }

}
