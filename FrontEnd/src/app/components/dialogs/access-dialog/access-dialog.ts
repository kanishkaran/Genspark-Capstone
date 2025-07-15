import { Component } from '@angular/core';
import { AccessCreateRequest } from '../../../models/access.model';
import { FormControl, FormGroup, FormsModule, MinLengthValidator, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-access-dialog',
  imports: [FormsModule, MatInputModule, MatFormFieldModule, ReactiveFormsModule],
  templateUrl: './access-dialog.html',
  styleUrl: './access-dialog.css'
})
export class AccessDialog {

  accessForm: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<AccessDialog>,
  ) {
    this.accessForm = new FormGroup({
      access: new FormControl(null, [Validators.required, Validators.minLength(3)])
    })
  }

  
  public get access() : any {
    return this.accessForm.get('access');
  }
  
  onFormSubmit(): void {
    if (this.accessForm.valid) {
      this.dialogRef.close(this.accessForm.value);
    }
  }

  cancelForm(): void {
    this.dialogRef.close(null);
  }


}
