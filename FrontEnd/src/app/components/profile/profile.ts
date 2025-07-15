import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Employee, EmployeeUpdateRequest } from '../../models/employee.model';
import { AuthService } from '../../services/auth.service';
import { EmployeeService } from '../../services/employee.service';
import { NotificationService } from '../../services/notification.service';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialog } from '@angular/material/dialog';
import { ResetPasswordDialog } from '../dialogs/reset-password-dialog/reset-password-dialog';
import { UserPasswordChangeRequest } from '../../models/user.model';
import { UserService } from '../../services/user.service';
import { UpdateProfileDialog } from '../dialogs/update-profile-dialog/update-profile-dialog';
import { RegisterRequest } from '../../models/requestModel';

@Component({
  selector: 'app-profile',
  imports: [CommonModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatDividerModule,
    MatChipsModule],
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class Profile {


  router = inject(ActivatedRoute);
  userName: string = '';
  isAdmin: boolean = false;
  currentUser: any;
  roleName: string = ''
  employee: Employee = {};
  shouldCompleteProfile: boolean = false;

  constructor(
    private authService: AuthService,
    private employeeService: EmployeeService,
    private userService: UserService,
    private notificationService: NotificationService,
    private dialog: MatDialog
  ) {

    this.authService.currUser$.subscribe(res => {
      this.currentUser = res.username;
      this.roleName = res.roleName;
      console.log(this.currentUser)
    })
  }

  ngOnInit() {
    this.userName = this.router.snapshot.params['uname']
    this.isAdmin = this.authService.isAdmin();


    if (this.isAdmin || this.userName == this.currentUser) {
      this.loadEmployee();
    }
    else {
      this.notificationService.showWarning("Un Authorised Access")
      return;
    }
    console.log(this.userName, this.currentUser)

  }

  loadEmployee() {
    this.employeeService.getEmployeeByEmail(this.userName).subscribe({
      next: (res: any) => {
        console.log(res)
        this.employee = res.data;

        if(this.isAdmin){
          this.userService.getById(this.userName).subscribe((res : any) => {
          this.roleName = res.data.roleName;
        })
        }
        
        if (this.employee.email == '' || this.employee.firstName == '' || this.employee.lastName == '') {
          this.notificationService.showInfo('Profile Not Found. Profile Completion Needed')
          this.employee.isActive = true;
          this.employee.email = this.userName;
          this.shouldCompleteProfile = true;
        }

        console.log(this.employee)
      },
      error: (err) => {
        console.log(err)
      }
    })
  }

  getInitials(firstName?: string, lastName?: string): string {
    const first = firstName?.charAt(0)?.toUpperCase() || '';
    const last = lastName?.charAt(0)?.toUpperCase() || '';
    return first + last;
  }

  getFullName(): string {
    return `${this.employee.firstName || ''} ${this.employee.lastName || ''}`.trim();
  }


  openChangePasswordDialog(employee: Employee) {
    const dialogRef = this.dialog.open(ResetPasswordDialog, {
      width: '700px'
    })

    dialogRef.afterClosed().subscribe({
      next: (data) => {
        if (data) {
          this.changePassword(data, employee);
        }
      },
      error: (err) => {
        this.notificationService.showError(err.message)
      }
    })

  }


  changePassword(data: UserPasswordChangeRequest, user: Employee) {
    this.userService.changePassword(data, user.email).subscribe({
      next: () => {
        this.notificationService.showSuccess("Password Changed successfully");
      },
      error: (err) => {
        this.notificationService.showError(err.message)
      }
    })
  }
  openEditDialog(employee: Employee) {
    const dialogRef = this.dialog.open(UpdateProfileDialog, {
      width: '800px',
      data: {
        firstName: employee.firstName,
        lastName: employee.lastName,
        email: employee.email,
        contactNumber: employee.contactNumber
      }
    })

    dialogRef.afterClosed().subscribe({
      next: (data) => {
        if (data) {

          if (!this.shouldCompleteProfile) {
            this.updateProfile(data, employee)
          }
          else {
            this.completeProfile(data)
          }
        }
      }
    })
  }

  completeProfile(employee: RegisterRequest) {
    this.employeeService.create(employee).subscribe({
      next: () => {
        this.notificationService.showSuccess("Profile Complete Successful");
      },
      error: (err) => {
        this.notificationService.showError(err.message);
      }
    })
  }


  updateProfile(data: EmployeeUpdateRequest, employee: Employee) {
    this.employeeService.update(employee.id || '', data).subscribe({
      next: () => {
        this.notificationService.showSuccess("Profile Updated Successfully");
        this.loadEmployee();
      },
      error: (err) => {
        this.notificationService.showError(err.message);
      }
    })
  }
}
