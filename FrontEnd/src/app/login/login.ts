import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { bannedWordsValidator, passwordStrengthValidator, textLengthValidator } from '../validators/custom-validators';
import { LucideAngularModule, LucideFolderUp } from 'lucide-angular';
import { BehaviorSubject } from 'rxjs';


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ FormsModule, ReactiveFormsModule, LucideAngularModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  loginForm: FormGroup;
  userForm: FormGroup;
  isSignup = false;
  loading = false;
  error: string | null = null;
  readonly FolderUpIcon = LucideFolderUp;


  private authService = inject(AuthService);
  private router = inject(Router); 

  bannedWords = ['admin', 'null', 'test', 'banned'];

  constructor() {
    this.userForm = new FormGroup({
      firstname: new FormControl(null, [Validators.required, bannedWordsValidator(this.bannedWords), textLengthValidator()]),
      lastname: new FormControl(null, [Validators.required, bannedWordsValidator(this.bannedWords), textLengthValidator()]),
      email: new FormControl(null, [Validators.required, Validators.email, bannedWordsValidator(this.bannedWords)]),
      password: new FormControl(null, [Validators.required, passwordStrengthValidator()]),
      contactNumber: new FormControl(null, [Validators.required, Validators.pattern(/^[0-9]{10}$/)])
    });

    this.loginForm = new FormGroup({
      email: new FormControl(null, [Validators.required, Validators.email]),
      password: new FormControl(null, [Validators.required])
    });
  }

  get reg_email(): any { return this.userForm.get('email'); }
  get reg_password(): any { return this.userForm.get('password'); }
  get firstname(): any { return this.userForm.get('firstname'); }
  get lastname(): any { return this.userForm.get('lastname'); }
  get contact(): any { return this.userForm.get('contactNumber'); }
  get login_email(): any { return this.loginForm.get('email'); }
  get login_password(): any { return this.loginForm.get('password'); }

  onSignupSubmit() {
    if (this.userForm.valid) {
      console.log('Signup form submitted:', this.userForm.value);

      this.loading = true;
      this.error = null;
      this.authService.registerEmployee(this.userForm.value).subscribe({
        next: (data) => {
          console.log(data.message);

          this.isSignup = false;
        },
        error: (err) => {
          this.error = err?.error?.errors?.fields || 'Invalid credentials';
          this.loading = false;
        },
        complete: () => {
          this.loading = false;
        }
      });
    } else {
      this.error = 'Please complete the signup form correctly.';
    }
  }

  onLoginSubmit() {
    if (this.loginForm.invalid) {
      this.error = 'Please complete all required login fields.';
      return;
    }

    this.loading = true;
    this.error = null;

    const { email, password } = this.loginForm.value;

    this.authService.login(email, password).subscribe({
      next: (res) => {
        console.log(res)
 
        this.router.navigate(['/home']); 
      },
      error: (err) => {
        this.error = err?.error?.errors?.fields || 'Invalid credentials';
        console.log(err)
        this.loading = false;
      },
      complete: () => {
        this.loading = false;
      }
    });
  }
}
