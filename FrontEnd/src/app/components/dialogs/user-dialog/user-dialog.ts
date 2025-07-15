import { Component, Inject } from '@angular/core';
import { UserCreateRequest } from '../../../models/user.model';
import { Role } from '../../../models/roleModel';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-user-dialog',
  imports: [FormsModule, MatFormFieldModule, MatSelectModule, MatInputModule],
  templateUrl: './user-dialog.html',
  styleUrl: './user-dialog.css'
})
export class UserDialog {


  isEditMode: boolean = false;
  createFormModel: UserCreateRequest = { username: '', password: '', roleName: '' }
  updateFormModel = { username: '', roleName: '' }
  roles: Role[] = [];

  constructor(
    public dialogRef: MatDialogRef<UserDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    if (data) {
      this.roles = data.roles;
      this.isEditMode = data.isEditMode;

      if (this.isEditMode) {
        this.updateFormModel = { ...data.formModel }
      }
      else {
        this.createFormModel = { ...data.formModel }
      }
    }
  }

  onFormSubmit() {
    if(this.isEditMode){
      this.dialogRef.close(this.updateFormModel);
    }
    else{
      this.createFormModel.isAdmin = this.createFormModel.roleName == "Admin";
      console.log(this.createFormModel)
      this.dialogRef.close(this.createFormModel);
    }
  }

  cancelForm(): void {
    this.dialogRef.close(null);
  }

}
