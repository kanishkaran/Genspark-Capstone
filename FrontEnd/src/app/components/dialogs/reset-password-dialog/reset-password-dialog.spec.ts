import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ResetPasswordDialog } from './reset-password-dialog';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

describe('ResetPasswordDialog', () => {
  let component: ResetPasswordDialog;
  let fixture: ComponentFixture<ResetPasswordDialog>;
  const mockDialogRef = {
    close: jasmine.createSpy('close')
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ResetPasswordDialog, FormsModule, ReactiveFormsModule],
      providers: [
        { provide: MatDialogRef, useValue: mockDialogRef }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ResetPasswordDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the dialog component with empty form', () => {
    expect(component).toBeTruthy();
    expect(component.passwordForm.valid).toBeFalse();
  });

  it('should close dialog with null when cancel is called', () => {
    component.Cancel();
    expect(mockDialogRef.close).toHaveBeenCalledWith(null);
  });

  it('should close dialog with form data on valid submit', () => {
    component.passwordForm.setValue({
      oldPassword: '123456',
      newPassword: 'StrongPass@123'  
    });

    component.submitForm();
    expect(mockDialogRef.close).toHaveBeenCalledWith({
      oldPassword: '123456',
      newPassword: 'StrongPass@123'
    });
  });
});
