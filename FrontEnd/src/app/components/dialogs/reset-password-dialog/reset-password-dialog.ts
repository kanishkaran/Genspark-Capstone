import { Component } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { passwordStrengthValidator } from '../../../validators/custom-validators';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-reset-password-dialog',
  imports: [ReactiveFormsModule, FormsModule, MatFormFieldModule, MatInputModule],
  templateUrl: './reset-password-dialog.html',
  styleUrl: './reset-password-dialog.css'
})
export class ResetPasswordDialog {
  

  passwordForm: FormGroup;

  constructor(private dialogRef: MatDialogRef<ResetPasswordDialog>) {
    this.passwordForm = new FormGroup({
      oldPassword: new FormControl(null, [Validators.required, Validators.minLength(3)]),
      newPassword: new FormControl(null, [Validators.required, passwordStrengthValidator()])
    });
  }


  public get oldPass(): any {
    return this.passwordForm.get('oldPassword');
  }


  public get newPass(): any {
    return this.passwordForm.get('newPassword');
  }

  Cancel() {
    this.dialogRef.close(null)
  }

  submitForm() {
    if(this.passwordForm.valid){
      this.dialogRef.close(this.passwordForm.value)
    }
  }


}
