import { Component, Inject } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { bannedWordsValidator, textLengthValidator } from '../../../validators/custom-validators';
import {  MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-update-profile-dialog',
  imports: [FormsModule, ReactiveFormsModule, MatInputModule, MatFormFieldModule],
  templateUrl: './update-profile-dialog.html',
  styleUrl: './update-profile-dialog.css'
})
export class UpdateProfileDialog {

  userForm: FormGroup;

  constructor(private dialogRef: MatDialogRef<UpdateProfileDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.userForm = new FormGroup({
      firstName: new FormControl(null, [Validators.required, textLengthValidator(), bannedWordsValidator(['test', 'user', 'admin'])]),
      lastName: new FormControl(null, [Validators.required, textLengthValidator()]),
      contactNumber: new FormControl(null, [Validators.required, Validators.minLength(10)]),
      email: new FormControl(null, [Validators.required, Validators.email])
    })

    this.userForm.setValue(data);
  }


  public get FirstName(): any {
    return this.userForm.get('firstName');
  }


  public get LastName(): any {
    return this.userForm.get('lastName');
  }


  public get ContactNumber(): any {
    return this.userForm.get('contactNumber');
  }


  public get Email(): any {
    return this.userForm.get('email');
  }

  Submit() {
    if(this.userForm.valid){
      this.dialogRef.close(this.userForm.value)
    }
  }
  Cancel() {
    this.dialogRef.close(null);
  }




}


